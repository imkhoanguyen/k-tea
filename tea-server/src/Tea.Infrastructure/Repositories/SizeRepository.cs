using Tea.Domain.Entities;
using Tea.Domain.Repositories;
using Tea.Infrastructure.DataAccess;

namespace Tea.Infrastructure.Repositories
{
    public class SizeRepository(TeaContext context) : Repository<Size>(context), ISizeRepository
    {
    }
}
