using ForgettingCurve.Domain.Entities;
using ForgettingCurve.Domain.Interfaces;

namespace ForgettingCurve.Application.Abstractions;

public interface ITopicRepository : IRepository<Topic>, IResourceOwnershipVerifier
{
    Task<IEnumerable<Topic>> GetTopicsByScopeIdAsync(Guid scopeId);
    Task<IEnumerable<Topic>> GetTopicsByScopeIdAsync(Guid scopeId, Guid userId, bool includeMastered = true);
    Task<Topic?> GetTopicWithRepetitionsAsync(Guid topicId, Guid userId);
    Task<bool> BelongsToUserAsync(Guid topicId, Guid userId);
    Task<IEnumerable<Topic>> GetTopicsWithUpcomingRepetitionsAsync(Guid userId);
} 