## Frontend - Angular jako główny framework

*   **Angular 17:** Kompleksowy framework do budowy interaktywnych aplikacji SPA (Single Page Application) z architekturą opartą na komponentach (w tym Standalone Components).
*   **TypeScript 5:** Zapewnia statyczne typowanie, poprawiając jakość kodu i wsparcie IDE w środowisku Angular.
*   **Angular Material:** Kompleksowa biblioteka komponentów UI zgodnych z Material Design, zapewniająca responsywny layout, gotowe komponenty (przyciski, formularze, tabele, dialogi) oraz spójny wygląd całego interfejsu.

## Backend - .NET (ASP.NET Core)

*   **ASP.NET Core 8:** Wydajny framework .NET do budowy RESTful API obsługujących żądania z frontendu Angular.
*   **Entity Framework Core + Microsoft SQL Server:** Standardowy ORM .NET do interakcji (CRUD) z bazą danych MS SQL Server, używając LINQ i modeli C#.
*   **ASP.NET Core Identity:** Wbudowane rozwiązanie do uwierzytelniania i zarządzania użytkownikami w aplikacjach ASP.NET Core.
*   **SendGrid:** Usługa do wysyłki e-maili, zwłaszcza do potwierdzeń rejestracji, resetowania haseł i powiadomień, zintegrowana przez pakiet SendGrid dla .NET.

## CI/CD i Hosting

*   **GitHub Actions:** Automatyzacja procesu budowania (Angular `ng build`, .NET `dotnet publish`), tworzenia obrazów Docker (frontend + backend) i wdrażania na serwerze.
*   **DigitalOcean (Docker):** Hosting aplikacji w kontenerach Docker (frontend Angular serwowany przez Nginx, backend ASP.NET Core) na infrastrukturze DigitalOcean (Droplets lub Kubernetes).     