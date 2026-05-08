using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreWebApi.Models;

namespace StoreWebApi.Configuration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshToken").HasKey(a=>a.Id);
            builder.Property(a=>a.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(a=>a.Token).IsRequired();
            builder.Property(a=>a.CreatedAt).IsRequired();
            builder.Property(a=>a.ExpiredAt).IsRequired();
            builder.HasOne(a=>a.User).WithMany(a=>a.RefreshTokens).HasForeignKey(a=>a.UserId);
        }
    }
}
