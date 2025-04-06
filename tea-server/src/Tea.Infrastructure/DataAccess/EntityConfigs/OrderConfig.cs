using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tea.Domain.Entities;

namespace Tea.Infrastructure.DataAccess.EntityConfigs
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(o => o.AppUser)
                   .WithMany() 
                   .HasForeignKey(o => o.UserId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(o => o.CreatedBy)
                   .WithMany() 
                   .HasForeignKey(o => o.CreatedById)
                   .IsRequired(false) 
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.Property(o => o.PaymentType)
                   .HasConversion<string>(); 

            builder.Property(o => o.OrderStatus)
                   .HasConversion<string>();

            builder.Property(o => o.OrderType)
                   .HasConversion<string>();

            builder.Property(o => o.PaymentStatus)
                   .HasConversion<string>();
        }
    }
}
