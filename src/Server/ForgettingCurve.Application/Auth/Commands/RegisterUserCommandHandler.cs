using MediatR;
using Microsoft.AspNetCore.Identity;
using ForgettingCurve.Domain.Entities;
using ForgettingCurve.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ForgettingCurve.Application.Auth.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IApplicationDbContext _context;

    public RegisterUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IConfiguration configuration,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _emailService = emailService;
        _configuration = configuration;
        _context = context;
    }

    public async Task<Unit> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Sprawdź czy użytkownik już istnieje
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Użytkownik o podanym adresie email już istnieje");
        }

        // Utwórz nowego użytkownika
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            IsActive = false // Konto będzie aktywne dopiero po weryfikacji emaila
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Generuj token weryfikacyjny
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        // Zapisz token w bazie
        var verificationToken = new EmailVerificationToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(1), // Token ważny przez 24 godziny
            IsUsed = false
        };
        
        _context.EmailVerificationTokens.Add(verificationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Wyślij email weryfikacyjny
        var baseUrl = _configuration["AppSettings:BaseUrl"];
        var subject = "Weryfikacja adresu email";
        var htmlContent = $@"
            <h1>Witaj!</h1>
            <p>Kliknij poniższy link, aby zweryfikować swój adres email:</p>
            <a href='{baseUrl}/verify-email?token={token}'>Zweryfikuj email</a>
        ";

        await _emailService.SendEmailAsync(user.Email, subject, htmlContent);

        return Unit.Value;
    }
} 