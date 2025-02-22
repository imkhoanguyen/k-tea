using Microsoft.EntityFrameworkCore;
using Tea.Domain.Entities;
using Tea.Infrastructure.DataAccess.EntityConfigs;

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
            builder.ApplyConfigurationsFromAssembly(typeof(CategoryConfig).Assembly);
        }
    }
}
