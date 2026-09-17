using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Security.Configuration
{
    public class AdminUserInvitationEntityConfiguration : IEntityTypeConfiguration<AdminUserInvitationEntity>
    {
        public void Configure(EntityTypeBuilder<AdminUserInvitationEntity> builder)
        {
            builder.Property(i => i.Email).HasMaxLength(320).IsRequired();
            builder.Property(i => i.TokenHash).HasMaxLength(64).IsRequired();
            builder.HasIndex(i => i.TokenHash).IsUnique();
            builder.HasIndex(i => i.Email);
        }
    }
}
