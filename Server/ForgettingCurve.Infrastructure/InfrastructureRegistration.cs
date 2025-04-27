using ForgettingCurve.Application.Abstractions;
using ForgettingCurve.Infrastructure.Persistence;
using ForgettingCurve.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForgettingCurve.Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
                
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IScopeRepository, ScopeRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<IRepetitionRepository, RepetitionRepository>();
        
        return services;
    }
} 