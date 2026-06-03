using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreDataBase.Configuration
{
    public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
    {
        public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
        {
            builder.ToTable("IdempotencyRecords").HasKey(a => a.Id);
            builder.Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Property(a => a.Key).IsRequired();
            builder.HasIndex(a => a.Key).IsUnique();
            builder.Property(a => a.StatusCode).IsRequired();
            builder.Property(a => a.Value).IsRequired();
            builder.Property(a => a.CreatedAt).IsRequired();
        }
    }
}
