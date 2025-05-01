using ForgettingCurve.Domain.Entities;
using ForgettingCurve.Domain.Interfaces;

namespace ForgettingCurve.Application.Abstractions;

public interface IScopeRepository : IRepository<Scope>, IResourceOwnershipVerifier
{
    Task<IEnumerable<Scope>> GetScopesByUserIdAsync(Guid userId);
    Task<bool> IsOwnerAsync(Guid scopeId, Guid userId);
    Task<Scope?> GetScopeWithTopicsAsync(Guid scopeId, Guid userId);
    Task<bool> BelongsToUserAsync(Guid scopeId, Guid userId);
} 