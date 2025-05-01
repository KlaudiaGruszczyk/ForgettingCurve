using MediatR;

namespace ForgettingCurve.Application.Commands.VerifyEmail
{
    public class VerifyEmailCommand : IRequest<Unit>
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
} 