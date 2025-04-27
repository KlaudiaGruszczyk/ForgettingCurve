using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForgettingCurve.Domain.Entities;

namespace ForgettingCurve.Application.Abstractions;

public interface IRepetitionRepository : IRepository<Repetition>
{
    Task<IEnumerable<Repetition>> GetRepetitionsByTopicIdAsync(Guid topicId);
    Task<Repetition?> GetRepetitionWithTopicAsync(Guid repetitionId);
    Task<IEnumerable<Repetition>> GetDueRepetitionsAsync(Guid userId, DateTime date);
    Task<Repetition?> GetLastCompletedRepetitionAsync(Guid topicId);
    Task<bool> IsOwnedByUserAsync(Guid repetitionId, Guid userId);
} 