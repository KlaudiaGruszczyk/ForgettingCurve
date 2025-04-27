using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForgettingCurve.Domain.Entities;

namespace ForgettingCurve.Application.Abstractions;

public interface ITopicRepository : IRepository<Topic>
{
    Task<IEnumerable<Topic>> GetTopicsByScopeIdAsync(Guid scopeId, Guid userId, bool includeMastered = true);
    Task<Topic?> GetTopicWithRepetitionsAsync(Guid topicId, Guid userId);
    Task<bool> BelongsToUserAsync(Guid topicId, Guid userId);
    Task<IEnumerable<Topic>> GetTopicsWithUpcomingRepetitionsAsync(Guid userId);
} 