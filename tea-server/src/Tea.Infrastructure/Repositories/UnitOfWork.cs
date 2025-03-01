using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;

namespace Tea.Infrastructure.Repositories
{
    public class UnitOfWork(TeaContext context) : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; } = new CategoryRepository(context);

        public ISizeRepository Size { get; private set; } = new SizeRepository(context);

        public IItemRepository Item { get; private set; } = new ItemRepository(context);

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
