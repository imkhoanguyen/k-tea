using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;

namespace Tea.Infrastructure.Repositories
{
    public class Repository<T>(TeaContext context) : IRepository<T> where T : class
    {
        public void Add(T entity)
        {
            context.Add(entity);
        }

        public void AddRange(IEnumerable<T> entity)
        {
            context.AddRange(entity);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>().AnyAsync(predicate);
        }

        public virtual async Task<T?> FindAsync(Expression<Func<T, bool>>? predicate, bool tracked = false)
        {
            if(predicate == null)
                return tracked ? await context.Set<T>().FirstOrDefaultAsync()
                : await context.Set<T>().AsNoTracking().FirstOrDefaultAsync();

            return tracked ? await context.Set<T>().FirstOrDefaultAsync(predicate)
                : await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>>? predicate, bool tracked = false)
        {
            if (predicate == null)
                return tracked ? await context.Set<T>().ToListAsync()
                : await context.Set<T>().AsNoTracking().ToListAsync();

            return tracked ? await context.Set<T>().Where(predicate).ToListAsync()
                : await context.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await context.Set<T>().CountAsync();
        }

        public void Remove(T entity)
        {
            context.Remove(entity);
        }
    }
}
