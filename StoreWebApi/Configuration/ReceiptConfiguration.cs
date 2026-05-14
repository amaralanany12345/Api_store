using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreWebApi.Models;

namespace StoreWebApi.Configuration
{
    public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
    {
        public void Configure(EntityTypeBuilder<Receipt> builder)
        {
            builder.ToTable("Receipts").HasKey(a=>a.Id);
            builder.Property(A=>A.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(a=>a.TotalAmount).IsRequired();
            builder.Property(a=>a.CreatedAt).IsRequired();
            builder.HasOne(a =>a.Order).WithOne(a=>a.Receipt).HasForeignKey<Receipt>(a=>a.orderId);
        }
    }
}
