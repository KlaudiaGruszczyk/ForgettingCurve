using Microsoft.EntityFrameworkCore;
using ForgettingCurve.Domain.Entities;

namespace ForgettingCurve.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<Scope> Scopes { get; }
    DbSet<Topic> Topics { get; }
    DbSet<Repetition> Repetitions { get; }
    DbSet<EmailVerificationToken> EmailVerificationTokens { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
} 