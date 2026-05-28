using Microsoft.EntityFrameworkCore;
using StoreDataBase.Configuration;
using StoreDomain.Models;

namespace StoreDataBase.AppContexts
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
