using System.Linq.Expressions;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Domain.Interfaces.Base;
using dotnet_api_erp.src.Infrastructure.Data;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;

namespace dotnet_api_erp.src.Application.Services.Base
{
    public class BaseService<TEntity, TRepository>(TRepository repository, ApplicationDbContext context) where TEntity : class where TRepository : IBaseRepository<TEntity>
    {
        protected readonly ApplicationDbContext _context = context;
        protected readonly TRepository _repository = repository;

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct) //POST
        {
            await _repository.AddAsync(entity, ct);
            return entity;
        }
        public virtual async Task AddRangeAsync(IList<TEntity> entities, CancellationToken ct) //POST
        {
            await _repository.AddRangeAsync(entities, ct);
        }
        public virtual async Task<TEntity> GetByIdAsync(Guid Id, CancellationToken ct) //GET{}
        {
            return await _repository.GetByIdAsync(Id, ct) ?? throw new NotFoundException("Entity not found");
        }
        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct) //GET
        {
            return await _repository.GetAsync(predicate, ct) ?? throw new NotFoundException("Nenhumm dado encontrado.");
        }
        public virtual async Task<ApiResponseTable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, int offset, int limit, CancellationToken ct) //GET
        {
            var data = await _repository.GetAllAsync(predicate, offset, limit, ct) ?? throw new NotFoundException("Nenhum dado encontrado.");
            var totalItems = await _repository.CountAsync(predicate, ct);

            return new ApiResponseTable<TEntity>
            {
                Data = [.. data],
                TotalItems = totalItems
            };
        }
        public virtual async Task DeleteAsync(Guid Id, CancellationToken ct) //DELETE
        {
            TEntity entity = await GetByIdAsync(Id, ct);
            var deleteMethod = typeof(TEntity).GetMethod("Delete", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (deleteMethod != null)
            {
                deleteMethod.Invoke(entity, null);
                await _repository.Update(entity, ct);
            }
            else
            {
                await _repository.Remove(entity, ct);
            }
        }
        public virtual Task DeleteRangeAsync(List<Guid> Ids, CancellationToken ct)
        {
            List<Task> tasks = [];
            foreach (var Id in Ids)
            {
                tasks.Add(DeleteAsync(Id, ct));
            }
            return Task.WhenAll(tasks);
        }
        public virtual Task<TEntity> UpdateAsync(Guid Id, TEntity entity, CancellationToken ct) //PUT
        {
            throw new NotImplementedException("Essa função deve ser sobrescrita");
        }
    }
}