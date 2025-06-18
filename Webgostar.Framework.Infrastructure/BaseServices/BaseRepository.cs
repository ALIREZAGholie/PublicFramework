using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Webgostar.Framework.Base.BaseModels;
using Webgostar.Framework.Base.BaseModels.GridData;
using Webgostar.Framework.Base.IBaseServices;
using Webgostar.Framework.Infrastructure.BaseServices.IDIContainer;
using Webgostar.Framework.Infrastructure.Contexts;
using Webgostar.Framework.Infrastructure.InfrastructureIServices;
using Webgostar.Framework.Infrastructure.InfrastructureModels.DbModels;

namespace Webgostar.Framework.Infrastructure.BaseServices
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly EfBaseContext _context;
        private readonly DbSet<TEntity> _dbTable;
        private readonly ILoggingContext _loggingContext;
        public readonly IAuthService _authService;

        public BaseRepository(EfBaseContext context, IRepositoryServices repositoryServices)
        {
            _context = context;
            _authService = repositoryServices.AuthService;
            _loggingContext = repositoryServices.LoggingContext;

            _dbTable = _context.Set<TEntity>();
        }

        #region Publisher
        public virtual async Task Add(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return;

            entity.SetCreate(_authService.GetUserId());

            await _dbTable.AddAsync(entity, cancellationToken);
            _loggingContext.AddLog(typeof(TEntity).Name, OperationLogType.Add, entity);
        }

        public virtual async Task Update(TEntity entity)
        {
            if (entity == null) return;

            if (_dbTable.Entry(entity).State == EntityState.Unchanged)
            {
                _dbTable.Entry(entity).State = EntityState.Detached;
            }

            _dbTable.Update(entity);
            _loggingContext.AddLog(typeof(TEntity).Name, OperationLogType.Update, entity);
        }

        public virtual async Task Remove(long id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);

            if (entity == null) return;

            entity.SetDelete();

            _dbTable.Update(entity);

            _loggingContext.AddLog(typeof(TEntity).Name, OperationLogType.Delete, entity);
        }
        #endregion

        #region Read
        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null
                ? await _dbTable.AnyAsync()
                : await _dbTable.AnyAsync(filter);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            return filter == null
                ? await _dbTable.CountAsync()
                : await _dbTable.CountAsync(filter);
        }

        public virtual async Task<TEntity> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await Table().FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
        }

        public virtual async Task<TEntity> GetTracking(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().AsTracking().FirstOrDefaultAsync(t => t.Id.Equals(id), cancellationToken);
        }

        public virtual async Task<List<TEntity>> GetAll(CancellationToken cancellationToken = default)
        {
            var list = await Table().OrderByDescending(x => x.Id).ToListAsync(cancellationToken);
            return list;
        }

        public virtual int GetFilterCount(BaseGrid baseGrid)
        {
            return baseGrid.FilterParam != null && baseGrid.FilterParam.Any()
                ? _dbTable.Where(x => x.IsDelete == false)
                          .FilterList(baseGrid)
                          .AsNoTracking()
                          .Count()

                : _dbTable.Where(x => x.IsDelete == false)
                          .AsNoTracking()
                          .Count();
        }

        public virtual IQueryable<TEntity> GetFilterPaging(BaseGrid baseGrid)
        {
            return baseGrid.FilterParam != null && baseGrid.FilterParam.Any()
                ? _dbTable.Where(x => x.IsDelete == false)
                          .FilterPagingList(baseGrid)
                          .AsNoTracking()
                : _dbTable.Where(x => x.IsDelete == false)
                          .PagingList(baseGrid)
                          .AsNoTracking();
        }

        public virtual async Task<GridData<TDto>> GetFilterPagingDto<TDto>(BaseGrid baseGrid, CancellationToken cancellationToken = default)
        {
            var query = GetFilterPaging(baseGrid);

            var queryList = await query.ToListAsync(cancellationToken);

            var mapedList = queryList.Adapt<List<TDto>>();

            var dto = new GridData<TDto>(mapedList, baseGrid, GetFilterCount(baseGrid));

            return dto;
        }

        public virtual IQueryable<TEntity> Table()
        {
            return _dbTable.Where(x => x.IsDelete == false).AsNoTracking();
        }

        public virtual IQueryable<TEntity> TableWithDelete()
        {
            return _dbTable.AsNoTracking();
        }

        public virtual IQueryable<TNewEntity> Context<TNewEntity>() where TNewEntity : BaseEntity
        {
            return _context.Set<TNewEntity>().Where(x => x.IsDelete == false).AsNoTracking();
        }

        public virtual IQueryable<TNewEntity> ContextWithDelete<TNewEntity>() where TNewEntity : BaseEntity
        {
            return _context.Set<TNewEntity>().AsNoTracking();
        }
        #endregion
    }
}