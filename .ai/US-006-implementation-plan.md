# Plan implementacji: Edycja nazwy Zakresu (US-006)

## 1. Przegląd punktu końcowego
Endpoint API umożliwiający zalogowanemu użytkownikowi aktualizację nazwy istniejącego Zakresu (Scope). Zakres musi należeć do użytkownika wykonującego żądanie. Implementacja zgodna z historyjką użytkownika US-006 z dokumentu wymagań produktu.

## 2. Szczegóły żądania
- Metoda HTTP: PUT
- Struktura URL: `/api/scopes/{scopeId}`
- Parametry:
  - Wymagane: 
    - `scopeId` (Guid) - identyfikator zakresu do aktualizacji
- Request Body: 
```json
{
  "name": "string"
}
```
- Nagłówki:
  - `Authorization: Bearer {token}` - JWT token uzyskany podczas logowania

## 3. Wymagane typy
### Data Transfer Objects (DTO)
```csharp
namespace ForgettingCurve.Application.Requests
{
    public class UpdateScopeRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}

namespace ForgettingCurve.Application.Responses
{
    public class ScopeResponse
    {
        public Guid ScopeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? LastModifiedDate { get; set; }
    }
}
```

### Commands
```csharp
namespace ForgettingCurve.Application.Commands.Scopes
{
    public class UpdateScopeCommand : IRequest<ScopeResponse>
    {
        public Guid ScopeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid UserId { get; set; } // Do weryfikacji własności
    }
}
```

### Validator
```csharp
namespace ForgettingCurve.Application.Commands.Scopes
{
    public class UpdateScopeCommandValidator : AbstractValidator<UpdateScopeCommand>
    {
        public UpdateScopeCommandValidator()
        {
            RuleFor(x => x.ScopeId)
                .NotEmpty().WithMessage("ScopeId is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(150).WithMessage("Name cannot exceed 150 characters");
                
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
        }
    }
}
```

## 4. Szczegóły odpowiedzi
- Kod statusu: 
  - 200 OK - pomyślna aktualizacja
  - 400 Bad Request - nieprawidłowe dane
  - 401 Unauthorized - brak autoryzacji
  - 403 Forbidden - brak uprawnień
  - 404 Not Found - zasób nie istnieje
- Struktura odpowiedzi (200 OK):
```json
{
  "scopeId": "guid",
  "name": "string",
  "lastModifiedDate": "2023-01-01T00:00:00Z"
}
```

## 5. Przepływ danych
1. Kontroler odbiera żądanie PUT z parametrem `scopeId` i body zawierającym nową nazwę.
2. Kontroler mapuje żądanie na `UpdateScopeCommand` wraz z ID użytkownika z tokenu JWT.
3. Command jest wysyłany do odpowiedniego CommandHandlera.
4. CommandHandler waliduje dane wejściowe używając FluentValidation.
5. CommandHandler sprawdza, czy zakres istnieje i czy należy do użytkownika.
6. CommandHandler aktualizuje nazwę zakresu i ustawia `LastModifiedDate` na aktualny czas UTC.
7. Zmiany są zapisywane w bazie danych poprzez `UnitOfWork`.
8. CommandHandler mapuje zaktualizowany zakres na `ScopeResponse` i zwraca do kontrolera.
9. Kontroler zwraca odpowiedź HTTP 200 OK z zaktualizowanymi danymi zakresu.

## 6. Względy bezpieczeństwa
1. **Uwierzytelnianie**: Wymagane uwierzytelnienie JWT poprzez atrybut `[Authorize]`.
2. **Autoryzacja**: Sprawdzenie czy użytkownik jest właścicielem zakresu poprzez:
   - Atrybut `[RequiresResourceOwnership]` na poziomie metody kontrolera
   - Dodatkowe sprawdzenie w CommandHandlerze poprzez `IScopeRepository.IsOwnerAsync()`
3. **Walidacja danych wejściowych**:
   - Sprawdzenie, czy nazwa nie jest pusta
   - Sprawdzenie, czy nazwa nie przekracza 150 znaków
4. **Ochrona przed atakami CSRF/XSRF**:
   - Wykorzystanie mechanizmów Anti-Forgery wbudowanych w ASP.NET Core
5. **Rate Limiting**:
   - Rozważenie implementacji limitu żądań dla zapobiegania atakom DoS

## 7. Obsługa błędów
- **400 Bad Request**:
  - Brak nazwy w żądaniu
  - Nazwa przekracza 150 znaków
  - ID w ścieżce URL nie zgadza się z ID w żądaniu (jeśli dotyczy)
- **401 Unauthorized**:
  - Brak tokenu JWT
  - Niewłaściwy token JWT
- **403 Forbidden**:
  - Użytkownik nie jest właścicielem zakresu
- **404 Not Found**:
  - Zakres o podanym ID nie istnieje
- **500 Internal Server Error**:
  - Nieoczekiwane błędy serwera (logowanie do systemu logowania)

## 8. Rozważania dotyczące wydajności
1. **Operacje bazodanowe**:
   - Używanie indeksów dla szybkiego wyszukiwania po ID zakresu i ID użytkownika
   - Efektywne wykorzystanie UnitOfWork dla minimalizacji operacji na bazie danych
2. **Caching**:
   - W przyszłości rozważenie cachowania często używanych zakresów dla szybszego dostępu
3. **Walidacja**:
   - Szybkie zwracanie błędów dla nieprawidłowych danych bez niepotrzebnych operacji bazodanowych

## 9. Etapy wdrożenia
1. **Stworzenie DTO**:
   - Zdefiniowanie `UpdateScopeRequest` i `ScopeResponse`
   
2. **Implementacja Command i CommandHandler**:
   - Stworzenie `UpdateScopeCommand` w `ForgettingCurve.Application.Commands.Scopes`
   - Implementacja `UpdateScopeCommandHandler`
   - Dodanie walidatora `UpdateScopeCommandValidator`
   
3. **Rozszerzenie kontrolera**:
   - Modyfikacja istniejącej metody PUT w `ScopesController`
   - Dodanie poprawnego mapowania modeli
   - Implementacja zwracania odpowiednich kodów odpowiedzi HTTP
   
4. **Dokumentacja**:
   - Aktualizacja dokumentacji API (Swagger)
   - Dodanie komentarzy XML do kodu dla automatycznej dokumentacji 