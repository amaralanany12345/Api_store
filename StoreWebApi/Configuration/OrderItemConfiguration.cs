using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreWebApi.Models;

namespace StoreWebApi.Configuration
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("orderItems").HasKey(a=> new {a.OrderId,a.ItemId});
            builder.HasOne(a=>a.Order).WithMany(a=>a.OrderItems).HasForeignKey(a=>a.OrderId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a=>a.Item).WithMany(a=>a.OrderItems).HasForeignKey(a=>a.ItemId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
