using System.Text.Json;
using Fisd.Persistence.Common;
using Fisd.Persistence.Entities.Content;
using Fisd.Persistence.Entities.Diagnostics;
using Fisd.Persistence.Entities.Media;
using Fisd.Persistence.Entities.Security;
using Fisd.Persistence.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Fisd.Persistence
{
    public class FisdDbContext : DbContext
    {
        // Tables excluded from the audit trail: AuditTrail itself (would recurse), ErrorLog
        // (system-written, not an admin action), and the security-token tables (PasswordResetToken/
        // AdminUserInvitation/AdminPasskey) whose row-level values are hashes/credentials, not
        // meaningful content to review - their lifecycle isn't what "who changed what" is for.
        private static readonly HashSet<Type> ExcludedFromAudit =
        [
            typeof(Entities.Diagnostics.AuditTrailEntity),
            typeof(ErrorLogEntity),
            typeof(PasswordResetTokenEntity),
            typeof(AdminUserInvitationEntity),
            typeof(AdminPasskeyEntity)
        ];

        private readonly ICurrentAdminAccessor? _currentAdminAccessor;

        public FisdDbContext(DbContextOptions<FisdDbContext> options, ICurrentAdminAccessor? currentAdminAccessor = null) : base(options)
        {
            _currentAdminAccessor = currentAdminAccessor;
        }

        public DbSet<ArticleEntity> Articles => Set<ArticleEntity>();
        public DbSet<ContactEntity> Contacts => Set<ContactEntity>();
        public DbSet<SouvenirEntity> Souvenirs => Set<SouvenirEntity>();
        public DbSet<SouvenirPhotoEntity> SouvenirPhotos => Set<SouvenirPhotoEntity>();
        public DbSet<TestimonialEntity> Testimonials => Set<TestimonialEntity>();
        public DbSet<EditionEntity> Editions => Set<EditionEntity>();
        public DbSet<ProgramDayEntity> ProgramDays => Set<ProgramDayEntity>();
        public DbSet<ScheduleItemEntity> ScheduleItems => Set<ScheduleItemEntity>();
        public DbSet<AdminUserEntity> AdminUsers => Set<AdminUserEntity>();
        public DbSet<PasswordResetTokenEntity> PasswordResetTokens => Set<PasswordResetTokenEntity>();
        public DbSet<AdminUserInvitationEntity> AdminUserInvitations => Set<AdminUserInvitationEntity>();
        public DbSet<AdminPasskeyEntity> AdminPasskeys => Set<AdminPasskeyEntity>();
        public DbSet<SlideEntity> Slides => Set<SlideEntity>();
        public DbSet<PillarEntity> Pillars => Set<PillarEntity>();
        public DbSet<ParticipantEntity> Participants => Set<ParticipantEntity>();
        public DbSet<SpeakerEntity> Speakers => Set<SpeakerEntity>();
        public DbSet<PartnerEntity> Partners => Set<PartnerEntity>();
        public DbSet<EngagementActionEntity> EngagementActions => Set<EngagementActionEntity>();
        public DbSet<MediaFileEntity> MediaFiles => Set<MediaFileEntity>();
        public DbSet<SiteSettingsEntity> SiteSettings => Set<SiteSettingsEntity>();
        public DbSet<ErrorLogEntity> ErrorLogs => Set<ErrorLogEntity>();
        public DbSet<Entities.Diagnostics.AuditTrailEntity> AuditTrail => Set<Entities.Diagnostics.AuditTrailEntity>();
        public DbSet<Entities.Diagnostics.HelpArticleEntity> HelpArticles => Set<Entities.Diagnostics.HelpArticleEntity>();
        public DbSet<Entities.Diagnostics.TicketEntity> Tickets => Set<Entities.Diagnostics.TicketEntity>();
        public DbSet<Entities.Diagnostics.TicketAttachmentEntity> TicketAttachments => Set<Entities.Diagnostics.TicketAttachmentEntity>();
        public DbSet<Entities.Diagnostics.EmailSentHistoryEntity> EmailSentHistory => Set<Entities.Diagnostics.EmailSentHistoryEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FisdDbContext).Assembly);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess) =>
            SaveChangesWithAuditAsync(acceptAllChangesOnSuccess, async: false).GetAwaiter().GetResult();

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) =>
            await SaveChangesWithAuditAsync(acceptAllChangesOnSuccess, async: true, cancellationToken);

        private async Task<int> SaveChangesWithAuditAsync(bool acceptAllChangesOnSuccess, bool async, CancellationToken cancellationToken = default)
        {
            var pendingAudits = new List<(EntityEntry Entry, Entities.Diagnostics.AuditTrailEntity Audit)>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State is EntityState.Unchanged or EntityState.Detached || ExcludedFromAudit.Contains(entry.Entity.GetType()))
                {
                    continue;
                }

                var audit = new Entities.Diagnostics.AuditTrailEntity
                {
                    Timestamp = DateTimeOffset.UtcNow,
                    AdminUserId = GetActingAdminUserId(),
                    TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    OldValues = entry.State != EntityState.Added ? SerializeProperties(entry, useOriginalValues: true) : null,
                    NewValues = entry.State != EntityState.Deleted ? SerializeProperties(entry, useOriginalValues: false) : null
                };

                if (entry.State != EntityState.Added)
                {
                    audit.KeyValues = SerializeKey(entry);
                }

                pendingAudits.Add((entry, audit));
            }

            var result = async
                ? await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken)
                : base.SaveChanges(acceptAllChangesOnSuccess);

            if (pendingAudits.Count > 0)
            {
                foreach (var (entry, audit) in pendingAudits)
                {
                    // Added rows only get their generated key (and any DB-computed defaults) after
                    // the first SaveChanges - fill those in now instead of leaving KeyValues/NewValues
                    // reflecting the pre-insert, client-side-only state.
                    if (audit.KeyValues == null)
                    {
                        audit.KeyValues = SerializeKey(entry);
                    }
                    if (audit.Action == nameof(EntityState.Added))
                    {
                        audit.NewValues = SerializeProperties(entry, useOriginalValues: false);
                    }
                }

                AuditTrail.AddRange(pendingAudits.Select(p => p.Audit));
                if (async)
                {
                    await base.SaveChangesAsync(true, cancellationToken);
                }
                else
                {
                    base.SaveChanges(true);
                }
            }

            return result;
        }

        private Guid? GetActingAdminUserId() => _currentAdminAccessor?.GetCurrentAdminUserId();

        private static string SerializeKey(EntityEntry entry)
        {
            var keyProperties = entry.Metadata.FindPrimaryKey()?.Properties ?? [];
            var values = keyProperties.ToDictionary(p => p.Name, p => entry.Property(p.Name).CurrentValue);
            return JsonSerializer.Serialize(values);
        }

        // AdminUserEntity.PasswordHash isn't directly usable if leaked, but TwoFactorSecret is the
        // raw shared TOTP secret - if that ended up readable in an audit trail, whoever can view it
        // could generate valid codes for that admin, defeating 2FA entirely. Redact both rather than
        // rely on "it's just a hash" for the one that matters less.
        private static readonly HashSet<string> RedactedProperties = ["PasswordHash", "TwoFactorSecret"];

        private static string SerializeProperties(EntityEntry entry, bool useOriginalValues)
        {
            var values = entry.Properties.ToDictionary(
                p => p.Metadata.Name,
                p => RedactedProperties.Contains(p.Metadata.Name) ? (object?)"***redacted***" : (useOriginalValues ? p.OriginalValue : p.CurrentValue));
            return JsonSerializer.Serialize(values);
        }
    }
}
