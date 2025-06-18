using Webgostar.Framework.Base.BaseModels;

namespace Webgostar.Framework.Base.IBaseServices
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken token = new CancellationToken());
        void BeginTransaction();
        Task BeginTransactionAsync(CancellationToken token = new CancellationToken());
        void CommitTransaction();
        Task CommitTransactionAsync(CancellationToken token = new CancellationToken());
        void RollbackTransaction();
        Task RollbackTransactionAsync(CancellationToken token = new CancellationToken());
        Task ExecuteTransactionAsync(Action action, CancellationToken token = new CancellationToken());
        Task ExecuteTransactionAsync(Func<Task> action, CancellationToken token = new CancellationToken());
    }
}
