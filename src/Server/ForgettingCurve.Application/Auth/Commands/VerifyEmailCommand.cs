using MediatR;

namespace ForgettingCurve.Application.Auth.Commands;

public class VerifyEmailCommand : IRequest<Unit>
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
} 