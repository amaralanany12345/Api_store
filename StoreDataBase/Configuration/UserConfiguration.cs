using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDomain.Enums;
using StoreDomain.Models;

namespace StoreDataBase.Configuration
{
    public class UserConfiguration:IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users").HasKey(a => a.Id);
            builder.Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(A => A.UserName).IsRequired();
            builder.Property(a => a.Email).IsRequired();
            builder.HasIndex(a => a.Email).IsUnique();
            builder.Property(A => A.PasswordHash).IsRequired();
            builder.Property(A => A.Role).IsRequired();
            builder.Property(A => A.CreatedAt).IsRequired();
            //builder.HasData(uploadData());

        }

        private User[] uploadData()
        {
            return new User[]
            {
                new User { Id = 1, UserName = "ammar", Email = "ammar@gmail.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123"),
                    Role = UserRole.Admin.ToString(),CreatedAt = new DateTime(2026, 5, 11)},
                new User { Id = 2, UserName = "saad", Email = "saad@gmail.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("saad123"),
                    Role = UserRole.Customer.ToString(),CreatedAt = new DateTime(2026, 5, 11)},
                new User { Id = 3, UserName = "ahmed", Email = "ahmed@gmail.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("ahmed123"),
                    Role = UserRole.Customer.ToString(),CreatedAt = new DateTime(2026, 5, 11)},
                new User { Id = 4, UserName = "abdo", Email = "abdo@gmail.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("abdo123"),
                    Role = UserRole.Admin.ToString(),CreatedAt = new DateTime(2026, 5, 11)},
            };
        }
    }
}
