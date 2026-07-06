# Quizzy

Quizzy is a personal ASP.NET Core MVC project for online quiz practice, subject registration, learning packages, quiz attempts, review history, and basic business dashboards.

The project was originally built from an academic learning context and has been reorganized as a personal portfolio project to continue improving the product, backend design, UI, and deployment readiness.

## Features

- User registration, email verification, login, logout, password change, and password reset.
- User profile management with avatar upload.
- Subject browsing, searching, category/tag filtering, and subject detail pages.
- Learning package registration by subject.
- Registration management with submitted, paid/registered, and cancelled states.
- Practice creation based on subject, level, question group, and number of questions.
- Quiz attempt flow with question loading, answer submission, scoring, and finish state.
- Quiz review page with submitted answers and correct answers.
- Blog listing, blog detail pages, categories, and latest posts.
- Simulation exam data model and basic exam listing flow.
- Dashboard APIs for registrations, revenue, customer stats, order counts, and subject revenue.

## Tech Stack

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core 8
- SQL Server
- Dapper
- Razor Views
- Bootstrap
- jQuery
- Chart.js
- Swiper
- Selenium/WebDriver packages for browser-based verification

## Project Structure

```text
Quizzy/
+-- ProjectBase/
|   +-- Controllers/        # MVC controllers and JSON API controllers
|   +-- Helpers/            # EF Core DbContext and helper classes
|   +-- Migrations/         # Entity Framework migrations and seed data
|   +-- Models/             # Entity models and view models
|   +-- Views/              # Razor views
|   +-- wwwroot/            # Static assets: CSS, JS, images, libraries
|   +-- Program.cs          # Application startup and middleware
|   +-- ProjectBase.csproj  # ASP.NET Core project file
+-- SWP391.sln              # Visual Studio solution
+-- README.md
```

## Requirements

- .NET SDK 8.0 or later
- SQL Server / SQL Server Express / Local SQL Server instance
- Visual Studio 2022 or another editor that supports .NET projects

## Configuration

The default connection string is defined in `ProjectBase/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ConnectedDb": "Server=localhost;Database=SWP391;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Update the connection string if your SQL Server instance uses another server name, SQL login, or database name.

For local-only secrets such as SMTP credentials, prefer environment variables, user secrets, or an untracked local configuration file instead of committing credentials to source control.

## Database Setup

From the repository root:

```powershell
dotnet restore .\ProjectBase\ProjectBase.csproj
dotnet ef database update --project .\ProjectBase\ProjectBase.csproj
```

If `dotnet ef` is not installed:

```powershell
dotnet tool install --global dotnet-ef
```

## Run Locally

From the repository root:

```powershell
dotnet run --project .\ProjectBase\ProjectBase.csproj --launch-profile http
```

The default local URL is:

```text
http://localhost:5152
```

You can also run the solution from Visual Studio by selecting the `ProjectBase` startup project.

## Main Backend Flows

### Account Flow

```text
Register -> email verification -> login -> authenticated session -> profile/password actions
```

The account module is handled mainly by `AccountController`.

### Subject Registration Flow

```text
Browse subject -> select package -> submit registration -> pay/activate registration -> access practice
```

Subject and package registration are handled mainly by `Subjects`, `SubjectRegister`, and `MyRegistrationsApiController`.

### Practice And Quiz Flow

```text
Create practice -> generate QuizHandle rows from QuizBank -> answer questions -> finish attempt -> review result
```

Practice and quiz attempts are handled mainly by `PracticeApiController`, `QuizController`, `QuizApiController`, and `QuizReview`.

### Dashboard Flow

```text
Registrations + packages + users -> revenue/customer/order statistics
```

Dashboard data is served by `DashboardApiController`.

## Build

```powershell
dotnet build .\ProjectBase\ProjectBase.csproj
```

## Status

This is a personal learning and portfolio project. It is functional locally and is being refined toward cleaner architecture, safer backend logic, and better deployment readiness.
