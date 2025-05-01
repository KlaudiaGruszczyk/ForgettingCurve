# Plan implementacji endpointu API: Tworzenie zakresu

## 1. Przegląd punktu końcowego
Endpoint umożliwia zalogowanym użytkownikom tworzenie nowych zakresów (scopes), w których będą grupowane powiązane tematy do nauki. Endpoint tworzy nowy rekord w tabeli Scopes i zwraca szczegóły utworzonego zakresu.

## 2. Szczegóły żądania
- Metoda HTTP: POST
- Struktura URL: `/api/scopes`
- Parametry:
  - Wymagane: Brak parametrów URL
  - Opcjonalne: Brak
- Nagłówki:
  - Authorization: Bearer {token} - token JWT uwierzytelniający użytkownika
- Request Body:
  ```json
  {
    "name": "string"
  }
  ```

## 3. Wykorzystywane typy

### DTO
```csharp
namespace ForgettingCurve.Application.Requests 
{
    public class CreateScopeRequest
    {
        public string Name { get; set; }
    }
}

namespace ForgettingCurve.Application.Responses
{
    public class ScopeResponse
    {
        public Guid ScopeId { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
```

### Command
```csharp
namespace ForgettingCurve.Application.Commands.Scope
{
    public class CreateScopeCommand : IRequest<ScopeResponse>
    {
        public string Name { get; set; }
    }
}
```

### CommandHandler
```csharp
namespace ForgettingCurve.Application.Commands.Scope
{
    public class CreateScopeCommandHandler : IRequestHandler<CreateScopeCommand, ScopeResponse>
    {
        // Implementacja
    }
}
```

### Validator
```csharp
namespace ForgettingCurve.Application.Commands.Scope
{
    public class CreateScopeCommandValidator : AbstractValidator<CreateScopeCommand>
    {
        // Implementacja
    }
}
```

## 4. Szczegóły odpowiedzi
- Kod statusu: 201 Created
- Nagłówki:
  - Location: URL nowo utworzonego zasobu
- Struktura odpowiedzi:
  ```json
  {
    "scopeId": "guid",
    "name": "string",
    "creationDate": "2023-01-01T00:00:00Z"
  }
  ```

## 5. Przepływ danych
1. Kontroler otrzymuje żądanie POST z danymi zakresu
2. Kontroler przekształca żądanie na CreateScopeCommand, dodając identyfikator użytkownika z tokenu JWT
3. Command jest wysyłany przez mediatora do CommandHandlera
4. CommandHandler:
   - Waliduje command używając CreateScopeCommandValidator
   - Tworzy nowy obiekt domeny Scope
   - Zapisuje obiekt w repozytorium
   - Zwraca odpowiedź z danymi utworzonego zakresu
5. Kontroler zwraca odpowiedź HTTP 201 Created z lokalizacją nowo utworzonego zasobu

## 6. Względy bezpieczeństwa
- Endpoint wymaga uwierzytelnienia za pomocą tokenu JWT
- Należy zaimplementować walidację:
  - Długość nazwy (maksymalnie 150 znaków)
  - Nazwa nie może być pusta
  - Nazwa nie może zawierać niebezpiecznych znaków (XSS)
- UserId powinien być pobierany z tokenu JWT, a nie z żądania
- Rejestrowanie wszystkich prób utworzenia zakresu z informacją o użytkowniku

## 7. Obsługa błędów
- 400 Bad Request
  - Pusta nazwa
  - Nazwa przekracza 150 znaków
  - Nieprawidłowy format danych
- 401 Unauthorized
  - Brak tokenu JWT
  - Nieważny token JWT
- 500 Internal Server Error
  - Błąd bazy danych
  - Nieoczekiwany wyjątek

## 8. Rozważania dotyczące wydajności
- Unikanie zbędnych operacji bazodanowych
- Właściwe wykorzystanie transakcji
- Optymalizacja zapytań i indeksów bazy danych
- Cache'owanie rezultatów często używanych operacji

## 9. Etapy wdrożenia

