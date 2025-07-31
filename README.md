
# 💈 BarberBookingAPI

BarberBookingAPI is an ASP.NET Core Web API project for booking appointments in a barbershop. It supports user registration and login, JWT authentication, role-based authorization, sending email confirmations, and is structured using Clean Architecture principles.

---

## 🚀 Technologies and Libraries

- ASP.NET Core 8.0
- Entity Framework Core
- SQL Server
- JWT (JSON Web Tokens)
- Identity Framework
- MailKit (for sending emails)
- Repository Pattern
- Custom Mapping Classes
- Clean Architecture Principles

---

## 📂 Project Structure (Clean Architecture)

The project follows **Clean Architecture** principles:

```
BarberBookingAPI/
│
├── Controllers/              // API controllers (Account, Appointments, Services)
├── Data/                     // ApplicationDBContext (EF Core)
├── DTOs/                     // Data Transfer Objects (DTOs)
│   ├── Account/
│   ├── Appointment/
│   └── BarberService/
├── Helpers/                  // Query objects (for pagination/filtering)
├── Interfaces/               // Repository and service interfaces
├── Mapper/                   // Custom mapping classes 
├── Migrations/               // EF Core migrations
├── Models/                   // Entity models
├── Repository/               // Repository implementations
├── Service/                  // Email and Token services
└── appsettings.json          // Configuration (JWT, Email, Connection Strings)
```

---

## 🔐 Authentication and Authorization

- Uses ASP.NET Identity for user management
- JWT is used for authenticating users
- Two roles supported: `Admin` and `User`

### 🔑 Endpoints
- `POST /api/account/register` – Register a new user
- `POST /api/account/login` – Log in and get JWT token

Tokens are generated via the `TokenService.cs`.

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
- Configuration is stored in `appsettings.json`:

```json
"EmailSettings": {
  "From": "your@email.com",
  "SmtpServer": "smtp.yourprovider.com",
  "Port": "465",
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

## 🧼 Clean Architecture

Clean Architecture ensures:
- Separation of concerns
- Easy testing and maintenance
- No direct dependency on framework in core logic

Flow:
`Controller → Repository Interface → Implementation → DB Context`

Models and DTOs are separated and mapping is done through custom mappers.

---

## ✅ How to Run

1. Set up your connection string in `appsettings.json`
2. Configure email credentials in `"EmailSettings"`
3. Apply migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the project:
   ```bash
   dotnet run
   ```
5. Open Swagger UI at `https://localhost:<port>/swagger`

---

## 📥 Sample User (for testing)

```json
{
  "username": "sonja",
  "email": "sonja@email.com",
  "password": "Test123!"
}
```

---

## 🙌 Author

- 👩 Sonja Divac (2025)
- Project created for practicing advanced .NET Web API development with clean architecture and modern patterns.
