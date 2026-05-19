# BarberBookingAPI

BarberBookingAPI is an ASP.NET Core Web API for managing barber services, user accounts, roles, and appointment booking workflows. The project uses SQL Server with Entity Framework Core, ASP.NET Core Identity for account management, JWT bearer authentication for API access, and Hangfire for scheduled appointment reminder jobs.

The codebase is structured as a practical backend API project with separated controllers, DTOs, repositories, services, EF Core migrations, and environment-specific configuration.

## Tech Stack

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server
- ASP.NET Core Identity
- JWT bearer authentication
- Google external authentication
- Hangfire with SQL Server storage
- MailKit / MimeKit for email delivery
- Serilog for structured logging
- Swagger / OpenAPI

## Features

- User registration and login
- JWT token generation with role claims
- Role-based authorization with `Admin`, `User`, and `Worker` roles
- Admin role assignment with validation
- Password reset endpoint for admins
- Barber service CRUD endpoints
- Appointment booking, update, delete, filtering, and pagination
- Appointment reminder background job using Hangfire
- Email sending through SMTP
- Google authentication flow
- Swagger documentation in Development
- Environment-specific configuration for Development and Production

## Project Structure

```text
BarberBookingAPI/
+-- Controllers/        API endpoints
+-- Data/               EF Core DbContext and seed data
+-- DTOs/               Request and response contracts
+-- Interfaces/         Repository and service abstractions
+-- Jobs/               Hangfire background jobs
+-- Mapper/             DTO mapping helpers
+-- Migrations/         EF Core migrations and model snapshot
+-- Models/             Domain and Identity models
+-- Repository/         Data access implementations
+-- Service/            Token and email services
+-- Program.cs          Application startup and middleware
+-- appsettings.json    Shared non-secret configuration
+-- appsettings.*.json  Environment-specific configuration
```

## Architecture Overview

The application follows a conventional layered Web API structure:

- Controllers expose HTTP endpoints and enforce authorization rules.
- DTOs define API request and response shapes.
- Repositories encapsulate EF Core data access.
- Services handle cross-cutting application logic such as token creation and email delivery.
- EF Core migrations define the database schema and seeded Identity roles.
- Hangfire runs recurring background jobs for appointment reminders.

## Environment Configuration

The project supports two environments:

- `Development`
- `Production`

Local debugging is configured through `Properties/launchSettings.json` with:

```text
ASPNETCORE_ENVIRONMENT=Development
```

ASP.NET Core loads configuration in the standard order:

```text
appsettings.json
appsettings.{Environment}.json
user-secrets / environment variables
```

Sensitive values should not be committed. Use user-secrets for local development and environment variables in production.

Required configuration keys:

```text
ConnectionStrings__DefaultConnection
JWT__Issuer
JWT__Audience
JWT__SigningKey
EmailSettings__From
EmailSettings__SmtpServer
EmailSettings__Port
EmailSettings__Username
EmailSettings__Password
Authentication__Google__ClientId
Authentication__Google__ClientSecret
```

Example local user-secrets setup:

```bash
dotnet user-secrets set "JWT:SigningKey" "replace-with-a-strong-local-signing-key"
dotnet user-secrets set "EmailSettings:Username" "local-smtp-user"
dotnet user-secrets set "EmailSettings:Password" "local-smtp-password"
dotnet user-secrets set "Authentication:Google:ClientId" "local-google-client-id"
dotnet user-secrets set "Authentication:Google:ClientSecret" "local-google-client-secret"
```

## Setup and Installation

Prerequisites:

- .NET 8 SDK
- SQL Server or SQL Server Express
- EF Core CLI tools

Install EF Core CLI tools if needed:

```bash
dotnet tool install --global dotnet-ef
```

Restore dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

## Database and Migrations

Update the configured SQL Server database:

```bash
dotnet ef database update
```

Check whether the EF model has pending changes:

```bash
dotnet ef migrations has-pending-model-changes --no-build
```

The application seeds the following Identity roles with deterministic IDs:

- `Admin`
- `User`
- `Worker`

## Running Locally

Start the API:

```bash
dotnet run
```

With the default development launch profile, the API runs on:

```text
http://localhost:5246
```

## API Documentation

Swagger is enabled only in the Development environment.

After starting the application locally, open:

```text
http://localhost:5246/swagger
```

Production does not expose Swagger or the developer exception page.

## Authentication Overview

The API uses ASP.NET Core Identity for user management and JWT bearer tokens for authenticated API access.

JWT tokens include role claims and are validated using:

- issuer
- audience
- signing key
- token lifetime
- signing key validation

Protected endpoints use `[Authorize]` and role-based policies such as:

```csharp
[Authorize(Roles = "Admin")]
```

Google authentication is configured as an external authentication flow. External auth failures are logged server-side and return a safe generic error response to the client.

## Background Jobs

Hangfire is configured with SQL Server storage and runs a recurring appointment reminder job:

```text
send-appointment-reminders
```

The Hangfire dashboard is available at:

```text
/hangfire
```

In Development, the dashboard is accessible for local debugging. In Production, access is restricted to authenticated users in the `Admin` role.

## Production Considerations

- Set `ASPNETCORE_ENVIRONMENT=Production`.
- Provide secrets through environment variables or a secure secret store.
- Do not store production connection strings, JWT signing keys, SMTP credentials, or Google OAuth credentials in appsettings files.
- Keep Swagger and developer exception pages disabled in Production.
- Use HTTPS termination at the hosting layer.
- Configure SQL Server with production-grade credentials and backups.
- Review Hangfire dashboard access before deployment.
- Monitor application logs and background job failures.

## Security Considerations

- JWT signing keys must be strong and at least 32 characters long.
- The production JWT signing key must come from environment variables or a secure secret source.
- Exception details are logged server-side and should not be returned to API clients.
- Role assignment validates the requested role before modifying existing user roles.
- SMTP certificate bypass is limited to explicit Development configuration.
- Dependency vulnerability checks should be part of regular maintenance:

```bash
dotnet list package --vulnerable --include-transitive
```

## Useful Commands

```bash
dotnet restore
dotnet build
dotnet ef database update
dotnet ef migrations has-pending-model-changes --no-build
dotnet run
```

## Future Improvements

- Add automated integration tests for authentication and appointment workflows.
- Add CI checks for build, EF migration drift, and vulnerable packages.
- Add refresh token support if longer-lived sessions are required.
- Add rate limiting for authentication endpoints.
- Add Docker configuration for repeatable local and deployment environments.
