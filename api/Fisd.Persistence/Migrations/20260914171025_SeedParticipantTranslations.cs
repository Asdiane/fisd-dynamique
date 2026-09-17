using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedParticipantTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM [Participants])
                BEGIN
                    INSERT INTO [Participants] ([Id], [NameFr], [NameEn], [DescriptionFr], [DescriptionEn], [IsVisible], [Status], [DisplayOrder])
                    VALUES
                    (NEWID(), N'Associations', N'Associations', N'Diasporas africaines, haïtiennes et communautaires.', N'African, Haitian and community diasporas.', 1, 'Published', 1),
                    (NEWID(), N'ONG', N'NGOs', N'Organismes du secteur du développement international.', N'Organizations in the international development sector.', 1, 'Published', 2),
                    (NEWID(), N'Universitaires', N'Academics', N'Québécois, Canadiens et autres acteurs dans le monde.', N'From Quebec, Canada and other actors around the world.', 1, 'Published', 3),
                    (NEWID(), N'Entrepreneurs', N'Entrepreneurs', N'Québécois, Canadiens et membres des diasporas africaines et haïtiennes.', N'From Quebec, Canada and members of the African and Haitian diasporas.', 1, 'Published', 4),
                    (NEWID(), N'Diplomates', N'Diplomats', N'Du Canada et des pays de l''Afrique de l''Ouest francophone.', N'From Canada and French-speaking West African countries.', 1, 'Published', 5),
                    (NEWID(), N'Pouvoirs publics', N'Public authorities', N'Ministres, députés, élus locaux et leurs représentants.', N'Ministers, members of parliament, local elected officials and their representatives.', 1, 'Published', 6);
                END
                """);

            migrationBuilder.Sql("UPDATE [Participants] SET [NameEn] = N'Associations', [DescriptionEn] = N'African, Haitian and community diasporas.' WHERE [NameFr] = N'Associations';");
            migrationBuilder.Sql("UPDATE [Participants] SET [NameEn] = N'NGOs', [DescriptionEn] = N'Organizations in the international development sector.' WHERE [NameFr] = N'ONG';");
            migrationBuilder.Sql("UPDATE [Participants] SET [NameEn] = N'Academics', [DescriptionEn] = N'From Quebec, Canada and other actors around the world.' WHERE [NameFr] = N'Universitaires';");
            migrationBuilder.Sql("UPDATE [Participants] SET [NameEn] = N'Entrepreneurs', [DescriptionEn] = N'From Quebec, Canada and members of the African and Haitian diasporas.' WHERE [NameFr] = N'Entrepreneurs';");
            migrationBuilder.Sql("UPDATE [Participants] SET [NameEn] = N'Diplomats', [DescriptionEn] = N'From Canada and French-speaking West African countries.' WHERE [NameFr] = N'Diplomates';");
            migrationBuilder.Sql("UPDATE [Participants] SET [NameEn] = N'Public authorities', [DescriptionEn] = N'Ministers, members of parliament, local elected officials and their representatives.' WHERE [NameFr] = N'Pouvoirs publics';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data-only migration - nothing to structurally roll back.
        }
    }
}
