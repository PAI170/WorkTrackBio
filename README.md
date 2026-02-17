# WorkTrackBio API

A RESTful API for managing employees, projects, and work attendance — built with ASP.NET Core and designed with biometric integration in mind.

## What is this?

WorkTrackBio is a backend system I built to handle the core operations of a workforce management platform. The idea behind it is to eventually support biometric devices for attendance tracking, but for now it covers the fundamentals: managing employees, assigning them to projects, tracking attendance, and keeping everything organized through a relational database.

Still a work in progress, but the core functionality is up and running.

## Tech Stack

- **ASP.NET Core 9** — Web API framework
- **Entity Framework Core** — ORM with Code First approach
- **SQL Server** — Relational database
- **AutoMapper** — Object mapping between models and DTOs
- **FluentValidation** — Input validation

## Project Structure

The project follows a layered architecture with a clear separation of responsibilities:

```
WorkTrackBio.API/
├── Controllers/        # Handles HTTP requests and responses
├── Services/           # Business logic layer
├── Repositories/       # Data access layer (Repository Pattern)
├── Data/
│   ├── Models/         # Database entities
│   └── Context/        # EF Core DbContext
├── DataTransferObjects/ # DTOs for Create, Update, and Read operations
├── Mappers/            # AutoMapper profiles
├── Validators/         # Input validation logic
├── Migrations/         # EF Core database migrations
└── Common/             # Shared utilities like ApiResponse
```

Each feature (Employee, Project, Assistance, Role, etc.) has its own set of files across all these layers.

## Architecture

The API uses the **Repository + Service pattern**:

- **Repository** — Talks directly to the database via EF Core
- **Service** — Contains the business rules and calls the repository
- **Controller** — Receives the HTTP request, calls the service, and returns a standardized response

Every endpoint returns a consistent `ApiResponse<T>` object so the client always gets the same response shape regardless of success or failure.

## What's Covered

- Employee management (CRUD)
- Project tracking with maintenance and warranty records
- Attendance registration
- Role and state management
- Document type handling
- Relational database design with proper foreign keys, indexes, and cascade rules

## Database

The schema was designed with a relational model in mind — entities like `EmployeeInfo`, `Project`, `Assistance`, and `InternUser` are properly related through foreign keys. Migrations are handled with EF Core.

To apply migrations:

```bash
dotnet ef database update
```

## Running the Project

```bash
# Clone the repo
git clone https://github.com/pai170/WorkTrackBio.git

# Navigate to the API folder
cd WorkTrackBio.API

# Update the connection string in appsettings.json

# Run
dotnet run
```

## What's Next

- JWT authentication and session management
- Biometric device integration
- Swagger documentation
- Audit logging

---

Built as a personal project to practice backend development with .NET and improve my understanding of clean architecture patterns.
