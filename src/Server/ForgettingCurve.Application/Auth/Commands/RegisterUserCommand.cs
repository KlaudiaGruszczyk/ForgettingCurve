using MediatR;

namespace ForgettingCurve.Application.Auth.Commands;

public class RegisterUserCommand : IRequest<Unit>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
} 