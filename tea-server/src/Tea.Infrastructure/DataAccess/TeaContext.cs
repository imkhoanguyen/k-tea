using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tea.Application.DTOs.Orders;
using Tea.Domain.Common;
using Tea.Domain.Entities;
using Tea.Infrastructure.DataAccess.EntityConfigs;

namespace Tea.Infrastructure.DataAccess
{
    public class TeaContext : IdentityDbContext<AppUser>
    {
        public virtual DbSet<OrderListResponse> OrderListResponses { get; set; }
        public virtual DbSet<TopSellingItemResponse> TopSellingItemResponses { get; set; }
        public virtual DbSet<DailyRevenueInMonthResponse> DailyRevenueInMonthResponses { get; set; }
        public TeaContext(DbContextOptions options) : base(options) { }


        #region DbSet
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<OrderListResponse>().HasNoKey();
            builder.Entity<TopSellingItemResponse>().HasNoKey();
            builder.Entity<DailyRevenueInMonthResponse>().HasNoKey();
            builder.ApplyConfigurationsFromAssembly(typeof(CategoryConfig).Assembly);
        }
    }
}
