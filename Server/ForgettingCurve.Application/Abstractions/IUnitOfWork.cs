using System;
using System.Threading;
using System.Threading.Tasks;

namespace ForgettingCurve.Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 