using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;
using Tea.Infrastructure.Utilities;

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

        public async Task<PaginationResponse<Item>> GetPaginationAsync(PaginationRequest request, Expression<Func<Item, bool>>? expression)
        {
            var query = context.Items.Include(x => x.Sizes)
                    .Include(x => x.ItemCategories).ThenInclude(x => x.Category)
                    .AsNoTracking().AsQueryable();

            query = query.Where(x => !x.IsDeleted);

            if(expression != null)
            {
                query = query.Where(expression);
            }

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.Search)
                || x.Id.ToString().Contains(request.Search));
            }

            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                query = request.OrderBy switch
                {
                    "id" => query.OrderBy(x => x.Id),
                    "id_desc" => query.OrderByDescending(x => x.Id),
                    "name" => query.OrderBy(x => x.Name),
                    "name_desc" => query.OrderByDescending(x => x.Name),
                    _ => query.OrderByDescending(x => x.Id)
                };
            }

            return await query.ApplyPaginationAsync(request.PageIndex, request.PageSize);
        }
    }
}
