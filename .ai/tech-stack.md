## Frontend - Angular jako główny framework

*   **Angular 17:** Kompleksowy framework do budowy interaktywnych aplikacji SPA (Single Page Application) z architekturą opartą na komponentach (w tym Standalone Components).
*   **TypeScript 5:** Zapewnia statyczne typowanie, poprawiając jakość kodu i wsparcie IDE w środowisku Angular.
*   **Angular Material:** Kompleksowa biblioteka komponentów UI zgodnych z Material Design, zapewniająca responsywny layout, gotowe komponenty (przyciski, formularze, tabele, dialogi) oraz spójny wygląd całego interfejsu.

## Backend - .NET (ASP.NET Core) z wykorzystaniem usług Supabase

*   **ASP.NET Core 8:** Wydajny framework .NET do budowy RESTful API obsługujących żądania z frontendu Angular.
*   **Entity Framework Core + Npgsql:** Standardowy ORM .NET do interakcji (CRUD) z bazą danych PostgreSQL hostowaną na Supabase, używając LINQ i modeli C#.
*   **Supabase Auth + .NET JWT Validation:** Frontend obsługuje logowanie/rejestrację przez Supabase Auth (`supabase-js`). Backend .NET jedynie waliduje otrzymane tokeny JWT (wystawione przez Supabase) w celu zabezpieczenia API.

## AI - Komunikacja z modelami przez usługę Openrouter.ai

*   **Openrouter.ai:** Backend .NET komunikuje się z tą usługą, aby uzyskać dostęp do różnorodnych modeli AI (OpenAI, Anthropic itp.), optymalizując koszty i wydajność.
*   **Zarządzanie API Keys & Limits:** Klucze API Openrouter i logika limitów kosztów zarządzane są bezpiecznie po stronie backendu .NET.

## CI/CD i Hosting

*   **GitHub Actions:** Automatyzacja procesu budowania (Angular `ng build`, .NET `dotnet publish`), tworzenia obrazów Docker (frontend + backend) i wdrażania na serwerze.
*   **DigitalOcean (Docker):** Hosting aplikacji w kontenerach Docker (frontend Angular serwowany przez Nginx, backend ASP.NET Core) na infrastrukturze DigitalOcean (Droplets lub Kubernetes).     