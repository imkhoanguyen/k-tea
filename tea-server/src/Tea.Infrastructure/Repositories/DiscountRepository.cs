using Microsoft.EntityFrameworkCore;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;
using Tea.Infrastructure.Utilities;

namespace Tea.Infrastructure.Repositories
{
    public class DiscountRepository(TeaContext context) : Repository<Discount>(context), IDiscountRepository
    {
        public async Task<PaginationResponse<Discount>> GetPaginationAsync(PaginationRequest request)
        {
            var query = context.Discounts.AsNoTracking().AsQueryable();

            query = query.Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.Search)
                || x.PromotionCode.ToString().Contains(request.Search));
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
