using MediatR;

namespace ForgettingCurve.Application.Commands.Register
{
    public class RegisterCommand : IRequest<Unit>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
} 