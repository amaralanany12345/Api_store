using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreWebApi.Models;

namespace StoreWebApi.Configuration
{
    public class ExternalLogConfiguration : IEntityTypeConfiguration<ExternalLog>
    {
        public void Configure(EntityTypeBuilder<ExternalLog> builder)
        {
            builder.ToTable("ExternalLogs").HasKey(a=>a.Id);
            builder.Property(a=>a.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(a=>a.UserEmail).IsRequired();
            builder.Property(a=>a.Provider).IsRequired();
            builder.Property(a=>a.Operation).IsRequired();
            builder.Property(a=>a.Status).IsRequired();
            builder.Property(a=>a.ResponsePayload).IsRequired();
            builder.Property(a=>a.RequestPayload).IsRequired();
            builder.Property(a=>a.CreatedAt).IsRequired();
        }
    }
}
