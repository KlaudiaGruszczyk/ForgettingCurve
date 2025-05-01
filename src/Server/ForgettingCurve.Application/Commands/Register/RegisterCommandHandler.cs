using MediatR;
using Microsoft.AspNetCore.Identity;
using ForgettingCurve.Domain.Entities;
using ForgettingCurve.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ForgettingCurve.Domain.Exceptions;

namespace ForgettingCurve.Application.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IApplicationDbContext _context;

        public RegisterCommandHandler(
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

        public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new UserAlreadyExistsException();
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
                var errors = result.Errors.Select(e => e.Description).ToArray();
                throw new UserRegistrationFailedException(errors);
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
} 