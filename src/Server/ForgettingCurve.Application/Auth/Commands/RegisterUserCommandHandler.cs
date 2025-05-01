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
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with the provided email already exists");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            IsActive = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        var verificationToken = new EmailVerificationToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsUsed = false
        };
        
        _context.EmailVerificationTokens.Add(verificationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var baseUrl = _configuration["AppSettings:BaseUrl"];
        var subject = "Email Verification";
        var htmlContent = $@"
            <h1>Hello!</h1>
            <p>Click the link below to verify your email address:</p>
            <a href='{baseUrl}/verify-email?token={token}'>Verify Email</a>
        ";

        await _emailService.SendEmailAsync(user.Email, subject, htmlContent);

        return Unit.Value;
    }
} 