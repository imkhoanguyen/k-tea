using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tea.Domain.Entities;

namespace Tea.Infrastructure.DataAccess.EntityConfigs
{
    public class SizeConfig : IEntityTypeConfiguration<Size>
    {
        public void Configure(EntityTypeBuilder<Size> builder)
        {
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
            builder.Property(x => x.NewPrice).HasColumnType("decimal(18,2)");
        }
    }
}
