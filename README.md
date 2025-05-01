# Forgetting Curve – Review Scheduling App

> A web application to help users manage long-term learning through automated review scheduling based on the Ebbinghaus forgetting curve.

[![Angular Version](https://img.shields.io/badge/Angular-17-blue)](https://angular.io/) [![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)

## Table of Contents

- [Tech Stack](#tech-stack)
- [Getting Started Locally](#getting-started-locally)
  - [Prerequisites](#prerequisites)
  - [Clone the Repository](#clone-the-repository)
  - [Backend Setup](#backend-setup)
  - [Frontend Setup](#frontend-setup)
- [Available Scripts](#available-scripts)
  - [Frontend (npm)](#frontend-npm)
  - [Backend (dotnet)](#backend-dotnet)
- [Project Scope](#project-scope)
- [Project Status](#project-status)
- [License](#license)

## Tech Stack

- **Frontend**
  - Angular 17 (Standalone Components)
  - TypeScript 5
  - Angular Material
- **Backend**
  - ASP.NET Core 8
  - Entity Framework Core + Microsoft SQL Server
  - ASP.NET Core Identity
  - Mailtrap (email confirmation)
- **CI/CD & Hosting**
  - GitHub Actions (build, Docker image creation)
  - Docker (frontend served via Nginx; backend in ASP.NET container)
  - DigitalOcean (Droplets or Kubernetes)

## Getting Started Locally

### Prerequisites

- Node.js (LTS version, e.g. ≥16.x)  
  _Note: Add a `.nvmrc` if you require a specific version._
- .NET 8.0 SDK  
- Microsoft SQL Server (local or remote instance)
- (Optional) Mailtrap account for email confirmations

### Clone the Repository

```bash
git clone https://github.com/<your-username>/forgetting-curve.git
cd forgetting-curve
```

### Backend Setup

1. Navigate to the API project:

   ```bash
   cd src/Server/ForgettingCurve.Api
   ```

2. Configure your database connection and Mailtrap credentials:  
   - Edit `appsettings.Development.json` or set environment variables:  
     ```bash
     export ConnectionStrings__DefaultConnection="Server=.;Database=ForgettingCurveDb;Trusted_Connection=True;"
     export Mailtrap__Username="YOUR_MAILTRAP_USERNAME"
     export Mailtrap__Password="YOUR_MAILTRAP_PASSWORD"
     export ASPNETCORE_ENVIRONMENT=Development
     ```
3. (If migrations are included) Apply EF Core migrations:

   ```bash
   dotnet tool install --global dotnet-ef
   dotnet ef database update
   ```

4. Run the API:

   ```bash
   dotnet run
   ```

5. Swagger UI will be available at `https://localhost:5001/swagger`

### Frontend Setup

1. Open a new terminal, go to the client folder:

   ```bash
   cd src/ClientApp
   ```

2. Install dependencies:

   ```bash
   npm install
   ```

3. Start the development server:

   ```bash
   npm start
   ```

4. The application will launch at `http://localhost:4200/`

## Available Scripts

### Frontend (npm)

| Command         | Description                                         |
| --------------- | --------------------------------------------------- |
| `npm start`     | Runs `ng serve` for local development               |
| `npm run build` | Builds the app in production mode (`ng build`)      |
| `npm run watch` | Builds and watches for changes in development mode  |
| `npm test`      | Runs unit tests via Karma/Jasmine                   |

### Backend (dotnet)

| Command              | Description                                   |
| -------------------- | --------------------------------------------- |
| `dotnet run`         | Runs the ASP.NET Core API                    |
| `dotnet ef migrations` | (Optional) Manage EF Core migrations        |
| `dotnet ef database update` | (Optional) Apply migrations to the database |

## Project Scope

**MVP Features**  
- User registration, email verification, login  
- Private management of learning _Scopes_ and _Topics_  
- CRUD operations for Scopes and Topics (with validation and confirmations)  
- Automatic scheduling of reviews at 1, 3, 7, 14, 30 days  
- Recalculation of future review dates when a review is delayed  
- Option to continue 30-day reviews or mark a Topic as "Mastered"  
- Central Review Panel ("What's due today?") with overdue highlighting  
- Mark/unmark reviews and revert last review  
- Simple, responsive UI with Angular Material  

_Functionalities outside MVP (e.g., data export, social features, push notifications) are intentionally excluded._

## Project Status

**MVP in development** – Core flows (registration, scheduling, review management) are implemented.  
Next steps: database migrations, email service configuration, styling refinements, end-to-end tests.

## License

MIT
