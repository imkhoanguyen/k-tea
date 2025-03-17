using System.Linq.Expressions;

namespace Tea.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>>? predicate, bool tracked = false);

        Task<T?> FindAsync(Expression<Func<T, bool>>? predicate, bool tracked = false);

        void Add(T entity);

        void AddRange(IEnumerable<T> entities);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        Task<int> GetCountAsync();

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
