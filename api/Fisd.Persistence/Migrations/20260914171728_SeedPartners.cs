using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedPartners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Same situation as the other content tables - the original seed migration was
            // dropped in the earlier squash. Partner/organization names aren't translated (an
            // org's name is the same in both languages), so no Fr/En split here - just reseed.
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [Partners])
                BEGIN
                    INSERT INTO [Partners] ([Id], [Name], [Label], [LogoUrl], [IsVisible], [Status], [DisplayOrder])
                    VALUES
                    (NEWID(), N'UNCCIAS Union Nationale des Chambres de Commerce, d''Industrie et d''Agriculture du Sénégal', N'UNCCIAS', 'assets/logos/partenaires/unccias-horizontal.jpeg', 1, 'Published', 1),
                    (NEWID(), N'UNCCIAS Union Nationale des Chambres de Commerce, d''Industrie et d''Agriculture du Sénégal', N'UNCCIAS', 'assets/logos/partenaires/unccias-vertical.jpeg', 1, 'Published', 2),
                    (NEWID(), N'Ville de Montréal', NULL, 'assets/logos/partenaires/montreal.jpg', 1, 'Published', 3),
                    (NEWID(), N'Gouvernement du Québec', NULL, 'assets/logos/partenaires/quebec.png', 1, 'Published', 4),
                    (NEWID(), N'FISIQ', NULL, 'assets/logos/partenaires/fisiq.png', 1, 'Published', 5),
                    (NEWID(), N'CECI', NULL, 'assets/logos/partenaires/ceci.png', 1, 'Published', 6),
                    (NEWID(), N'Educonnexion', NULL, 'assets/logos/partenaires/educonnexion.png', 1, 'Published', 7),
                    (NEWID(), N'Village Monde', NULL, 'assets/logos/partenaires/village-monde.png', 1, 'Published', 8),
                    (NEWID(), N'SUCO', NULL, 'assets/logos/partenaires/suco.png', 1, 'Published', 9),
                    (NEWID(), N'Terre Sans Frontières', NULL, 'assets/logos/partenaires/terre-sans-frontieres.png', 1, 'Published', 10),
                    (NEWID(), N'Coopération Canada', NULL, 'assets/logos/partenaires/cooperation-canada.png', 1, 'Published', 11),
                    (NEWID(), N'AQOCI', NULL, 'assets/logos/partenaires/aqoci.png', 1, 'Published', 12),
                    (NEWID(), N'AEOC', NULL, 'assets/logos/partenaires/aeoc.png', 1, 'Published', 13),
                    (NEWID(), N'UQAM IEIM', NULL, 'assets/logos/partenaires/uqam-ieim.png', 1, 'Published', 14),
                    (NEWID(), N'Ad Alefa Diaspora', NULL, 'assets/logos/partenaires/ad-alefa-diaspora.png', 1, 'Published', 15),
                    (NEWID(), N'Espace Afrique', NULL, 'assets/logos/partenaires/espace-afrique.png', 1, 'Published', 16),
                    (NEWID(), N'ComDev Africa', NULL, 'assets/logos/partenaires/comdev-africa.png', 1, 'Published', 17),
                    (NEWID(), N'Guinean Women Development Foundation', NULL, 'assets/logos/partenaires/guinean-women.png', 1, 'Published', 18);
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
