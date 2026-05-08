using BarberBookingAPI.Data;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Jobs;
using BarberBookingAPI.Models;
using BarberBookingAPI.Repository;
using BarberBookingAPI.Service;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Text;

// Configure Serilog for logging to console and file
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console() // Log output to console for development/debugging
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // Log to file, daily rolling
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
var jwtConfiguration = JwtConfigurationValidator.Validate(builder.Configuration);

builder.Host.UseSerilog();

// Clear default logging providers and add console logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add controllers and enable Swagger endpoints explorer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT authentication support
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Configure Entity Framework DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure ASP.NET Core Identity for user management and password policies
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders(); // Enables token providers for password reset, email confirmation, etc.

// Configure Hangfire for background job processing with SQL Server storage
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// Configure cookie policy to handle SameSite cookie attribute
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// Configure Authentication schemes
builder.Services.AddAuthentication(options =>
{
    // Set JWT Bearer as the default scheme for authentication and challenge
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    // Google OAuth will use its own scheme triggered explicitly
})
// Configure JWT Bearer options for validating tokens
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfiguration.Issuer,
        ValidAudience = jwtConfiguration.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SigningKey)),

        RoleClaimType = ClaimTypes.Role 
    };
})
// Add Cookie authentication required for Google OAuth flow
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
// Configure Google OAuth login
.AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";

    // After successful Google login, sign in user with Cookie authentication scheme
    googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    // Handle Google OAuth remote failures gracefully
    googleOptions.Events.OnRemoteFailure = context =>
    {
        var failure = context.Failure?.Message ?? "Unknown error";
        Console.WriteLine("Google remote failure: " + failure);
        context.HandleResponse();
        context.Response.Redirect("/error?message=" + Uri.EscapeDataString(failure));
        return Task.CompletedTask;
    };
});

// Register application services and repositories for dependency injection
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IBarberServiceRepository, BarberServiceRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAppointmentReminderJob, AppointmentReminderJob>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCookiePolicy();

app.UseAuthentication(); // Enables authentication middleware (JWT and cookies)
app.UseAuthorization();  // Enables authorization middleware

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorizationFilter(app.Environment) }
});
RecurringJob.AddOrUpdate<AppointmentReminderJob>(
    "send-appointment-reminders",
    job => job.SendRemindersAsync(),
    Cron.Minutely
);


app.MapControllers();

// Endpoint to handle errors from external authentication (e.g. Google login failures)
app.MapGet("/error", (HttpContext context) =>
{
    var message = context.Request.Query["message"];
    return Results.Content($"Error during external authentication: {message}");
});

app.Run();

public sealed class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IWebHostEnvironment _environment;

    public HangfireDashboardAuthorizationFilter(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public bool Authorize(DashboardContext context)
    {
        if (_environment.IsDevelopment())
            return true;

        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == true
            && httpContext.User.IsInRole("Admin");
    }
}

public sealed class JwtConfiguration
{
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SigningKey { get; init; } = string.Empty;
}

public static class JwtConfigurationValidator
{
    private const int MinimumSigningKeyLength = 32;

    public static JwtConfiguration Validate(IConfiguration configuration)
    {
        var issuer = GetRequiredValue(configuration, "JWT:Issuer");
        var audience = GetRequiredValue(configuration, "JWT:Audience");
        var signingKey = GetRequiredSecretValue(configuration, "JWT:SigningKey");

        if (signingKey.Length < MinimumSigningKeyLength)
            throw new InvalidOperationException($"Configuration 'JWT:SigningKey' must be at least {MinimumSigningKeyLength} characters long.");

        return new JwtConfiguration
        {
            Issuer = issuer,
            Audience = audience,
            SigningKey = signingKey
        };
    }

    private static string GetRequiredValue(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Missing required configuration value '{key}'. Use environment variables or user-secrets for secrets.");

        if (LooksLikePlaceholder(value))
            throw new InvalidOperationException($"Configuration value '{key}' still contains a placeholder. Use environment variables or user-secrets.");

        return value;
    }

    private static string GetRequiredSecretValue(IConfiguration configuration, string key)
    {
        var value = GetRequiredValue(configuration, key);

        if (configuration is not IConfigurationRoot configurationRoot)
            throw new InvalidOperationException($"Configuration '{key}' source could not be verified. Use environment variables or user-secrets.");

        var winningProvider = configurationRoot.Providers
            .Reverse()
            .FirstOrDefault(provider =>
                provider.TryGet(key, out var providerValue) &&
                string.Equals(providerValue, value, StringComparison.Ordinal));

        if (winningProvider == null || !IsAllowedSecretProvider(winningProvider))
            throw new InvalidOperationException($"Configuration '{key}' must be supplied by environment variables or user-secrets, not appsettings files.");

        return value;
    }

    private static bool IsAllowedSecretProvider(IConfigurationProvider provider)
    {
        var providerType = provider.GetType().FullName ?? string.Empty;
        var providerDescription = provider.ToString() ?? string.Empty;

        return providerType.Contains("EnvironmentVariablesConfigurationProvider", StringComparison.OrdinalIgnoreCase)
            || providerDescription.Contains("secrets.json", StringComparison.OrdinalIgnoreCase);
    }

    private static bool LooksLikePlaceholder(string value)
    {
        return value.Contains("REPLACE_", StringComparison.OrdinalIgnoreCase)
            || value.Contains("YOUR_", StringComparison.OrdinalIgnoreCase)
            || value.Contains("YOUR-", StringComparison.OrdinalIgnoreCase)
            || value.Contains("PLACEHOLDER", StringComparison.OrdinalIgnoreCase)
            || value.Contains("SIGNING_KEY_HERE", StringComparison.OrdinalIgnoreCase);
    }
}
