using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreWebApi.Models;
using System.Reflection;
using System;

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
            builder.HasData(uploadData());
        }

        private Item[] uploadData()
        {
            return new Item[]
            {
                            new Item {Id=1,Name="math book",Price=200,StockQuantity=106,CategoryId=1},
                            new Item {Id=2,Name="english book",Price=300,StockQuantity= 201,CategoryId= 1},
                            new Item {Id=3,Name="arabic book", Price=250,StockQuantity= 160,CategoryId=1},
                            new Item {Id=4,Name="science book",Price=400,StockQuantity= 150,CategoryId= 1},
                            new Item {Id=5,Name="mobile",Price=2500,StockQuantity= 100,CategoryId= 2},
                            new Item {Id=6,Name="labtop",Price=3500,StockQuantity= 102,CategoryId= 2},
            };
        }
    }
}
