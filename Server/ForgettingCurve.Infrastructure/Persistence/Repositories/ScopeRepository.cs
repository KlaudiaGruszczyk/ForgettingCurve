using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForgettingCurve.Application.Abstractions;
using ForgettingCurve.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForgettingCurve.Infrastructure.Persistence.Repositories;

public class ScopeRepository : Repository<Scope>, IScopeRepository
{
    public ScopeRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Scope>> GetScopesByUserIdAsync(Guid userId)
    {
        return await Context.Scopes
            .Where(s => s.OwnerUserId == userId)
            .OrderByDescending(s => s.CreationDate)
            .ToListAsync();
    }
    
    public async Task<Scope?> GetScopeWithTopicsAsync(Guid scopeId, Guid userId)
    {
        return await Context.Scopes
            .Include(s => s.Topics)
            .FirstOrDefaultAsync(s => s.Id == scopeId && s.OwnerUserId == userId);
    }
    
    public async Task<bool> BelongsToUserAsync(Guid scopeId, Guid userId)
    {
        return await Context.Scopes
            .AnyAsync(s => s.Id == scopeId && s.OwnerUserId == userId);
    }
} 