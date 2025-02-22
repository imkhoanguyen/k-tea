using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;
using Tea.Infrastructure.Utilities;

namespace Tea.Infrastructure.Repositories
{
    public class CategoryRepository(TeaContext context) : Repository<Category>(context), ICategoryRepository
    {
        public async Task<PaginationResponse<Category>> GetPaginationAsync(PaginationRequest request)
        {
            var query = context.Categories.AsNoTracking().Include(x => x.Children).AsQueryable();

            query = query.Where(x => !x.IsDeleted && x.ParentId == null);

            if(!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.Search) 
                || x.Id.ToString().Contains(request.Search));
            }

            if(!string.IsNullOrEmpty(request.OrderBy))
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

        public override async Task<Category?> FindAsync(Expression<Func<Category, bool>>? predicate, bool tracked = false)
        {
            if (predicate == null)
                return tracked ? await context.Categories.Include(x => x.Children).FirstOrDefaultAsync()
                : await context.Categories.Include(x=> x.Children).AsNoTracking().FirstOrDefaultAsync();

            return tracked ? await context.Categories.Include(x => x.Children).FirstOrDefaultAsync(predicate)
                : await context.Categories.Include(x => x.Children).AsNoTracking().FirstOrDefaultAsync(predicate);
        }
    }
}
