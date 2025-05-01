using FluentValidation;
using ForgettingCurve.Application.Auth;
using ForgettingCurve.Application.Responses;
using ForgettingCurve.Domain.Entities;
using ForgettingCurve.Domain.Identity.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ForgettingCurve.Application.Commands.Login
{
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
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                throw new ValidationException("Email and password are required.");
            }

            // Find user by email
            var user = await _userManager.FindByEmailAsync(request.Email);
            
            // Check if user exists
            if (user == null)
            {
                _logger.LogWarning("Login failed: User with email {Email} not found", request.Email);
                throw new AuthenticationException("Invalid email or password");
            }
            
            // Check if account is locked out
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("Login failed: Account is locked out for user {Email}", request.Email);
                throw new AccountLockedException("Account is temporarily locked due to too many failed login attempts", user.LockoutEnd);
            }
            
            // Check password validity
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed: Invalid password for user {Email}", request.Email);
                
                // Increment failed access attempts
                await _userManager.AccessFailedAsync(user);
                
                // Check if the account is now locked out after this failed attempt
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("Account locked out after too many failed attempts for user {Email}", request.Email);
                    throw new AccountLockedException("Account is temporarily locked due to too many failed login attempts", user.LockoutEnd);
                }
                
                throw new AuthenticationException("Invalid email or password");
            }
            
            // Check if email is verified
            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Login failed: Email not verified for user {Email}", request.Email);
                throw new AccountNotVerifiedException("Account not verified");
            }
            
            // Reset failed access attempts on successful login
            await _userManager.ResetAccessFailedCountAsync(user);
            
            // Generate JWT token
            var (token, expiresAt) = await _jwtTokenGenerator.GenerateTokenAsync(user);
            
            return new LoginResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserId = user.Id
            };
        }
    }
} 