### 1. Implementacja modelu domeny
```csharp
// Domain/Entities/Scope.cs
public class Scope
{
    public Guid ScopeId { get; private set; }
    public Guid OwnerUserId { get; private set; }
    public string Name { get; private set; }
    public DateTime CreationDate { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }

    private Scope() { }

    public Scope(Guid ownerId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Scope name cannot be empty");

        if (name.Length > 150)
            throw new DomainException("Scope name cannot exceed 150 characters");

        ScopeId = Guid.NewGuid();
        OwnerUserId = ownerId;
        Name = name;
        CreationDate = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Scope name cannot be empty");

        if (name.Length > 150)
            throw new DomainException("Scope name cannot exceed 150 characters");

        Name = name;
        LastModifiedDate = DateTime.UtcNow;
    }
}
```

### 2. Implementacja interface'u repozytorium
```csharp
// Domain/Interfaces/IScopeRepository.cs
public interface IScopeRepository
{
    Task<Scope> GetByIdAsync(Guid scopeId, Guid userId);
    Task<IEnumerable<Scope>> GetAllByUserIdAsync(Guid userId, string sortBy = "creationDate", int? skip = null, int? take = null);
    Task<Scope> AddAsync(Scope scope);
    Task<Scope> UpdateAsync(Scope scope);
    Task DeleteAsync(Guid scopeId, Guid userId);
    Task<int> GetCountByUserIdAsync(Guid userId);
}
```

### 3. Implementacja Command i CommandHandler
```csharp
// Application/Commands/Scope/CreateScope/CreateScopeCommand.cs
public class CreateScopeCommand : IRequest<ScopeResponse>
{
    public string Name { get; set; }
    public Guid UserId { get; set; }
}

// Application/Commands/Scope/CreateScope/CreateScopeCommandHandler.cs
public class CreateScopeCommandHandler : IRequestHandler<CreateScopeCommand, ScopeResponse>
{
    private readonly IScopeRepository _scopeRepository;

    public CreateScopeCommandHandler(IScopeRepository scopeRepository)
    {
        _scopeRepository = scopeRepository;
    }

    public async Task<ScopeResponse> Handle(CreateScopeCommand request, CancellationToken cancellationToken)
    {
        var scope = new Scope(request.UserId, request.Name);
        
        var createdScope = await _scopeRepository.AddAsync(scope);
        
        return new ScopeResponse
        {
            ScopeId = createdScope.ScopeId,
            Name = createdScope.Name,
            CreationDate = createdScope.CreationDate
        };
    }
}
```

### 4. Implementacja walidatora
```csharp
// Application/Commands/Scope/CreateScope/CreateScopeCommandValidator.cs
public class CreateScopeCommandValidator : AbstractValidator<CreateScopeCommand>
{
    public CreateScopeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Scope name is required")
            .MaximumLength(150).WithMessage("Scope name cannot exceed 150 characters");
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}
```

### 5. Implementacja repozytorium
```csharp
// Infrastructure/Persistence/Repositories/ScopeRepository.cs
public class ScopeRepository : IScopeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ScopeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Scope> AddAsync(Scope scope)
    {
        await _dbContext.Scopes.AddAsync(scope);
        await _dbContext.SaveChangesAsync();
        return scope;
    }

    // Implementacja pozostałych metod
}
```

### 6. Implementacja kontrolera
```csharp
// Api/Controllers/ScopesController.cs
[ApiController]
[Route("api/scopes")]
[Authorize]
public class ScopesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ScopesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ScopeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateScope([FromBody] CreateScopeRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var command = new CreateScopeCommand
        {
            Name = request.Name,
            UserId = Guid.Parse(userId)
        };
        
        var result = await _mediator.Send(command);
        
        return CreatedAtAction(nameof(GetScope), new { scopeId = result.ScopeId }, result);
    }

    // Pozostałe metody kontrolera
}
```

### 7. Rejestracja zależności
```csharp
// Program.cs lub Startup.cs
services.AddScoped<IScopeRepository, ScopeRepository>();
services.AddMediatR(typeof(CreateScopeCommand).Assembly);
services.AddValidatorsFromAssembly(typeof(CreateScopeCommandValidator).Assembly);
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```