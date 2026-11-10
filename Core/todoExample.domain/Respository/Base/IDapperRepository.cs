using todoExample.domain.Data;
using todoExample.domain.Models;

namespace todoExample.domain.Respository.Base;

public interface IDapperRepository<T> where T : BaseEntity
{
    #region Properties

    public dynamic ConnectionFactory { get; }

    #endregion

    public Task<T?> GetByIdAsync(Guid id);

    public Task<IEnumerable<T>> GetAllAsync(List<Where>? where = null, List<OrderBy>? orderBy = null, int limit = 20,
        int offset = 0);

    public Task<T?> AddAsync(T entity);

    public Task<T?> UpdateAsync(T entity);

    public Task DeleteAsync(T entity);

    public Task<long> CountAsync(List<Where>? where = null);
}