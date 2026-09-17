using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedEngagementActionTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [EngagementActions])
                BEGIN
                    INSERT INTO [EngagementActions] ([Id], [Anchor], [TitleFr], [TitleEn], [TextFr], [TextEn], [Icon], [ImageUrl], [Link], [CtaFr], [CtaEn], [DetailFr], [DetailEn], [IsVisible], [Status], [DisplayOrder])
                    VALUES
                    (NEWID(), N'partenaire', N'Devenir partenaire', N'Become a partner',
                        N'Associer votre organisation au FISD et construire une collaboration visible, durable et utile.',
                        N'Associate your organization with FISD and build a visible, lasting and meaningful collaboration.',
                        N'PT', 'assets/images/agir-avec-nous.jpg', 'https://docs.google.com/forms/d/1wj2F1RXTcSjzTfX-KGPXceh_wCg-hfQ3LZ5prZvRlxw/viewform?edit_requested=true',
                        N'Remplir le formulaire partenaire', N'Fill out the partner form',
                        N'Pour les institutions, organisations, entreprises ou structures qui souhaitent soutenir le forum.',
                        N'For institutions, organizations, businesses or structures wishing to support the forum.', 1, 'Published', 1),
                    (NEWID(), N'stand', N'Prendre un stand', N'Get a booth',
                        N'Présenter vos initiatives, projets, produits ou services dans un espace dédié pendant le forum.',
                        N'Showcase your initiatives, projects, products or services in a dedicated space during the forum.',
                        N'ST', 'assets/images/fisd-est-visibilite.jpg', 'https://docs.google.com/forms/d/e/1FAIpQLSep06rVkgrzSKk0sCVjhix5B0cRHw_wkrbdQvxIbagHcf4zxA/viewform',
                        N'Demander un stand', N'Request a booth',
                        N'Pour les exposants qui veulent rencontrer le public, les partenaires et les participants.',
                        N'For exhibitors who want to meet the public, partners and participants.', 1, 'Published', 2),
                    (NEWID(), N'benevole', N'Devenir bénévole', N'Become a volunteer',
                        N'Contribuer à l''accueil, à l''organisation et au bon déroulement des activités du FISD.',
                        N'Help with welcoming guests, organizing and running FISD''s activities smoothly.',
                        N'BV', 'assets/images/agir.jpeg', 'https://docs.google.com/forms/d/e/1FAIpQLSdOfyKT8_KsmNQYFPWtLFssvd-Yb9d6iKAu7hOPC6kC07qcJQ/viewform',
                        N'Rejoindre l''équipe bénévole', N'Join the volunteer team',
                        N'Pour les personnes qui veulent donner du temps et participer concrètement à l''événement.',
                        N'For people who want to give their time and get hands-on with the event.', 1, 'Published', 3);
                END
                """);

            migrationBuilder.Sql("UPDATE [EngagementActions] SET [TitleEn] = N'Become a partner', [TextEn] = N'Associate your organization with FISD and build a visible, lasting and meaningful collaboration.', [CtaEn] = N'Fill out the partner form', [DetailEn] = N'For institutions, organizations, businesses or structures wishing to support the forum.' WHERE [Anchor] = N'partenaire';");
            migrationBuilder.Sql("UPDATE [EngagementActions] SET [TitleEn] = N'Get a booth', [TextEn] = N'Showcase your initiatives, projects, products or services in a dedicated space during the forum.', [CtaEn] = N'Request a booth', [DetailEn] = N'For exhibitors who want to meet the public, partners and participants.' WHERE [Anchor] = N'stand';");
            migrationBuilder.Sql("UPDATE [EngagementActions] SET [TitleEn] = N'Become a volunteer', [TextEn] = N'Help with welcoming guests, organizing and running FISD''s activities smoothly.', [CtaEn] = N'Join the volunteer team', [DetailEn] = N'For people who want to give their time and get hands-on with the event.' WHERE [Anchor] = N'benevole';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data-only migration - nothing to structurally roll back.
        }
    }
}
