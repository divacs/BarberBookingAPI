
# 💈 BarberBookingAPI

BarberBookingAPI is an ASP.NET Core Web API project for booking appointments in a barbershop. It supports user registration and login, JWT authentication, role-based authorization, sending email confirmations, and includes Single Sign-On (SSO) via Google. The application is structured using Clean Architecture principles and follows security best practices by separating sensitive configuration.

---

## 🚀 Technologies and Libraries

- ASP.NET Core 8.0
- Entity Framework Core
- SQL Server
- JWT (JSON Web Tokens)
- ASP.NET Identity
- MailKit (email service)
- Google OAuth 2.0 (SSO)
- Hangfire (background jobs)
- Repository Pattern
- Custom Mapping Classes
- Clean Architecture

---

## 📂 Project Structure (Clean Architecture)

The project follows **Clean Architecture** principles:

```
BarberBookingAPI/
│
├── Controllers/              // API controllers (Account, Appointments, Auth, Services, TestJob)
├── Data/                     // ApplicationDBContext (EF Core)
├── DTOs/                     // Data Transfer Objects (DTOs)
│   ├── Account/
│   ├── Appointment/
│   └── BarberService/
├── Helpers/                  // Query objects (for pagination/filtering)
├── Interfaces/              // Repository and service interfaces
├── Mapper/                  // Custom mapping classes
├── Migrations/              // EF Core migrations
├── Models/                  // Entity models
├── Repository/              // Repository implementations
├── Service/                 // Email, Token, Background jobs (Hangfire)
├── Jobs/                    // Hangfire job implementations
├── Logs/                    // Hangfire job logs (optional)
├── appsettings.json         // General configuration
├── appsettings.Development.json // Development secrets (excluded from Git)
```

---

## 🔐 Authentication and Authorization

- Uses ASP.NET Identity for user management
- JWT is used for authenticating users
- Two roles supported: `Admin` and `User`
- Google SSO is implemented for external login via OAuth 2.0

### 🔑 Endpoints

- `POST /api/account/register` – Register a new user
- `POST /api/account/login` – Log in and get JWT token
- `GET /auth/login-google` – Redirects to Google for SSO login

---

## 📅 Appointment Booking

Users can:

- View available barber services
- Book appointments
- View, update, or delete their appointments

---

## 📬 Email Confirmation

A confirmation email is sent when an appointment is created.

- Implemented using **MailKit**
- Configuration stored in `appsettings.json` and overridden in `appsettings.Development.json`

---

## ⏰ Appointment Reminder (Scheduled Job with Hangfire)

- Implemented background job that sends email reminders **1 hour before** the scheduled appointment.
- Used **Hangfire** to schedule and execute the job.
- Reminder includes the appointment details and is sent only if the start time is in the future.
- The job is scheduled automatically after an appointment is created.
- The reminder job ID is stored in the database.

✅ The job was tested manually using Hangfire Dashboard and via a dedicated controller for test endpoints.

---

## 🧪 TestJobController

- Created a separate `TestJobController` to isolate all testing endpoints from production logic.
- Allows manual execution and scheduling of reminder jobs.

---

## 🌐 Google Authentication (SSO)

- Configured via **Google Developer Console**
- Requires `ClientId` and `ClientSecret`
- These credentials are stored securely in `appsettings.Development.json` (excluded from Git)

---

## 🔐 Configuration and Security

To avoid leaking sensitive credentials (e.g., email, JWT, Google secrets):

- `appsettings.json` contains general configuration
- `appsettings.Development.json` contains secrets and is **excluded** from Git

`.gitignore` includes:

```
appsettings.Development.json
```

Ensure `appsettings.Development.json` includes:

```json
"Authentication": {
  "Google": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  }
},
"EmailSettings": {
  "From": "your@email.com",
  "SmtpServer": "smtp.provider.com",
  "Port": 465,
  "Username": "your@email.com",
  "Password": "yourPassword"
}
```

---

## 📌 Query Objects

Custom query objects are used for:

- Pagination: `?PageNumber=1&PageSize=10`
- Filtering: Search by parameters

---

## 📁 Database

SQL Server is used. Entity Framework Core handles migrations and seeding.

### Commands:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## ✅ How to Run

1. Add your `appsettings.Development.json` with secrets
2. Apply migrations:
   ```bash
   dotnet ef database update
   ```
3. Run the project:
   ```bash
   dotnet run
   ```
4. Open Swagger UI at `https://localhost:<port>/swagger`

---

## 📥 Sample User (for testing)

```json
{
  "username": "probni",
  "email": "probni@email.com",
  "password": "Probni_123456"
}
```

---

## 🙌 Author

- 👩 Sonja Divac (2025)
- Project created to demonstrate .NET Web API skills using modern architecture, testing practices, and background job handling with Hangfire.
