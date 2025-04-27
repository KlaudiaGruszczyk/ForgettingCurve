# API Endpoint Implementation Plan: Rejestracja nowego użytkownika

## 1. Przegląd punktu końcowego

Endpoint rejestracji umożliwia nowym użytkownikom utworzenie konta w aplikacji poprzez podanie adresu e-mail i hasła. Po pomyślnej rejestracji, system wysyła wiadomość e-mail weryfikacyjną z unikalnym linkiem, który musi zostać kliknięty w celu aktywacji konta.

## 2. Szczegóły żądania

- **Metoda HTTP:** POST
- **Struktura URL:** `/api/auth/register`
- **Parametry:** Brak parametrów ścieżki
- **Nagłówki:**
  - `Content-Type: application/json` (wymagany)
- **Request Body:**
  ```json
  {
    "email": "string",
    "password": "string",
    "confirmPassword": "string"
  }
  ```

## 3. Wykorzystywane typy

### DTO (Data Transfer Objects)

```csharp
namespace ForgettingCurve.API.Contracts.Requests
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

namespace ForgettingCurve.API.Contracts.Responses
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
```

### Command (CQRS)

// Plik: RegisterUserCommand.cs
namespace ForgettingCurve.Application.Auth.Commands
{
    public class RegisterUserCommand : IRequest<RegisterResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

// Plik: RegisterUserCommandHandler.cs
namespace ForgettingCurve.Application.Auth.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterResponse>
    {
        // Implementacja handlera
    }
}

## 4. Szczegóły odpowiedzi

### Struktura odpowiedzi (200 OK)

```json
{
  "success": true,
  "message": "Rejestracja udana. Sprawdź email, aby zweryfikować konto."
}
```

### Kody statusu

- **200 OK:** Pomyślna rejestracja
- **400 Bad Request:** Nieprawidłowe dane (np. format e-mail, hasło nie spełnia wymagań, hasła nie pasują do siebie)
- **409 Conflict:** E-mail już istnieje w systemie
- **500 Internal Server Error:** Błąd serwera

## 5. Przepływ danych

1. Kontroler otrzymuje żądanie HTTP POST z danymi rejestracyjnymi
2. System waliduje format danych wejściowych:
   - Poprawność formatu adresu e-mail
   - Hasło spełnia wymogi złożoności (min. 8 znaków, 1 wielka litera, 1 mała litera, 1 cyfra)
   - Pola Password i ConfirmPassword są identyczne
3. Kontroler tworzy i wysyła command RegisterUserCommand poprzez MediatR
4. Handler wykonuje następujące operacje:
   - Sprawdza, czy użytkownik o podanym adresie e-mail już nie istnieje
   - Tworzy nowego użytkownika z wykorzystaniem ASP.NET Core Identity
   - Generuje token weryfikacyjny dla konta
   - Tworzy link weryfikacyjny zawierający token i adres e-mail
   - Wysyła e-mail z linkiem weryfikacyjnym do użytkownika (poprzez SendGrid)
   - Ustawia konto jako nieaktywne do czasu weryfikacji
   - Zwraca wynik operacji
5. Kontroler zwraca odpowiednią odpowiedź HTTP

## 6. Względy bezpieczeństwa

1. **Walidacja danych wejściowych:**
   - Format adresu e-mail musi być poprawny
   - Hasło musi spełniać wymagania złożoności: minimum 8 znaków, co najmniej 1 wielka litera, 1 mała litera, 1 cyfra
   - Pola Password i ConfirmPassword muszą być identyczne

2. **Bezpieczeństwo haseł:**
   - Hasła są przechowywane w postaci hashowanej, nie w formie jawnego tekstu
   - ASP.NET Core Identity używa zaawansowanych algorytmów hashowania z solą

3. **Zabezpieczenie przed przejęciem konta:**
   - Wymagana jest weryfikacja adresu e-mail przed aktywacją konta
   - Token weryfikacyjny jest unikalny, zabezpieczony kryptograficznie i czasowo ograniczony

4. **Ochrona przed atakami:**
   - Zastosowanie rate-limitingu dla endpointu rejestracji w celu ochrony przed atakami typu brute-force

## 7. Obsługa błędów

