using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedEditionTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Same situation as Slides/Pillars - the original content-seed migration was dropped
            // when the migration history got squashed. Reseed the 2 known editions, bilingual,
            // with 2026 (the upcoming one) marked current so the homepage stops defaulting to
            // whichever row happens to come back first.
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [Editions])
                BEGIN
                    INSERT INTO [Editions] ([Id], [Year], [BadgeFr], [BadgeEn], [TitleFr], [TitleEn], [TextFr], [TextEn], [StartDate], [EndDate], [LocationLabelFr], [LocationLabelEn], [TicketingUrl], [IsVisible], [Status], [IsCurrent])
                    VALUES
                    (NEWID(), 2025, N'Édition 2025', N'2025 Edition',
                        N'Programme officiel de l''édition 2025', N'Official 2025 edition program',
                        N'Préparez-vous à vivre 2 jours d''immersion transformative, suivis d''activités post-forum exclusives, conçues pour maximiser les retombées stratégiques de votre participation au FISD. La prochaine édition se tiendra les 5 et 6 décembre 2025 à Montréal, au Canada.',
                        N'Get ready for 2 days of transformative immersion, followed by exclusive post-forum activities designed to maximize the strategic impact of your participation in FISD. The next edition will take place on December 5 and 6, 2025 in Montreal, Canada.',
                        '2025-12-05', '2025-12-06', N'Montréal, Canada', N'Montreal, Canada', NULL, 1, 'Published', 0),
                    (NEWID(), 2026, N'Édition 2026', N'2026 Edition',
                        N'Rendez-vous à Montréal', N'See you in Montreal',
                        N'L''édition 2026 du FISD se déroulera les 26, 27 et 28 novembre 2026 à Montréal, au Canada. Préparez-vous à vivre 3 jours d''immersion transformative, suivis d''activités post-forum exclusives, conçues pour maximiser les retombées stratégiques de votre participation au FISD. Le lieu sera confirmé prochainement.',
                        N'The 2026 edition of FISD will take place on November 26, 27 and 28, 2026 in Montreal, Canada. Get ready for 3 days of transformative immersion, followed by exclusive post-forum activities designed to maximize the strategic impact of your participation in FISD. The venue will be confirmed soon.',
                        '2026-11-26', '2026-11-28', N'Montréal, Canada (lieu à confirmer)', N'Montreal, Canada (venue to be confirmed)', NULL, 1, 'Published', 1);
                END
                """);

            migrationBuilder.Sql("UPDATE [Editions] SET [BadgeEn] = N'2025 Edition', [TitleEn] = N'Official 2025 edition program', [TextEn] = N'Get ready for 2 days of transformative immersion, followed by exclusive post-forum activities designed to maximize the strategic impact of your participation in FISD. The next edition will take place on December 5 and 6, 2025 in Montreal, Canada.' WHERE [Year] = 2025;");
            migrationBuilder.Sql("UPDATE [Editions] SET [BadgeEn] = N'2026 Edition', [TitleEn] = N'See you in Montreal', [TextEn] = N'The 2026 edition of FISD will take place on November 26, 27 and 28, 2026 in Montreal, Canada. Get ready for 3 days of transformative immersion, followed by exclusive post-forum activities designed to maximize the strategic impact of your participation in FISD. The venue will be confirmed soon.' WHERE [Year] = 2026;");

            // Make sure exactly the 2026 edition ends up current if both rows already existed
            // under these years from before this migration (rather than leaving whatever the
            // earlier "single-row" fallback picked, or none at all).
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM [Editions] WHERE [Year] = 2026)
                BEGIN
                    UPDATE [Editions] SET [IsCurrent] = 0 WHERE [Year] <> 2026;
                    UPDATE [Editions] SET [IsCurrent] = 1 WHERE [Year] = 2026;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data-only migration - nothing to structurally roll back.
        }
    }
}
