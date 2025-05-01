# Plan implementacji API Endpoint: Zabezpieczenie dostępu (US-003)

## 1. Przegląd punktu końcowego

Zabezpieczenie dostępu nie jest konkretnym endpointem API, lecz zestawem mechanizmów, które muszą być zaimplementowane globalnie dla całej aplikacji. Mechanizm ten zapewnia, że zalogowany użytkownik ma dostęp wyłącznie do własnych danych (Zakresów, Tematów i Powtórek), a niezalogowani użytkownicy nie mają dostępu do funkcjonalności zarządzania danymi. 

Funkcjonalność ta stanowi podstawę bezpieczeństwa aplikacji i zgodnie z US-003 ma ona zagwarantować, że:
1. Dane utworzone przez jednego użytkownika nie są widoczne dla innych użytkowników
2. Dostęp do funkcjonalności zarządzania danymi wymaga bycia zalogowanym

## 2. Szczegóły implementacji uwierzytelniania

### Globalne zabezpieczenie API
- Metody uwierzytelniania: JWT (JSON Web Token)
- Czas życia tokenu: 1 godzina (3600 sekund)
- Sposób przekazywania tokenu: Nagłówek HTTP `Authorization: Bearer {token}`
- Wymagane w nagłówku dla wszystkich endpointów API poza `/api/auth/register` i `/api/auth/login`

### Implementacja autoryzacji
- Weryfikacja na poziomie controllera: wszystkie kontrolery (poza AuthController) powinny być oznaczone atrybutem `[Authorize]` 
- Weryfikacja na poziomie zasobów: każde żądanie do zasobu powinno sprawdzać czy użytkownik jest właścicielem zasobu przez porównanie `OwnerUserId` z ID zalogowanego użytkownika

## 3. Wykorzystywane typy

### Filter/Middleware
```csharp
// Filtr autoryzacji zasobów
public class ResourceOwnershipFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Logika weryfikacji właściciela zasobu
    }
}

// Middleware autoryzacji
public class ResourceOwnershipMiddleware
{
    private readonly RequestDelegate _next;

    public ResourceOwnershipMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        // Logika weryfikacji właściciela zasobu
    }
}
```

### Atrybuty
```csharp
// Atrybut do oznaczania kontrolerów/metod wymagających weryfikacji właściciela zasobu
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiresResourceOwnershipAttribute : Attribute
{
    public string ResourceIdParameter { get; }
    public Type ResourceType { get; }

    public RequiresResourceOwnershipAttribute(string resourceIdParameter, Type resourceType)
    {
        ResourceIdParameter = resourceIdParameter;
        ResourceType = resourceType;
    }
}
```

### Extensions
```csharp
// Metody rozszerzające do weryfikacji właściciela zasobu
public static class ResourceOwnershipExtensions
{
    public static bool IsResourceOwner(this ClaimsPrincipal user, Guid ownerId)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId != null && ownerId.ToString() == userId;
    }
}
```

## 4. Przepływ danych

1. **Rejestracja i uwierzytelnianie:**
   - Użytkownik rejestruje się w systemie (nie wymaga uwierzytelnienia)
   - Użytkownik weryfikuje email (nie wymaga uwierzytelnienia)
   - Użytkownik loguje się i otrzymuje token JWT (nie wymaga uwierzytelnienia)
   - Token JWT zawiera ClaimTypes.NameIdentifier z ID użytkownika

2. **Weryfikacja dostępu:**
   - Każde żądanie do API (poza auth) zawiera token JWT w nagłówku
   - Middleware JWT weryfikuje token i ustawia ClaimsPrincipal w HttpContext
   - Atrybut [Authorize] weryfikuje obecność uwierzytelnionego użytkownika
   - Middleware/Filter ResourceOwnership weryfikuje właściciela zasobu

