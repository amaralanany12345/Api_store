using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreWebApi.Models;

namespace StoreWebApi.Configuration
{
    public class CustomerConfiguration 
    {
        //public void Configure(EntityTypeBuilder<Customer> builder)
        //{
        //    //builder.ToTable("Customers").HasKey(a => a.Id);
        //    //builder.Property(a=>a.Id).IsRequired().ValueGeneratedOnAdd();
        //    //builder.Property(A => A.UserName).IsRequired();
        //    //builder.Property(a => a.Email).IsRequired();
        //    //builder.HasIndex(a => a.Email).IsUnique();
        //    //builder.Property(A => A.PasswordHash).IsRequired();
        //    //builder.Property(A => A.Role).IsRequired();
        //    //builder.Property(A => A.CreatedAt).IsRequired();
        //    //builder.Property(a=>a.Balance).IsRequired();
        //    //builder.HasMany(a=>a.Orders).WithOne(a=>a.Customer).HasForeignKey(a=>a.CustomerId);
        //}
    }
}
