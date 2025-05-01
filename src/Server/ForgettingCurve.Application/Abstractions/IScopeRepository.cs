using ForgettingCurve.Domain.Entities;

namespace ForgettingCurve.Application.Abstractions;

public interface IScopeRepository : IRepository<Scope>
{
    Task<IEnumerable<Scope>> GetScopesByUserIdAsync(Guid userId);
    Task<Scope?> GetScopeWithTopicsAsync(Guid scopeId, Guid userId);
    Task<bool> BelongsToUserAsync(Guid scopeId, Guid userId);
} 