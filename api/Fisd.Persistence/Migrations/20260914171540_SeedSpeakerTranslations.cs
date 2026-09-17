using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedSpeakerTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Same situation as the other content tables - the original seed migration was
            // dropped in the earlier squash. Reseed the 14 FISD 2025 speakers, bilingual.
            // Names aren't translated, only role/title and bio. Depends on the 2025 Editions row
            // already existing (SeedEditionTranslations runs first).
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [Speakers]) AND EXISTS (SELECT 1 FROM [Editions] WHERE [Year] = 2025)
                BEGIN
                    DECLARE @edition2025 UNIQUEIDENTIFIER = (SELECT TOP 1 [Id] FROM [Editions] WHERE [Year] = 2025);

                    INSERT INTO [Speakers] ([Id], [EditionId], [Name], [RoleFr], [RoleEn], [ImageUrl], [DescriptionFr], [DescriptionEn], [IsVisible], [Status], [DisplayOrder])
                    VALUES
                    (NEWID(), @edition2025, N'Mahamat Tochi Chidi', N'Sénateur du Tchad, rapporteur 2e adjoint de la commission Défense, Sécurité et Souveraineté', N'Senator of Chad, 2nd deputy rapporteur of the Defense, Security and Sovereignty Committee', 'assets/images/intervenants/mahamat-tochi-chidi.jpg', N'Panéliste du FISD 2025, sénateur du Tchad.', N'FISD 2025 panelist, Senator of Chad.', 1, 'Published', 1),
                    (NEWID(), @edition2025, N'Bénie Kouyaté', N'Présidente de Bénie Foundation Inc.', N'President of Bénie Foundation Inc.', 'assets/images/intervenants/benie-kouyate.jpg', N'Panéliste du FISD 2025, présentée comme présidente de Bénie Foundation Inc.', N'FISD 2025 panelist, introduced as President of Bénie Foundation Inc.', 1, 'Published', 2),
                    (NEWID(), @edition2025, N'Hawa Barry Diallo', N'Présidente de Guinean Women Development', N'President of Guinean Women Development', 'assets/images/intervenants/hawa-barry-diallo.jpg', N'Panéliste du FISD 2025, présentée comme présidente de Guinean Women Development.', N'FISD 2025 panelist, introduced as President of Guinean Women Development.', 1, 'Published', 3),
                    (NEWID(), @edition2025, N'Pr. François Audet', N'Directeur de l''Observatoire canadien sur les crises et l''action humanitaires (OCCAH)', N'Director of the Canadian Observatory on Crises and Humanitarian Action (OCCAH)', 'assets/images/intervenants/francois-audet.jpg', N'Panéliste du FISD 2025 et directeur de l''OCCAH.', N'FISD 2025 panelist and director of OCCAH.', 1, 'Published', 4),
                    (NEWID(), @edition2025, N'Catherine Cauchon', N'Directrice des programmes / Terre Sans Frontières', N'Programs Director / Terre Sans Frontières', 'assets/images/intervenants/catherine-cauchon.jpg', N'Panéliste du FISD 2025, directrice des programmes chez Terre Sans Frontières.', N'FISD 2025 panelist, Programs Director at Terre Sans Frontières.', 1, 'Published', 5),
                    (NEWID(), @edition2025, N'Alpha Touré', N'Président du comité administratif de Jeunes Solidaires', N'Chair of the Jeunes Solidaires administrative committee', 'assets/images/intervenants/alpha-toure.jpg', N'Président du FISD 2025 et président du comité administratif de Jeunes Solidaires.', N'Chair of FISD 2025 and chair of the Jeunes Solidaires administrative committee.', 1, 'Published', 6),
                    (NEWID(), @edition2025, N'M. Seydou Togola', N'Directeur pays / Terre Sans Frontières - Mali', N'Country Director / Terre Sans Frontières - Mali', 'assets/images/intervenants/seydou-togola.jpg', N'Panéliste du FISD 2025, directeur pays pour Terre Sans Frontières au Mali.', N'FISD 2025 panelist, Country Director for Terre Sans Frontières in Mali.', 1, 'Published', 7),
                    (NEWID(), @edition2025, N'M. Sansy Kaba Diakité', N'Directeur de L''Harmattan Guinée et président-fondateur du Lions Club Conakry Bate', N'Director of L''Harmattan Guinée and founding president of the Lions Club Conakry Bate', 'assets/images/intervenants/sansy-kaba-diakite.jpg', N'Panéliste du FISD 2025, directeur de L''Harmattan Guinée et président-fondateur du Lions Club Conakry Bate.', N'FISD 2025 panelist, director of L''Harmattan Guinée and founding president of the Lions Club Conakry Bate.', 1, 'Published', 8),
                    (NEWID(), @edition2025, N'Mme Pauline Effa', N'Vice-présidente du Forum International de l''Économie Sociale et Solidaire', N'Vice-President of the International Forum on the Social and Solidarity Economy', 'assets/images/intervenants/pauline-effa.jpg', N'Panéliste du FISD 2025, vice-présidente du Forum International de l''Économie Sociale et Solidaire et cofondatrice du FORAESS.', N'FISD 2025 panelist, Vice-President of the International Forum on the Social and Solidarity Economy and co-founder of FORAESS.', 1, 'Published', 9),
                    (NEWID(), @edition2025, N'Dr. Paubert T. Mahatante', N'Ministre de la Pêche et de l''Économie bleue de Madagascar', N'Minister of Fisheries and Blue Economy of Madagascar', 'assets/images/intervenants/paubert-mahatante.jpg', N'Panéliste du FISD 2025, ministre de la Pêche et de l''Économie bleue de Madagascar.', N'FISD 2025 panelist, Minister of Fisheries and Blue Economy of Madagascar.', 1, 'Published', 10),
                    (NEWID(), @edition2025, N'Mme Michèle Asselin', N'Directrice générale - AQOCI', N'Executive Director - AQOCI', 'assets/images/intervenants/michele-asselin.jpg', N'Panéliste du FISD 2025, directrice générale de l''AQOCI.', N'FISD 2025 panelist, Executive Director of AQOCI.', 1, 'Published', 11),
                    (NEWID(), @edition2025, N'M. Michel Filion, Ph.D.', N'Spécialiste des politiques et des finances publiques', N'Public policy and public finance specialist', 'assets/images/intervenants/michel-filion.jpg', N'Panéliste du FISD 2025, spécialiste des politiques et des finances publiques.', N'FISD 2025 panelist, public policy and public finance specialist.', 1, 'Published', 12),
                    (NEWID(), @edition2025, N'Bigsoul 224', N'Créateur de contenu', N'Content creator', 'assets/images/intervenants/bigsoul-224.jpg', N'Panéliste du FISD 2025 et créateur de contenu.', N'FISD 2025 panelist and content creator.', 1, 'Published', 13),
                    (NEWID(), @edition2025, N'Sénateur Abdallah Darkallah Sidi', N'Rapporteur 1er adjoint à la commission des Affaires étrangères et des Tchadiens de l''étranger / Sénat du Tchad', N'1st deputy rapporteur of the Foreign Affairs and Chadians Abroad Committee / Senate of Chad', 'assets/images/intervenants/abdallah-darkallah-sidi.jpg', N'Panéliste du FISD 2025, sénateur et rapporteur 1er adjoint au Sénat du Tchad.', N'FISD 2025 panelist, senator and 1st deputy rapporteur at the Senate of Chad.', 1, 'Published', 14);
                END
                """);

            // If these 14 speakers already existed under these names (e.g. from the French
            // fallback the previous migration applied), overwrite Role/Description with the
            // real English translations above instead of leaving the French-language fallback.
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'Senator of Chad, 2nd deputy rapporteur of the Defense, Security and Sovereignty Committee', [DescriptionEn] = N'FISD 2025 panelist, Senator of Chad.' WHERE [Name] = N'Mahamat Tochi Chidi';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'President of Bénie Foundation Inc.', [DescriptionEn] = N'FISD 2025 panelist, introduced as President of Bénie Foundation Inc.' WHERE [Name] = N'Bénie Kouyaté';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'President of Guinean Women Development', [DescriptionEn] = N'FISD 2025 panelist, introduced as President of Guinean Women Development.' WHERE [Name] = N'Hawa Barry Diallo';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'Director of the Canadian Observatory on Crises and Humanitarian Action (OCCAH)', [DescriptionEn] = N'FISD 2025 panelist and director of OCCAH.' WHERE [Name] = N'Pr. François Audet';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'Programs Director / Terre Sans Frontières', [DescriptionEn] = N'FISD 2025 panelist, Programs Director at Terre Sans Frontières.' WHERE [Name] = N'Catherine Cauchon';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'Chair of the Jeunes Solidaires administrative committee', [DescriptionEn] = N'Chair of FISD 2025 and chair of the Jeunes Solidaires administrative committee.' WHERE [Name] = N'Alpha Touré';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'Country Director / Terre Sans Frontières - Mali', [DescriptionEn] = N'FISD 2025 panelist, Country Director for Terre Sans Frontières in Mali.' WHERE [Name] = N'M. Seydou Togola';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'Director of L''Harmattan Guinée and founding president of the Lions Club Conakry Bate', [DescriptionEn] = N'FISD 2025 panelist, director of L''Harmattan Guinée and founding president of the Lions Club Conakry Bate.' WHERE [Name] = N'M. Sansy Kaba Diakité';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'Vice-President of the International Forum on the Social and Solidarity Economy', [DescriptionEn] = N'FISD 2025 panelist, Vice-President of the International Forum on the Social and Solidarity Economy and co-founder of FORAESS.' WHERE [Name] = N'Mme Pauline Effa';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'Minister of Fisheries and Blue Economy of Madagascar', [DescriptionEn] = N'FISD 2025 panelist, Minister of Fisheries and Blue Economy of Madagascar.' WHERE [Name] = N'Dr. Paubert T. Mahatante';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'Executive Director - AQOCI', [DescriptionEn] = N'FISD 2025 panelist, Executive Director of AQOCI.' WHERE [Name] = N'Mme Michèle Asselin';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'Public policy and public finance specialist', [DescriptionEn] = N'FISD 2025 panelist, public policy and public finance specialist.' WHERE [Name] = N'M. Michel Filion, Ph.D.';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'Content creator', [DescriptionEn] = N'FISD 2025 panelist and content creator.' WHERE [Name] = N'Bigsoul 224';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = N'1st deputy rapporteur of the Foreign Affairs and Chadians Abroad Committee / Senate of Chad', [DescriptionEn] = N'FISD 2025 panelist, senator and 1st deputy rapporteur at the Senate of Chad.' WHERE [Name] = N'Sénateur Abdallah Darkallah Sidi';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data-only migration - nothing to structurally roll back.
        }
    }
}
