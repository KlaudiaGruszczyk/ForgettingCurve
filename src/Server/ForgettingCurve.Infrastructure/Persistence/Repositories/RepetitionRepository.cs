using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForgettingCurve.Application.Abstractions;
using ForgettingCurve.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForgettingCurve.Infrastructure.Persistence.Repositories;

public class RepetitionRepository : Repository<Repetition>, IRepetitionRepository
{
    public RepetitionRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Repetition>> GetRepetitionsByTopicIdAsync(Guid topicId)
    {
        return await Context.Repetitions
            .Where(r => r.TopicId == topicId)
            .OrderBy(r => r.ScheduledDate)
            .ToListAsync();
    }
    
    public async Task<Repetition?> GetRepetitionWithTopicAsync(Guid repetitionId)
    {
        return await Context.Repetitions
            .Include(r => r.Topic)
            .FirstOrDefaultAsync(r => r.Id == repetitionId);
    }
    
    public async Task<IEnumerable<Repetition>> GetDueRepetitionsAsync(Guid userId, DateTime date)
    {
        return await Context.Repetitions
            .Include(r => r.Topic)
            .ThenInclude(t => t.Scope)
            .Where(r => r.Topic.OwnerUserId == userId && 
                        r.CompletedDate == null && 
                        r.ScheduledDate.Date <= date.Date)
            .OrderBy(r => r.ScheduledDate)
            .ToListAsync();
    }
    
    public async Task<Repetition?> GetLastCompletedRepetitionAsync(Guid topicId)
    {
        return await Context.Repetitions
            .Where(r => r.TopicId == topicId && r.CompletedDate != null)
            .OrderByDescending(r => r.CompletedDate)
            .FirstOrDefaultAsync();
    }
    
    public async Task<bool> IsOwnedByUserAsync(Guid repetitionId, Guid userId)
    {
        return await Context.Repetitions
            .AnyAsync(r => r.Id == repetitionId && r.Topic.OwnerUserId == userId);
    }
} 