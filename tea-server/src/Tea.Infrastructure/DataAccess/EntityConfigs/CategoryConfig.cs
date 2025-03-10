using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tea.Domain.Entities;

namespace Tea.Infrastructure.DataAccess.EntityConfigs
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasMany(x => x.Children) // Một Parent có nhiều Children
            .WithOne(x => x.Parent)     // Một Children có một Parent
            .HasForeignKey(x => x.ParentId) // Khóa ngoại là ParentId
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
