# API Endpoint Implementation Plan: Logowanie użytkownika

## 1. Przegląd punktu końcowego
Endpoint umożliwia zalogowanie zweryfikowanego użytkownika do aplikacji, weryfikując jego dane uwierzytelniające i zwracając token JWT niezbędny do autoryzacji kolejnych żądań.

## 2. Szczegóły żądania
- Metoda HTTP: POST
- Struktura URL: `/api/auth/login`
- Request Body:
  ```json
  {
    "email": "string",
    "password": "string"
  }
  ```
- Wymagane pola:
  - email (string): adres email użytkownika
  - password (string): hasło użytkownika

## 3. Wykorzystywane typy

### Modele żądań
```csharp
namespace ForgettingCurve.Application.Identity.Authentication.Commands
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
```

### Walidatory
```csharp
namespace ForgettingCurve.Application.Identity.Authentication.Validators
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email address is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
```

### Modele odpowiedzi
```csharp
namespace ForgettingCurve.Application.Identity.Authentication.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Guid UserId { get; set; }
    }
}
```

### Handler komend
```csharp
namespace ForgettingCurve.Application.Identity.Authentication.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        // Implementacja w dalszej części
    }
}
```

## 4. Szczegóły odpowiedzi
- Pomyślna odpowiedź (200 OK):
  ```json
  {
    "token": "string",
    "expiresAt": "2023-01-01T00:00:00Z",
    "userId": "guid"
  }
  ```
- Błędne dane (400 Bad Request):
  ```json
  {
    "type": "validation_error",
    "title": "Validation failed",
    "status": 400,
    "errors": {
      "email": ["The Email field is required"],
      "password": ["The Password field is required"]
    }
  }
  ```
- Nieprawidłowe dane logowania (401 Unauthorized):
  ```json
  {
    "type": "authentication_error",
    "title": "Invalid email or password",
    "status": 401
  }
  ```
- Konto niezweryfikowane (403 Forbidden):
  ```json
  {
    "type": "verification_error",
    "title": "Account not verified",
    "status": 403,
    "detail": "Please verify your email address to continue"
  }
  ```

## 5. Przepływ danych
1. Kontroler otrzymuje żądanie POST z danymi logowania
2. Żądanie jest walidowane przez validator FluentValidation
3. Jeśli walidacja się powiedzie, żądanie jest przekazywane do handlera poprzez MediatR
4. Handler używa ASP.NET Core Identity do weryfikacji danych logowania
5. Jeśli dane są prawidłowe, handler sprawdza czy konto jest zweryfikowane
6. Jeśli konto jest zweryfikowane, handler generuje token JWT, ustawia czas wygaśnięcia i zwraca dane
7. Kontroler zwraca te informacje do klienta

## 6. Względy bezpieczeństwa
- **Uwierzytelnianie**: 
  - Implementacja używa ASP.NET Core Identity do weryfikacji danych logowania
  - Hasła są przechowywane w formie zahaszowanej, nigdy jako czysty tekst
- **Walidacja**:
  - Walidacja danych wejściowych zapobiega atakom typu injection
  - Kontrola poprawności adresu email
- **Autoryzacja**:
  - Generowanie bezpiecznego tokenu JWT z odpowiednim czasem życia (1 godzina)
- **Ochrona przed atakami**:
  - Implementacja limitów prób logowania, aby zapobiec atakom brute force
  - Stosowanie opóźnień przy niepoprawnych próbach logowania (rate limiting)
- **HTTPS**:
  - Zapewnienie, że endpoint jest dostępny tylko przez HTTPS

## 7. Obsługa błędów
- **400 Bad Request**:
  - Brakujące wymagane pola (email, password)
  - Nieprawidłowy format adresu email
- **401 Unauthorized**:
  - Nieprawidłowy adres email (użytkownik nie istnieje)
  - Nieprawidłowe hasło
- **403 Forbidden**:
  - Konto użytkownika nie zostało zweryfikowane
- **500 Internal Server Error**:
  - Nieoczekiwane błędy serwera
  - Problemy z bazą danych
  - Błędy przy generowaniu tokenu JWT

## 8. Rozważania dotyczące wydajności
- Wykorzystanie cachowania dla często używanych operacji Identity
- Optymalizacja zapytań do bazy danych
- Minimalizacja czasu generowania tokenu JWT
- Możliwość skalowania poziomego poprzez bezstanowy charakter endpointu

## 9. Etapy wdrożenia

### 1. Przygotowanie warstwy aplikacji
- Utworzenie klasy `LoginCommand` implementującej `IRequest<LoginResponse>`
- Implementacja walidatora `LoginCommandValidator`
- Utworzenie klasy odpowiedzi `LoginResponse`
- Stworzenie handlera komendy `LoginCommandHandler` implementującego `IRequestHandler<LoginCommand, LoginResponse>`

```csharp
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Znajdź użytkownika po emailu
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        // Sprawdź czy użytkownik istnieje
        if (user == null)
        {
            _logger.LogWarning("Login failed: User with email {Email} not found", request.Email);
            throw new AuthenticationException("Invalid email or password");
        }
        
        // Sprawdź poprawność hasła
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Login failed: Invalid password for user {Email}", request.Email);
            throw new AuthenticationException("Invalid email or password");
        }
        
        // Sprawdź czy email jest zweryfikowany
        if (!user.EmailConfirmed)
        {
            _logger.LogWarning("Login failed: Email not verified for user {Email}", request.Email);
            throw new AccountNotVerifiedException("Account not verified");
        }
        
        // Wygeneruj token JWT
        var (token, expiresAt) = await _jwtTokenGenerator.GenerateTokenAsync(user);
        
        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id
        };
    }
}
```

### 2. Implementacja generatora tokenu JWT

```csharp
public interface IJwtTokenGenerator
{
    Task<(string token, DateTime expiresAt)> GenerateTokenAsync(ApplicationUser user);
}

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager)
    {
        _jwtSettings = jwtSettings.Value;
        _userManager = userManager;
    }

    public async Task<(string token, DateTime expiresAt)> GenerateTokenAsync(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        // Dodaj role jako claims
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var expiresAt = DateTime.UtcNow.AddHours(1);
        
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );
        
        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
```

### 3. Utworzenie kontrolera

```csharp
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.ToProblemDetails());
        }
        catch (AuthenticationException)
        {
            return Unauthorized(new ProblemDetails
            {
                Type = "authentication_error",
                Title = "Invalid email or password",
                Status = StatusCodes.Status401Unauthorized
            });
        }
        catch (AccountNotVerifiedException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Type = "verification_error",
                Title = "Account not verified",
                Status = StatusCodes.Status403Forbidden,
                Detail = "Please verify your email address to continue"
            });
        }
    }
}
```

### 4. Dodanie niestandardowych wyjątków

```csharp
public class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message) { }
}

public class AccountNotVerifiedException : Exception
{
    public AccountNotVerifiedException(string message) : base(message) { }
}
```

### 5. Konfiguracja zależności w DI

```csharp
// Program.cs lub odpowiedni plik konfiguracyjny
services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Dodanie konfiguracji JWT
services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

// Rejestracja walidatorów
services.AddValidatorsFromAssembly(typeof(LoginCommandValidator).Assembly);

// Rejestracja MediatR
services.AddMediatR(typeof(LoginCommand).Assembly);
```

### 6. Dodanie middleware do obsługi JWT

```csharp
// Program.cs
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["JwtSettings:Issuer"],
        ValidAudience = configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]))
    };
});
```
