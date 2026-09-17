using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Diagnostics.Configuration
{
    public class ErrorLogEntityConfiguration : IEntityTypeConfiguration<ErrorLogEntity>
    {
        public void Configure(EntityTypeBuilder<ErrorLogEntity> builder)
        {
            builder.ToTable("ErrorLog");
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.Level).HasMaxLength(20).IsRequired();
            builder.Property(e => e.Message).IsRequired();
            builder.Property(e => e.Logger).HasMaxLength(300);
            builder.Property(e => e.RequestMethod).HasMaxLength(10);
            builder.Property(e => e.RequestUrl).HasMaxLength(2000);
            builder.Property(e => e.UserEmail).HasMaxLength(320);
            builder.HasIndex(e => e.Logged);
            builder.HasIndex(e => e.Level);
        }
    }
}
