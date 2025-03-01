using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tea.Domain.Entities;

namespace Tea.Infrastructure.DataAccess.EntityConfigs
{
    public class ItemCategoryConfig : IEntityTypeConfiguration<ItemCategory>
    {
        public void Configure(EntityTypeBuilder<ItemCategory> builder)
        {
            builder.HasKey(x => new {x.CategoryId, x.ItemId});
        }
    }
}
