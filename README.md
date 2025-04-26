# Spaced Repetition Planner

## Table of Contents
1. [Project Description](#project-description)  
2. [Tech Stack](#tech-stack)  
3. [Getting Started Locally](#getting-started-locally)  
4. [Available Scripts](#available-scripts)  
5. [Project Scope](#project-scope)  
6. [Project Status](#project-status)  
7. [License](#license)  

## Project Description
Spaced Repetition Planner is a web application that automates the scheduling of review sessions based on the scientifically validated forgetting curve (Ebbinghaus). Users can create **Scopes** (subjects) and **Topics** (units of study), manage their review schedule, and track progress through daily and overdue review panels.

## Tech Stack
- **Frontend**  
  - Angular 17  
  - TypeScript 5  
  - Angular Material  

- **Backend**  
  - ASP.NET Core 8  
  - Entity Framework Core (+ Npgsql)  
  - Supabase Auth (email verification) + .NET JWT validation  

- **AI Integration**  
  - Openrouter.ai (communicates with AI models)  

- **CI/CD & Hosting**  
  - GitHub Actions (build & deploy)  
  - Docker (DigitalOcean)  
  - Nginx (serves frontend)  

## Getting Started Locally

### Prerequisites
- Node.js (≥18.x) & npm  
- Angular CLI  
- .NET SDK 8.0  
- Docker & Docker Compose (optional, for containerized setup)  

### Installation Steps
1. Clone the repository and navigate into it:  
   ```bash
   git clone <repo-url>
   cd <repo-root>
   ```

2. Configure environment variables:  
   - Create `.env` in **frontend** directory:
     ```
     VITE_SUPABASE_URL=your_supabase_url
     VITE_SUPABASE_ANON_KEY=your_supabase_anon_key
     ```
   - Create `.env` in **backend** directory:
     ```
     SUPABASE_URL=your_supabase_url
     SUPABASE_SERVICE_ROLE_KEY=your_supabase_service_role_key
     OPENROUTER_API_KEY=your_openrouter_api_key
     ```

3. Start the **frontend**:
   ```bash
   cd frontend
   npm install
   npm run start
   ```

4. Start the **backend**:
   ```bash
   cd ../backend
   dotnet restore
   dotnet run
   ```

5. Open your browser at `http://localhost:4200`

*(Alternatively, use Docker Compose at project root)*  
```bash
docker-compose up --build
```

## Available Scripts

### Frontend (Angular)
- `npm run start` — launch in development mode  
- `npm run build` — build for production  
- `npm run test` — run unit tests  
- `npm run lint` — lint codebase  

### Backend (.NET)
- `dotnet run` — start API  
- `dotnet test` — run unit tests  
- `dotnet ef migrations add <Name>` — add a new migration  
- `dotnet ef database update` — apply migrations to database  

### Docker
- `docker-compose up --build` — build & run containers  
- `docker-compose down` — stop & remove containers  

## Project Scope

### Included (MVP)
- **Authentication & Registration**  
  - Supabase Auth with email verification  
  - Password policies (min. 8 chars, uppercase, lowercase, digit)  
- **CRUD for Scopes & Topics**  
  - Create/Edit/Delete with validation  
  - Two-step confirmation when deleting a Scope  
  - Filtering and sorting of Topics (hide/show mastered)  
- **Automated Review Scheduling**  
  - Fixed intervals: 1, 3, 7, 14, 30 days  
  - Recalculation on delayed review  
  - Post-30-day review decision (continue or mark as mastered)  
- **“What’s for today?” Panel**  
  - Aggregates due and overdue reviews  
  - Overdue items visually highlighted  
- **Review Tracking**  
  - Mark as completed / undo last completion  
- **Mastered Topics**  
  - Mark/unmark as mastered; hidden from daily panel  

### Excluded
- Customizable review algorithm  
- Data import/export (CSV)  
- Social or mobile-specific features  
- Notifications (email, push)  
- Advanced analytics or gamification  
- Tagging beyond basic scopes & topics  

## Project Status
**MVP Phase** — Core features implemented; some modules under active development.

## License
_This project does not yet have a license. License terms to be determined._
