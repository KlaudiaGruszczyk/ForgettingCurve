# Plan Wdrożenia Endpointu API: Usuwanie Zakresu

## 1. Przegląd punktu końcowego
Endpoint służy do usuwania istniejącego Zakresu (Scope) oraz wszystkich powiązanych z nim Tematów (Topics). Zgodnie z US-007, użytkownik powinien mieć możliwość usunięcia Zakresu, którego już nie potrzebuje. Usunięcie jest operacją kaskadową, która powoduje również usunięcie wszystkich powiązanych Tematów i ich Powtórek.

## 2. Szczegóły żądania
- **Metoda HTTP**: DELETE
- **Struktura URL**: `/api/scopes/{scopeId}`
- **Parametry**:
  - **Wymagane**: `scopeId` (GUID, parametr ścieżki)
  - **Opcjonalne**: brak
- **Request Body**: brak

## 3. Wykorzystywane typy

### Command
```csharp
namespace ForgettingCurve.Application.Commands.DeleteScope
{
    public class DeleteScopeCommand : IRequest<DeleteScopeResponse>
    {
        public Guid ScopeId { get; set; }
    }
}
```

### Response
```csharp
namespace ForgettingCurve.Application.Responses
{
    public class DeleteScopeResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
```

### Validator
```csharp
namespace ForgettingCurve.Application.Commands.DeleteScope
{
    public class DeleteScopeCommandValidator : AbstractValidator<DeleteScopeCommand>
    {
        public DeleteScopeCommandValidator()
        {
            RuleFor(x => x.ScopeId)
                .NotEmpty().WithMessage("ScopeId is required.");
        }
    }
}
```

## 4. Szczegóły odpowiedzi
- **Format odpowiedzi**: JSON
- **Struktura odpowiedzi**:
  ```json
  {
    "success": true,
    "message": "Zakres usunięty pomyślnie."
  }
  ```
- **Kody odpowiedzi**:
  - 200 OK: Zakres został pomyślnie usunięty
  - 400 Bad Request: Nieprawidłowe dane (np. niepoprawny format scopeId)
  - 401 Unauthorized: Użytkownik nie jest zalogowany
  - 403 Forbidden: Użytkownik nie ma dostępu do tego zakresu
  - 404 Not Found: Zakres o podanym ID nie istnieje
  - 500 Internal Server Error: Nieoczekiwany błąd serwera

## 5. Przepływ danych
1. Kontroler otrzymuje żądanie DELETE z identyfikatorem zakresu (scopeId).
2. Kontroler tworzy instancję DeleteScopeCommand i przekazuje ją do mediatora.
3. CommandHandler pobiera zakres z repozytorium i sprawdza, czy zakres istnieje.
4. CommandHandler weryfikuje, czy zalogowany użytkownik jest właścicielem zakresu.
5. CommandHandler zleca usunięcie zakresu poprzez repozytorium (usunięcie to kaskadowo usunie również powiązane tematy i powtórki, zgodnie z ustawieniami bazy danych).
6. Repozytorium wykonuje operację usunięcia w bazie danych.
7. CommandHandler zwraca odpowiedź z informacją o sukcesie.
8. Kontroler zwraca odpowiednią odpowiedź HTTP.

## 6. Względy bezpieczeństwa
- **Uwierzytelnianie**: Endpoint musi być zabezpieczony poprzez uwierzytelnianie - tylko zalogowani użytkownicy mogą usuwać zakresy.
- **Autoryzacja**: Przed usunięciem należy sprawdzić, czy zalogowany użytkownik jest właścicielem zakresu (porównanie OwnerUserId z Id aktualnie zalogowanego użytkownika).
- **Walidacja danych**: Sprawdzenie poprawności scopeId jako GUID.
- **Zabezpieczenia przed CSRF**: Wykorzystanie tokenów anty-CSRF dla żądań DELETE.

