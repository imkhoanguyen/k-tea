using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tea.Domain.Entities;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;

namespace Tea.Infrastructure.Repositories
{
    public class ItemRepository(TeaContext context) : Repository<Item>(context), IItemRepository
    {
        public override async Task<Item?> FindAsync(Expression<Func<Item, bool>>? predicate, bool tracked = false)
        {
            if (predicate == null)
                return tracked ? await context.Items.Include(x => x.Sizes)
                    .Include(x => x.ItemCategories).ThenInclude(x => x.Category).FirstOrDefaultAsync()
                : await context.Items.Include(x => x.Sizes)
                    .Include(x => x.ItemCategories).ThenInclude(x => x.Category)
                    .AsNoTracking().FirstOrDefaultAsync();

            return tracked ? await context.Items.Include(x => x.Sizes)
                    .Include(x => x.ItemCategories).ThenInclude(x=> x.Category).FirstOrDefaultAsync(predicate)
                : await context.Items.Include(x => x.Sizes)
                    .Include(x => x.ItemCategories).ThenInclude(x=> x.Category)
                    .AsNoTracking().FirstOrDefaultAsync(predicate);
        }
    }
}
