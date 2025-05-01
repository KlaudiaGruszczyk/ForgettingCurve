using MediatR;
using Microsoft.AspNetCore.Identity;
using ForgettingCurve.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using ForgettingCurve.Domain.Exceptions;

namespace ForgettingCurve.Application.Commands.VerifyEmail
{
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
                throw new UserNotFoundException(request.Email);
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
            {
                throw new InvalidVerificationTokenException();
            }

            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return Unit.Value;
        }
    }
} 