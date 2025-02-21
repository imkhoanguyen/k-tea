using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;

namespace Tea.Infrastructure.Repositories
{
    public class UnitOfWork(TeaContext context) : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; } = new CategoryRepository(context);

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
