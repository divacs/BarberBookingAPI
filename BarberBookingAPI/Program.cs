using BarberBookingAPI.Data;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Models;
using BarberBookingAPI.Repository;
using BarberBookingAPI.Service;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;


// var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console() // Log to console for development and debugging
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Override default logging
builder.Host.UseSerilog();

// Configure logging to console 
// This is useful for debugging and monitoring in development and production environments
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// Adding Swagger documentation and JWT authentication configuration
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
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
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<ApplicationDBContext>();

// adding the SCHEME for JWT and Google authentication
// JWT is used as the default authentication scheme for protected endpoints.
// Google uses its own GoogleDefaults.AuthenticationScheme, which is fine 
// since it's triggered explicitly via Challenge(...) when needed.
builder.Services.AddAuthentication(options =>
{
    // DefaultScheme = JWT
    // This means HttpContext.User for protected endpoints will expect a JWT token 
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; 
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultForbidScheme =
    //options.DefaultScheme =
    //options.DefaultSignInScheme =
    //options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Events = new CookieAuthenticationEvents
    {
        OnValidatePrincipal = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation($"Validating cookie for {context.Principal.Identity.Name}");
            return Task.CompletedTask;
        },
        OnRedirectToLogin = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation($"Redirect to login: {context.RedirectUri}");
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation($"Access denied redirect: {context.RedirectUri}");
            return Task.CompletedTask;
        }
    };
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.CallbackPath = "/api/GoogleAuth/google-response";
    googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    // GoogleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Use cookie scheme for Google sign-in
    // This is important: the Google middleware uses the cookie scheme to "sign in" the user after a successful login
    // This allows the application to maintain the user's session after they authenticate with Google
    googleOptions.Events.OnRemoteFailure = context =>
    {
        Console.WriteLine("OAuth remote failure: " + context.Failure?.Message);
        context.HandleResponse(); 
        context.Response.Redirect("/error?message=" + Uri.EscapeDataString(context.Failure?.Message ?? "Unknown error"));
        return Task.CompletedTask;
    };
});
// GoogleOptions.SignInScheme = Cookie
// Very important: the Google middleware uses the cookie scheme to "sign in" the user after a successful login


builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IBarberServiceRepository, BarberServiceRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger in development mode for testing SSO google login and other endpoints
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard();

app.MapControllers();

// Endpoint to redirect to Google's login page
app.MapGet("/error", (HttpContext context) =>
{
    var message = context.Request.Query["message"];
    return Results.Content($"Error during external authentication: {message}");
});

app.Run();