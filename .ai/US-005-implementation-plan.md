# Plan implementacji endpointu API: Pobieranie wszystkich zakresów

## 1. Przegląd punktu końcowego
Endpoint umożliwia zalogowanym użytkownikom pobieranie listy wszystkich swoich zakresów (scopes) z możliwością sortowania i paginacji. Endpoint zwraca tylko zakresy należące do zalogowanego użytkownika.

## 2. Szczegóły żądania
- Metoda HTTP: GET
- Struktura URL: `/api/scopes`
- Parametry:
  - Wymagane: Brak parametrów wymaganych
  - Opcjonalne:
    - `sortBy` (string): Pole do sortowania. Dozwolone wartości: "creationDate" (domyślnie), "name", "nextRepetition"
    - `skip` (int): Liczba elementów do pominięcia (do paginacji)
    - `take` (int): Liczba elementów do pobrania (do paginacji)
- Nagłówki:
  - Authorization: Bearer {token} - token JWT uwierzytelniający użytkownika

## 3. Wykorzystywane typy

### DTO
```csharp
namespace ForgettingCurve.Application.Responses
{
    public class ScopeResponse
    {
        public Guid ScopeId { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? NextRepetitionDate { get; set; }
    }

    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
```

### Query
```csharp
namespace ForgettingCurve.Application.Queries.Scope
{
    public class GetScopesQuery : IRequest<PagedResponse<ScopeResponse>>
    {
        public string SortBy { get; set; } = "creationDate";
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public Guid UserId { get; set; }
    }
}
```

### QueryHandler
```csharp
namespace ForgettingCurve.Application.Queries.Scope
{
    public class GetScopesQueryHandler : IRequestHandler<GetScopesQuery, PagedResponse<ScopeResponse>>
    {
        // Implementacja
    }
}
```

### Validator
```csharp
namespace ForgettingCurve.Application.Queries.Scope
{
    public class GetScopesQueryValidator : AbstractValidator<GetScopesQuery>
    {
        // Implementacja
    }
}
```

## 4. Szczegóły odpowiedzi
- Kod statusu: 200 OK
- Struktura odpowiedzi:
  ```json
  {
    "items": [
      {
        "scopeId": "guid",
        "name": "string",
        "creationDate": "2023-01-01T00:00:00Z",
        "lastModifiedDate": "2023-01-01T00:00:00Z",
        "nextRepetitionDate": "2023-01-01T00:00:00Z"
      }
    ],
    "totalCount": 10
  }
  ```

## 5. Przepływ danych
1. Kontroler otrzymuje żądanie GET z opcjonalnymi parametrami sortBy, skip, take
2. Kontroler pobiera identyfikator użytkownika z tokenu JWT
3. Kontroler tworzy i wysyła zapytanie GetScopesQuery do mediatora
4. GetScopesQueryHandler:
   - Waliduje zapytanie za pomocą GetScopesQueryValidator
   - Pobiera zakresy z repozytorium z uwzględnieniem:
     - Filtrowania po UserId (tylko zakresy zalogowanego użytkownika)
     - Sortowania (creationDate, name lub nextRepetition)
     - Paginacji (skip, take)
   - Mapuje wyniki na obiekty ScopeResponse, uwzględniając obliczenie NextRepetitionDate
   - Zwraca paginowaną odpowiedź (PagedResponse<ScopeResponse>)
5. Kontroler zwraca odpowiedź HTTP 200 OK z danymi zakresów

## 6. Względy bezpieczeństwa
- Endpoint wymaga uwierzytelnienia za pomocą tokenu JWT
- Dane są filtrowane po identyfikatorze użytkownika (OwnerUserId) - użytkownik widzi tylko swoje zakresy
- Walidacja parametrów wejściowych zapobiega atakom typu injection
- UserId jest pobierany z tokenu JWT, a nie z parametrów żądania
- Logowanie wszystkich prób dostępu do zakresów z informacją o użytkowniku

## 7. Obsługa błędów
- 401 Unauthorized
  - Brak tokenu JWT
  - Nieważny token JWT
- 400 Bad Request
  - Nieprawidłowa wartość parametru sortBy (inne niż "creationDate", "name", "nextRepetition")
  - Nieprawidłowe wartości parametrów skip lub take (ujemne liczby)
- 500 Internal Server Error
  - Błąd bazy danych
  - Nieoczekiwany wyjątek

## 8. Rozważania dotyczące wydajności
- Zastosowanie paginacji zapobiega pobieraniu zbyt dużej ilości danych naraz
- Indeksy w bazie danych:
  - IX_Scopes_OwnerUserId - dla filtrowania po właścicielu
  - IX_Scopes_CreationDate - dla sortowania po dacie utworzenia
  - IX_Scopes_Name - dla sortowania po nazwie
