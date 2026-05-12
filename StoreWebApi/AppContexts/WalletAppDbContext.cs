using Microsoft.EntityFrameworkCore;
using StoreWebApi.Configuration;
using StoreWebApi.Models;

namespace StoreWebApi.zAppContexts
{
    public class WalletAppDbContext : DbContext
    {
        public DbSet<Wallet> Wallets { get; set; }
        public WalletAppDbContext(DbContextOptions<WalletAppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new WalletConfiguration());
        }
    }
}
