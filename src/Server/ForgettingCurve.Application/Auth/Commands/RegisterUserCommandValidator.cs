using FluentValidation;
using System.Text.RegularExpressions;
using ForgettingCurve.Application.Auth.Commands;

namespace ForgettingCurve.Application.Auth.Commands;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany")
            .EmailAddress().WithMessage("Nieprawidłowy format adresu email");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Hasło jest wymagane")
            .MinimumLength(8).WithMessage("Hasło musi mieć co najmniej 8 znaków")
            .Matches("[A-Z]").WithMessage("Hasło musi zawierać co najmniej jedną wielką literę")
            .Matches("[a-z]").WithMessage("Hasło musi zawierać co najmniej jedną małą literę")
            .Matches("[0-9]").WithMessage("Hasło musi zawierać co najmniej jedną cyfrę");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Potwierdzenie hasła jest wymagane")
            .Equal(x => x.Password).WithMessage("Hasła nie są identyczne");
    }
} 