- Wyliczanie NextRepetitionDate może być kosztowne - należy zoptymalizować zapytanie
- Możliwość cache'owania rezultatów dla tego samego użytkownika z tym samym zestawem parametrów

## 9. Etapy wdrożenia

### 1. Implementacja modeli odpowiedzi
```csharp
// Application/Responses/ScopeResponse.cs
namespace ForgettingCurve.Application.Responses
{
    public class ScopeResponse
    {
        public Guid ScopeId { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? NextRepetitionDate { get; set; }
    }
}

// Application/Responses/PagedResponse.cs
namespace ForgettingCurve.Application.Responses
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        
        public PagedResponse(IEnumerable<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
    }
}
```

### 2. Implementacja Query i QueryHandler
```csharp
// Application/Queries/Scope/GetScopes/GetScopesQuery.cs
namespace ForgettingCurve.Application.Queries.Scope
{
    public class GetScopesQuery : IRequest<PagedResponse<ScopeResponse>>
    {
        public string SortBy { get; set; } = "creationDate";
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public Guid UserId { get; set; }
    }
}

// Application/Queries/Scope/GetScopes/GetScopesQueryHandler.cs
namespace ForgettingCurve.Application.Queries.Scope
{
    public class GetScopesQueryHandler : IRequestHandler<GetScopesQuery, PagedResponse<ScopeResponse>>
    {
        private readonly IScopeRepository _scopeRepository;
        private readonly ITopicRepository _topicRepository;

        public GetScopesQueryHandler(IScopeRepository scopeRepository, ITopicRepository topicRepository)
        {
            _scopeRepository = scopeRepository;
            _topicRepository = topicRepository;
        }

        public async Task<PagedResponse<ScopeResponse>> Handle(GetScopesQuery request, CancellationToken cancellationToken)
        {
            // Pobieranie zakresów z paginacją
            var (scopes, totalCount) = await _scopeRepository.GetAllByUserIdAsync(
                request.UserId,
                request.SortBy,
                request.Skip,
                request.Take
            );

            // Mapowanie do ScopeResponse
            var scopeResponses = new List<ScopeResponse>();
            
            foreach (var scope in scopes)
            {
                // Dla sortowania po nextRepetition, informacja o następnej powtórce jest już pobrana
                DateTime? nextRepetitionDate = null;
                
                if (request.SortBy != "nextRepetition")
                {
                    // Znajdź najwcześniejszą datę powtórki dla tematu w tym zakresie
                    nextRepetitionDate = await _topicRepository.GetEarliestNextRepetitionDateByScopeIdAsync(scope.ScopeId);
                }
                else
                {
                    // Wykorzystaj już pobrane dane w przypadku sortowania po nextRepetition
                    nextRepetitionDate = scope.NextRepetitionDate;
                }

                scopeResponses.Add(new ScopeResponse
                {
                    ScopeId = scope.ScopeId,
                    Name = scope.Name,
                    CreationDate = scope.CreationDate,
                    LastModifiedDate = scope.LastModifiedDate,
                    NextRepetitionDate = nextRepetitionDate
                });
            }

            return new PagedResponse<ScopeResponse>(scopeResponses, totalCount);
        }
    }
}
```

### 3. Implementacja walidatora
```csharp
// Application/Queries/Scope/GetScopes/GetScopesQueryValidator.cs
namespace ForgettingCurve.Application.Queries.Scope
{
    public class GetScopesQueryValidator : AbstractValidator<GetScopesQuery>
    {
        public GetScopesQueryValidator()
        {
            RuleFor(x => x.SortBy)
                .Must(sortBy => sortBy == "creationDate" || sortBy == "name" || sortBy == "nextRepetition")
                .WithMessage("SortBy must be 'creationDate', 'name', or 'nextRepetition'");

            RuleFor(x => x.Skip)
                .GreaterThanOrEqualTo(0).When(x => x.Skip.HasValue)
                .WithMessage("Skip must be greater than or equal to 0");

            RuleFor(x => x.Take)
                .GreaterThanOrEqualTo(1).When(x => x.Take.HasValue)
                .WithMessage("Take must be greater than or equal to 1");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required");
        }
    }
}
```