3. **Weryfikacja właściciela zasobu:**
   - Pobierz ID zasobu z parametrów żądania (route, query, body)
   - Pobierz zasób z repozytorium
   - Porównaj OwnerUserId zasobu z ID zalogowanego użytkownika
   - Jeśli nie są zgodne, zwróć błąd 403 Forbidden

## 5. Względy bezpieczeństwa

### Uwierzytelnianie
- Token JWT musi być podpisany bezpiecznym kluczem
- Klucz JWT powinien być przechowywany w bezpiecznym miejscu (np. zmienne środowiskowe, Azure Key Vault)
- Token powinien mieć ograniczony czas życia (1 godzina)
- Token powinien zawierać tylko niezbędne informacje (ID użytkownika, role)

### Autoryzacja
- Weryfikacja właściciela zasobu musi być wykonywana dla każdego żądania
- Weryfikacja powinna być przeprowadzana tak wcześnie jak to możliwe w pipeline'ie przetwarzania żądania
- Należy unikać ujawniania informacji o istnieniu zasobów (404 Not Found zamiast 403 Forbidden dla nieistniejących zasobów)

### HTTPS
- Cała komunikacja z API powinna odbywać się przez HTTPS
- Należy stosować przekierowanie HTTP->HTTPS
- Należy rozważyć użycie HSTS (HTTP Strict Transport Security)

## 6. Obsługa błędów

| Kod statusu | Znaczenie | Przykład scenariusza |
|-------------|-----------|----------------------|
| 401 Unauthorized | Brak uwierzytelnienia | Brak tokenu JWT lub nieważny token |
| 403 Forbidden | Brak autoryzacji | Użytkownik próbuje uzyskać dostęp do zasobu, którego nie jest właścicielem |
| 404 Not Found | Zasób nie znaleziony | Zasób nie istnieje lub użytkownik nie jest jego właścicielem (ukrywanie zasobów) |

## 7. Rozważania dotyczące wydajności

- **Warstwa cache:** Rozważyć implementację warstwy cache dla często używanych zasobów (z uwzględnieniem bezpieczeństwa)
- **Opóźniona weryfikacja:** Dla złożonych żądań można rozważyć weryfikację właściciela zasobu dopiero po wstępnej walidacji innych aspektów
- **Optymalizacja zapytań:** Upewnić się, że zapytania do bazy danych filtrują wyniki według OwnerUserId już na poziomie SQL

## 8. Etapy wdrożenia

1. **Konfiguracja uwierzytelniania JWT:**
   - Dodanie konfiguracji JWT (klucz, wystawca, odbiorcy, czas życia) do appsettings.json
   - Konfiguracja JWT w Program.cs (AddAuthentication, AddJwtBearer)
   - Implementacja middleware JWT w pipeline'ie (UseAuthentication, UseAuthorization)

2. **Implementacja autoryzacji zasobów:**
   - Utworzenie ResourceOwnershipFilter lub Middleware
   - Dodanie filtra/middleware do pipeline'u przetwarzania żądań
   - Utworzenie atrybutu RequiresResourceOwnership

3. **Zabezpieczenie kontrolerów:**
   - Oznaczenie wszystkich kontrolerów poza AuthController atrybutem [Authorize]
   - Oznaczenie odpowiednich metod atrybutem [RequiresResourceOwnership]

4. **Modyfikacja repozytoriów:**
   - Modyfikacja metod repozytoriów, aby zawsze filtrowały wyniki według OwnerUserId
   - Implementacja mechanizmu wstrzykiwania ID zalogowanego użytkownika do repozytoriów

5. **Implementacja obsługi błędów:**
   - Implementacja middleware obsługującego błędy autoryzacji (401, 403)
   - Zapewnienie odpowiednich komunikatów błędów

7. **Dokumentacja:**
   - Aktualizacja dokumentacji API o wymogi uwierzytelnienia i autoryzacji
   - Dokumentacja wewnętrzna dla developerów dotycząca implementacji zabezpieczeń 