| Scenariusz błędu                         | Kod statusu | Komunikat                                                 |
|------------------------------------------|-------------|----------------------------------------------------------|
| Niepoprawny format adresu e-mail         | 400         | "Nieprawidłowy format adresu e-mail"                     |
| Hasło zbyt słabe                         | 400         | "Hasło musi zawierać min. 8 znaków, 1 wielką literę, 1 małą literę i 1 cyfrę" |
| Hasła nie pasują do siebie               | 400         | "Hasła nie są identyczne"                                |
| E-mail już istnieje w systemie           | 409         | "Użytkownik o podanym adresie e-mail już istnieje"       |
| Błąd wysyłania e-maila                   | 500         | "Wystąpił problem z wysłaniem e-maila. Spróbuj ponownie." |
| Nieobsługiwany wyjątek                   | 500         | "Wystąpił nieoczekiwany błąd podczas rejestracji"        |

## 8. Rozważania dotyczące wydajności

1. **Wysyłanie e-maili:**
   - Wysyłanie e-maili powinno być wykonywane asynchronicznie, aby nie blokować odpowiedzi API
   - Można rozważyć kolejkowanie e-maili za pomocą background service lub zewnętrznego systemu kolejkowego

2. **Walidacja złożoności hasła:**
   - Operacja walidacji hasła może być kosztowna obliczeniowo, szczególnie przy bardziej złożonych regułach
   - Zaleca się wykorzystanie wbudowanych mechanizmów ASP.NET Core Identity, które są zoptymalizowane

3. **Kontrola duplikatów e-maili:**
   - Indeks na kolumnie NormalizedEmail w tabeli AspNetUsers poprawi wydajność sprawdzania duplikatów

## 9. Etapy wdrożenia

1. **Przygotowanie modelu i konfiguracji Identity:**
   - Utwórz model ApplicationUser dziedziczący po IdentityUser
   ```csharp
   public class ApplicationUser : IdentityUser<Guid>
   {
       // Dodatkowe pola specyficzne dla aplikacji, jeśli potrzebne
   }
   ```
   - Dodaj DbContext dla Identity
   ```csharp
   public class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
   {
       public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
           : base(options)
       {
       }
       
       protected override void OnModelCreating(ModelBuilder builder)
       {
           base.OnModelCreating(builder);
           // Dodatkowe konfiguracje, jeśli potrzebne
       }
   }
   ```
   - Dodaj konfigurację w klasie Program.cs lub Startup.cs
   ```csharp
   // Dodaj usługi Identity
   services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => {
       // Konfiguracja walidacji hasła
       options.Password.RequiredLength = 8;
       options.Password.RequireNonAlphanumeric = false;
       options.Password.RequireUppercase = true;
       options.Password.RequireLowercase = true;
       options.Password.RequireDigit = true;
       
       // Konfiguracja potwierdzenia e-mail
       options.SignIn.RequireConfirmedEmail = true;
   })
   .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
   .AddDefaultTokenProviders();
   ```

2. **Implementacja warstwy aplikacji:**
   - Zdefiniuj DTO: RegisterRequest, ApiResponse
   - Zaimplementuj command RegisterUserCommand i handler
   - Dodaj walidatory Fluent Validation dla RegisterUserCommand

3. **Implementacja warstwy infrastruktury:**
   - Skonfiguruj SendGrid do wysyłania e-maili
   - Utwórz EmailService do obsługi szablonów e-maili i łączności z SendGrid

4. **Implementacja kontrolera API:**
   - Utwórz AuthController z metodą Register
   - Dodaj atrybuty routingu i walidacji
   - Zaimplementuj obsługę błędów i logowanie

5. **Migracje bazy danych:**
   - Utwórz i zastosuj migracje dla tabel Identity (AspNetUsers, AspNetRoles, itd.)
   ```bash
   dotnet ef migrations add AddIdentitySchema -c ApplicationIdentityDbContext
   dotnet ef database update -c ApplicationIdentityDbContext
   ```

6. **Testy:**
   - Napisz testy jednostkowe dla walidacji
   - Napisz testy integracyjne dla kontrolera
   - Przetestuj przypadki brzegowe (np. e-mail już istnieje)

7. **Dokumentacja:**
   - Zaktualizuj dokumentację API (np. Swagger)
   - Dodaj komentarze XML do kontrolera i DTO dla dokumentacji 