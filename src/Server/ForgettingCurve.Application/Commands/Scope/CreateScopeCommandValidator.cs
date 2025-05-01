using FluentValidation;

namespace ForgettingCurve.Application.Commands.Scope
{
    public class CreateScopeCommandValidator : AbstractValidator<CreateScopeCommand>
    {
        public CreateScopeCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Scope name is required")
                .MaximumLength(150).WithMessage("Scope name cannot exceed 150 characters");
            
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required");
        }
    }
} 