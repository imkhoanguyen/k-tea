using Microsoft.EntityFrameworkCore;
using Tea.Domain.Entities;

namespace Tea.Infrastructure.DataAccess
{
    public class TeaContext : DbContext
    {
        public TeaContext(DbContextOptions options) : base(options) { }


        #region DbSet
        public DbSet<Category> Categories { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
