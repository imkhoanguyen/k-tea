using Microsoft.EntityFrameworkCore;
using Tea.Domain.Entities;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;

namespace Tea.Infrastructure.Repositories
{
    public class ItemCategoryRepository(TeaContext context) : Repository<ItemCategory>(context), IItemCategoryRepository
    {
    }
}
