using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedPillarTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Same situation as Slides - the original content-seed migration was dropped when
            // the migration history got squashed. Reseed the 3 homepage pillars, bilingual.
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [Pillars])
                BEGIN
                    INSERT INTO [Pillars] ([Id], [TitleFr], [TitleEn], [TextFr], [TextEn], [ImageUrl], [Icon], [IsVisible], [Status], [DisplayOrder])
                    VALUES
                    (NEWID(), N'Un espace de concertations', N'A space for dialogue',
                        N'Un lieu de dialogue stratégique entre acteurs du développement international, diasporas, ONG et représentants publics.',
                        N'A place for strategic dialogue among international development actors, diaspora communities, NGOs and public representatives.',
                        'assets/images/concertations.jpeg', '01', 1, 'Published', 1),
                    (NEWID(), N'Un cadre de plaidoyer et d''actions citoyennes', N'A framework for advocacy and civic action',
                        N'Une plateforme qui met en avant les enjeux sociaux, économiques et humains afin de porter une voix collective.',
                        N'A platform that highlights social, economic and human issues to carry a collective voice.',
                        'assets/images/plaidoyer-actions-citoyennes.jpeg', '02', 1, 'Published', 2),
                    (NEWID(), N'Un espace de visibilité et de réseautage', N'A space for visibility and networking',
                        N'Un environnement propice aux rencontres, aux opportunités et à la mise en valeur des initiatives à fort impact.',
                        N'An environment conducive to encounters, opportunities and the showcasing of high-impact initiatives.',
                        'assets/images/visibilite-reseautage.jpeg', '03', 1, 'Published', 3);
                END
                """);

            migrationBuilder.Sql("UPDATE [Pillars] SET [TitleEn] = N'A space for dialogue', [TextEn] = N'A place for strategic dialogue among international development actors, diaspora communities, NGOs and public representatives.' WHERE [TitleFr] = N'Un espace de concertations';");
            migrationBuilder.Sql("UPDATE [Pillars] SET [TitleEn] = N'A framework for advocacy and civic action', [TextEn] = N'A platform that highlights social, economic and human issues to carry a collective voice.' WHERE [TitleFr] = N'Un cadre de plaidoyer et d''actions citoyennes';");
            migrationBuilder.Sql("UPDATE [Pillars] SET [TitleEn] = N'A space for visibility and networking', [TextEn] = N'An environment conducive to encounters, opportunities and the showcasing of high-impact initiatives.' WHERE [TitleFr] = N'Un espace de visibilité et de réseautage';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data-only migration - nothing to structurally roll back.
        }
    }
}
