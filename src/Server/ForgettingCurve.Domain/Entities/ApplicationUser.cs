using Microsoft.AspNetCore.Identity;

namespace ForgettingCurve.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
} 