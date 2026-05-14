using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreWebApi.Models;

namespace StoreWebApi.Configuration
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets").HasKey(a => a.Id);
            builder.Property(a=>a.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(a=>a.UserEmail).IsRequired();
            builder.Property(a=>a.Balance).IsRequired();
            builder.Property(a=>a.Currency).IsRequired();
            builder.HasData(uploadData());
        }

        private Wallet[] uploadData()
        {
            var walletData = new Wallet[]
            {
                new Wallet { Id = 1, UserEmail = "saad@gmail.com", Balance = 5000 ,Currency="USD $"},
                new Wallet { Id = 2, UserEmail = "ahmed@gmail.com", Balance = 5000 , Currency="USD $"},
            };
            return walletData;
        }
    }
}
