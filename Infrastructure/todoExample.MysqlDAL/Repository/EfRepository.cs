using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using todoExample.domain.Models;
using todoExample.domain.Respository.Base;
using todoExample.MysqlDAL.Data;

namespace todoExample.MysqlDAL.Repository;

public class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly IdentityMySqlContext _context;
    private readonly DbSet<T> _set;

    public EfRepository(IdentityMySqlContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _set.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        IQueryable<T> query = _set;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.ToListAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _set.AnyAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
        {
            return await _set.CountAsync();
        }

        return await _set.CountAsync(predicate);
    }

    public async Task AddAsync(T entity)
    {
        _set.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _set.Update(entity);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        entity.Enabled = false;
        await UpdateAsync(entity);
    }
}