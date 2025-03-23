using System.Linq.Expressions;

namespace dotnet_api_erp.src.Domain.Interfaces.Base
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        // Metodos de leitura
        Task<TEntity?> GetByIdAsync(int Id, CancellationToken ct, Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeExpression = null);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, int offset, int limit, CancellationToken ct, Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeExpression = null);
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct, Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeExpression = null);

        // Metodos de escrita
        Task AddAsync(TEntity entity, CancellationToken ct);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct);
        Task Update(TEntity entity, CancellationToken ct);
        Task UpdateRange(IEnumerable<TEntity> entities, CancellationToken ct);
        Task Remove(TEntity entity, CancellationToken ct);
        Task RemoveRange(IEnumerable<TEntity> entities, CancellationToken ct);
    }
}