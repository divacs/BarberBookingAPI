using BarberBookingAPI.Data;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Models;
using BarberBookingAPI.Repository;
using BarberBookingAPI.Service;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

// Configure Serilog for logging to console and file
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console() // Log output to console for development/debugging
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // Log to file, daily rolling
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

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
        ValidIssuer = builder.Configuration["JWT:Issuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])),

        ValidateLifetime = true, // Validate token expiration
        ClockSkew = TimeSpan.Zero
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

app.UseHangfireDashboard();

app.MapControllers();

// Endpoint to handle errors from external authentication (e.g. Google login failures)
app.MapGet("/error", (HttpContext context) =>
{
    var message = context.Request.Query["message"];
    return Results.Content($"Error during external authentication: {message}");
});

app.Run();
