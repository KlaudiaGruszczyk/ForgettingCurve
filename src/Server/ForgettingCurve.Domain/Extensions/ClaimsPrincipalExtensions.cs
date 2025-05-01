using System.Security.Claims;

namespace ForgettingCurve.Domain.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsResourceOwner(this ClaimsPrincipal user, Guid ownerId)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId != null && ownerId.ToString() == userId;
        }

        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId != null ? Guid.Parse(userId) : null;
        }
    }
} 