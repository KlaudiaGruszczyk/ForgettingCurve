using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForgettingCurve.Application.Abstractions;
using ForgettingCurve.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForgettingCurve.Infrastructure.Persistence.Repositories;

public class TopicRepository : Repository<Topic>, ITopicRepository
{
    public TopicRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Topic>> GetTopicsByScopeIdAsync(Guid scopeId, Guid userId, bool includeMastered = true)
    {
        var query = Context.Topics
            .Where(t => t.ScopeId == scopeId && t.OwnerUserId == userId);
            
        if (!includeMastered)
        {
            query = query.Where(t => !t.IsMastered);
        }
        
        return await query.ToListAsync();
    }
    
    public async Task<Topic?> GetTopicWithRepetitionsAsync(Guid topicId, Guid userId)
    {
        return await Context.Topics
            .Include(t => t.Repetitions)
            .Include(t => t.Scope)
            .FirstOrDefaultAsync(t => t.Id == topicId && t.OwnerUserId == userId);
    }
    
    public async Task<bool> BelongsToUserAsync(Guid topicId, Guid userId)
    {
        return await Context.Topics
            .AnyAsync(t => t.Id == topicId && t.OwnerUserId == userId);
    }
    
    public async Task<IEnumerable<Topic>> GetTopicsWithUpcomingRepetitionsAsync(Guid userId)
    {
        return await Context.Topics
            .Where(t => t.OwnerUserId == userId && !t.IsMastered)
            .Include(t => t.Repetitions.Where(r => r.CompletedDate == null))
            .ToListAsync();
    }
    
    public async Task<bool> IsOwnerAsync(Guid resourceId, Guid userId)
    {
        var topic = await DbSet.FindAsync(resourceId);
        return topic != null && topic.OwnerUserId == userId;
    }

    public async Task<IEnumerable<Topic>> GetTopicsByScopeIdAsync(Guid scopeId)
    {
        return await Context.Topics
            .Where(t => t.ScopeId == scopeId)
            .OrderByDescending(t => t.CreationDate)
            .ToListAsync();
    }
} 