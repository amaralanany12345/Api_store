using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreWebApi.Models;

namespace StoreWebApi.Configuration
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Items").HasKey(a=>a.Id);
            builder.Property(a=>a.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(a=>a.Name).IsRequired();
            builder.Property(a=>a.Price).IsRequired();
            builder.Property(a=>a.StockQuantity).IsRequired();
            builder.HasOne(a=>a.Category).WithMany(a=>a.Items).HasForeignKey(a=>a.CategoryId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
