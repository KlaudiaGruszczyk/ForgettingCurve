using MediatR;
using Microsoft.AspNetCore.Identity;
using ForgettingCurve.Domain.Entities;

namespace ForgettingCurve.Application.Auth.Commands;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public VerifyEmailCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Unit> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Invalid verification token");
        }

        user.IsActive = true;
        await _userManager.UpdateAsync(user);

        return Unit.Value;
    }
} 