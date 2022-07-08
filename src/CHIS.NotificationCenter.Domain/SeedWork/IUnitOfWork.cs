using System;
using System.Threading;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Domain.SeedWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> SaveEntitiesWithMessagingAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
