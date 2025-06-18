using System.Linq.Expressions;
using Webgostar.Framework.Base.BaseModels;
using Webgostar.Framework.Base.BaseModels.GridData;

namespace Webgostar.Framework.Base.IBaseServices
{
    public interface IBaseReadRepository<TEntity> where TEntity : BaseEntity
    {
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null);
        Task<TEntity> GetByIdAsync(long id, CancellationToken cancellationToken = new CancellationToken());
        Task<TEntity> GetTracking(long id, CancellationToken cancellationToken = new CancellationToken());
        Task<List<TEntity>> GetAll(CancellationToken cancellationToken = new CancellationToken());
        IQueryable<TEntity> GetFilterPaging(BaseGrid baseGrid);
        Task<GridData<TDto>> GetFilterPagingDto<TDto>(BaseGrid baseGrid, CancellationToken cancellationToken = new CancellationToken());
        int GetFilterCount(BaseGrid baseGrid);
        IQueryable<TEntity> Table();
        IQueryable<TEntity> TableWithDelete();
        IQueryable<TNewEntity> Context<TNewEntity>() where TNewEntity : BaseEntity;
        IQueryable<TNewEntity> ContextWithDelete<TNewEntity>() where TNewEntity : BaseEntity;
    }
}
