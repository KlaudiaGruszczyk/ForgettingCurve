using FluentValidation;
using ForgettingCurve.Application.Auth.Commands;

namespace ForgettingCurve.Application.Auth.Commands;

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany")
            .EmailAddress().WithMessage("Nieprawidłowy format adresu email");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token weryfikacyjny jest wymagany");
    }
} 