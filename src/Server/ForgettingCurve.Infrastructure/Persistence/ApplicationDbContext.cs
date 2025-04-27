using ForgettingCurve.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForgettingCurve.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<Scope> Scopes { get; set; } = null!;
    public DbSet<Topic> Topics { get; set; } = null!;
    public DbSet<Repetition> Repetitions { get; set; } = null!;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
} 