### 4. Rozszerzenie interfejsu repozytorium
```csharp
// Domain/Interfaces/IScopeRepository.cs
public interface IScopeRepository
{
    // Istniejące metody...
    
    Task<(IEnumerable<Scope> scopes, int totalCount)> GetAllByUserIdAsync(
        Guid userId, 
        string sortBy = "creationDate", 
        int? skip = null, 
        int? take = null);
}

// Domain/Interfaces/ITopicRepository.cs
public interface ITopicRepository
{
    // Istniejące metody...
    
    Task<DateTime?> GetEarliestNextRepetitionDateByScopeIdAsync(Guid scopeId);
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

    public async Task<(IEnumerable<Scope> scopes, int totalCount)> GetAllByUserIdAsync(
        Guid userId, 
        string sortBy = "creationDate", 
        int? skip = null, 
        int? take = null)
    {
        // Zapytanie bazowe - filtorowanie po UserId
        IQueryable<Scope> query = _dbContext.Scopes.Where(s => s.OwnerUserId == userId);
        
        // Zliczanie całkowitej liczby przed zastosowaniem paginacji
        int totalCount = await query.CountAsync();
        
        // Stosowanie sortowania
        query = sortBy.ToLower() switch
        {
            "name" => query.OrderBy(s => s.Name),
            "nextrepetition" => query
                .GroupJoin(
                    _dbContext.Topics.Where(t => !t.IsMastered)
                        .GroupJoin(
                            _dbContext.Repetitions.Where(r => r.CompletedDate == null),
                            topic => topic.TopicId,
                            rep => rep.TopicId,
                            (topic, reps) => new { Topic = topic, NextRepDate = reps.Min(r => r.ScheduledDate) }
                        ),
                    scope => scope.ScopeId,
                    topicWithDate => topicWithDate.Topic.ScopeId,
                    (scope, topicDates) => new {
                        Scope = scope,
                        NextRepetitionDate = topicDates.Min(t => t.NextRepDate)
                    }
                )
                .OrderBy(x => x.NextRepetitionDate)
                .Select(x => x.Scope),
            _ => query.OrderByDescending(s => s.CreationDate) // domyślnie "creationDate"
        };
        
        // Stosowanie paginacji
        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }
        
        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }
        
        // Wykonanie zapytania
        var scopes = await query.ToListAsync();
        
        // W przypadku sortowania po nextRepetition, musimy przekazać również informacje o datach
        if (sortBy.ToLower() == "nextrepetition")
        {
            // Pobieramy daty następnych powtórek dla każdego zakresu
            var scopeIds = scopes.Select(s => s.ScopeId).ToList();
            var nextRepDates = await _dbContext.Topics
                .Where(t => scopeIds.Contains(t.ScopeId) && !t.IsMastered)
                .GroupJoin(
                    _dbContext.Repetitions.Where(r => r.CompletedDate == null),
                    topic => topic.TopicId,
                    rep => rep.TopicId,
                    (topic, reps) => new { Topic = topic, NextRepDate = reps.Min(r => r.ScheduledDate) }
                )
                .GroupBy(x => x.Topic.ScopeId)
                .Select(g => new { ScopeId = g.Key, NextRepDate = g.Min(x => x.NextRepDate) })
                .ToDictionaryAsync(x => x.ScopeId, x => x.NextRepDate);
            
            // Ustawiamy daty w obiektach Scope (dodajemy dynamiczną właściwość)
            foreach (var scope in scopes)
            {
                if (nextRepDates.TryGetValue(scope.ScopeId, out var date))
                {
                    // Używamy refleksji lub dynamicznych właściwości, zależnie od implementacji
                    ((dynamic)scope).NextRepetitionDate = date;
                }
            }
        }
        
        return (scopes, totalCount);
    }

    // Implementacja pozostałych metod...
}

// Infrastructure/Persistence/Repositories/TopicRepository.cs
public class TopicRepository : ITopicRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TopicRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DateTime?> GetEarliestNextRepetitionDateByScopeIdAsync(Guid scopeId)
    {
        return await _dbContext.Topics
            .Where(t => t.ScopeId == scopeId && !t.IsMastered)
            .GroupJoin(
                _dbContext.Repetitions.Where(r => r.CompletedDate == null),
                topic => topic.TopicId,
                rep => rep.TopicId,
                (topic, reps) => new { Topic = topic, MinScheduledDate = reps.Min(r => r.ScheduledDate) }
            )
            .Where(x => x.MinScheduledDate != null)
            .MinAsync(x => x.MinScheduledDate);
    }

    // Implementacja pozostałych metod...
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

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ScopeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetScopes(
        [FromQuery] string sortBy = "creationDate",
        [FromQuery] int? skip = null,
        [FromQuery] int? take = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var query = new GetScopesQuery
        {
            SortBy = sortBy,
            Skip = skip,
            Take = take,
            UserId = Guid.Parse(userId)
        };
        
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    // Pozostałe metody kontrolera
}
```

### 7. Rejestracja zależności
```csharp
// Program.cs lub Startup.cs
services.AddScoped<IScopeRepository, ScopeRepository>();
services.AddScoped<ITopicRepository, TopicRepository>();
services.AddMediatR(typeof(GetScopesQuery).Assembly);
services.AddValidatorsFromAssembly(typeof(GetScopesQueryValidator).Assembly);
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
``` 