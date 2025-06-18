using Microsoft.EntityFrameworkCore.Storage;
using Webgostar.Framework.Base.BaseModels;
using Webgostar.Framework.Base.IBaseServices;
using Webgostar.Framework.Infrastructure.BaseServices.IDIContainer;
using Webgostar.Framework.Infrastructure.Contexts;
using Webgostar.Framework.Infrastructure.InfrastructureExceptions;
using Webgostar.Framework.Infrastructure.InfrastructureExtentisions;
using Webgostar.Framework.Infrastructure.InfrastructureIServices;

namespace Webgostar.Framework.Infrastructure.BaseServices;

public class UnitOfWork : IUnitOfWork
{
    private readonly IRepositoryServices _repositoryServices;
    private readonly ILoggingContext _loggingContext;
    private readonly EfBaseContext _context;

    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(EfBaseContext dbContext, IRepositoryServices repositoryServices, ILoggingContext loggingContext)
    {
        _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _repositoryServices = repositoryServices;
        _loggingContext = loggingContext;
    }

    public IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
    {
        var type = typeof(TEntity);

        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new BaseRepository<TEntity>(_context, _repositoryServices);
        }

        return (IBaseRepository<TEntity>)_repositories[type];
    }

    public int SaveChanges()
    {
        var result = _context.SaveChanges();

        _loggingContext.FlushLogs();

        return result;
    }

    public async Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        var result = await _context.SaveChangesAsync(token);

        await _loggingContext.FlushLogsAsync();

        return result;
    }

    public void BeginTransaction()
    {
        try
        {
            _transaction = _context.Database.CurrentTransaction ?? _context.Database.BeginTransaction();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken token = default)
    {
        try
        {
            _transaction = _context.Database.CurrentTransaction ?? await _context.Database.BeginTransactionAsync(token);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void CommitTransaction()
    {
        if (_transaction == null) throw new UserFriendlyException(ErrorCode.Internal, "No Transaction To Commit");
        try
        {
            _context.SaveChanges();
            _transaction.Commit();
            _loggingContext.FlushLogs();
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async Task CommitTransactionAsync(CancellationToken token = default)
    {
        if (_transaction == null) throw new UserFriendlyException(ErrorCode.Internal, "No Transaction To Commit");
        try
        {
            await _context.SaveChangesAsync(token);
            await _transaction.CommitAsync(token);
            await _loggingContext.FlushLogsAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void RollbackTransaction()
    {
        if (_transaction == null) throw new UserFriendlyException(ErrorCode.Internal, "No Transaction To Rollback");
        _transaction.Rollback();
        _transaction.Dispose();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken token = default)
    {
        if (_transaction == null) throw new UserFriendlyException(ErrorCode.Internal, "No Transaction To Rollback");
        await _transaction.RollbackAsync(token);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task ExecuteTransactionAsync(Action action, CancellationToken token = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(token);
        try
        {
            action();
            await _context.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
            await _loggingContext.FlushLogsAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(token);
            throw new UserFriendlyException(ErrorCode.Internal, $"Can't Execute Transaction: {ex.Message}");
        }
    }

    public async Task ExecuteTransactionAsync(Func<Task> action, CancellationToken token = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(token);
        try
        {
            await action();
            await _context.SaveChangesAsync(token);
            await transaction.CommitAsync(token);
            await _loggingContext.FlushLogsAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(token);
            throw new UserFriendlyException(ErrorCode.Internal, $"Can't Execute Transaction: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _transaction?.Dispose();
        _context?.Dispose();
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_transaction != null) await _transaction.DisposeAsync();
            await _context.DisposeAsync();
            _disposed = true;
        }
    }
}