## 7. Obsługa błędów
- **Nieprawidłowy format ScopeId**: Zwróć 400 Bad Request z odpowiednim komunikatem.
- **Niezalogowany użytkownik**: Zwróć 401 Unauthorized.
- **Próba usunięcia zakresu należącego do innego użytkownika**: Zwróć 403 Forbidden.
- **Zakres nie istnieje**: Zwróć 404 Not Found z komunikatem "Zakres o podanym identyfikatorze nie istnieje".
- **Błąd bazy danych**: Zaloguj szczegóły błędu, zwróć użytkownikowi 500 Internal Server Error.

## 8. Rozważania dotyczące wydajności
- **Usuwanie kaskadowe**: Upewnij się, że baza danych ma skonfigurowane odpowiednie indeksy i klauzule ON DELETE CASCADE dla powiązanych tabel.
- **Transakcyjność**: Użyj transakcji bazodanowej do zapewnienia, że wszystkie powiązane tematy i powtórki zostaną usunięte atomowo.

## 9. Etapy wdrożenia

1. **Utworzenie Command i Response**:
   - Utwórz klasę `DeleteScopeCommand` w katalogu `ForgettingCurve.Application.Commands.DeleteScope`
   - Utwórz klasę `DeleteScopeResponse` w katalogu `ForgettingCurve.Application.Responses`

2. **Implementacja Validatora**:
   - Utwórz klasę `DeleteScopeCommandValidator` implementującą reguły walidacji

3. **Implementacja CommandHandlera**:
   ```csharp
   namespace ForgettingCurve.Application.Commands.DeleteScope
   {
       public class DeleteScopeCommandHandler : IRequestHandler<DeleteScopeCommand, DeleteScopeResponse>
       {
           private readonly IScopeRepository _scopeRepository;
           private readonly ICurrentUserService _currentUserService;
           private readonly IUnitOfWork _unitOfWork;

           public DeleteScopeCommandHandler(
               IScopeRepository scopeRepository,
               ICurrentUserService currentUserService,
               IUnitOfWork unitOfWork)
           {
               _scopeRepository = scopeRepository;
               _currentUserService = currentUserService;
               _unitOfWork = unitOfWork;
           }

           public async Task<DeleteScopeResponse> Handle(DeleteScopeCommand request, CancellationToken cancellationToken)
           {
               var scope = await _scopeRepository.GetByIdAsync(request.ScopeId, cancellationToken);
               if (scope == null)
               {
                   throw new NotFoundException("Scope", request.ScopeId);
               }

               var currentUserId = _currentUserService.GetUserId();
               if (scope.OwnerUserId != currentUserId)
               {
                   throw new ForbiddenAccessException("You don't have permission to delete this scope");
               }

               _scopeRepository.Delete(scope);
               await _unitOfWork.SaveChangesAsync(cancellationToken);

               return new DeleteScopeResponse
               {
                   Success = true,
                   Message = "Zakres usunięty pomyślnie."
               };
           }
       }
   }
   ```

4. **Implementacja endpointu w kontrolerze**:
   ```csharp
   [HttpDelete("{scopeId}")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status401Unauthorized)]
   [ProducesResponseType(StatusCodes.Status403Forbidden)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   [ProducesResponseType(StatusCodes.Status500InternalServerError)]
   public async Task<ActionResult<DeleteScopeResponse>> DeleteScope(Guid scopeId)
   {
       try
       {
           var command = new DeleteScopeCommand { ScopeId = scopeId };
           var result = await _mediator.Send(command);
           return Ok(result);
       }
       catch (ValidationException ex)
       {
           return BadRequest(ex.Errors);
       }
       catch (NotFoundException ex)
       {
           return NotFound(new { message = ex.Message });
       }
       catch (ForbiddenAccessException ex)
       {
           return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error deleting scope with ID {ScopeId}", scopeId);
           return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while processing your request." });
       }
   }
   ```

5. **Rejestracja zależności**:
   - Zarejestruj `DeleteScopeCommandHandler` i `DeleteScopeCommandValidator` w kontenerze DI

6. **Dokumentacja**:
   - Zaktualizuj dokumentację API (Swagger/OpenAPI) z informacjami o nowym endpoincie 