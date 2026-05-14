using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreWebApi.Models;

namespace StoreWebApi.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories").HasKey(a => a.Id);
            builder.Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(a => a.Name).IsRequired();
            builder.Property(a => a.Description).IsRequired();
            builder.HasData(uploadData());
        }

        private Category[] uploadData()
        {
            return new Category[]
            {
                new Category { Id = 1,Name="Books",Description="Books description" },
                new Category { Id = 2,Name="Electronics",Description="Electronics description" },
            };
        }
    }
}
