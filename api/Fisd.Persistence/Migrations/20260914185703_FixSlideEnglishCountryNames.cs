using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixSlideEnglishCountryNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The original seed left the French country name inside the English fields -
            // the English name for "Côte d'Ivoire" is "Ivory Coast".
            migrationBuilder.Sql("UPDATE [Slides] SET [PlaceEn] = N'Ivory Coast' WHERE [PlaceEn] = N'Côte d''Ivoire';");
            migrationBuilder.Sql("UPDATE [Slides] SET [TextEn] = REPLACE([TextEn], N'Côte d''Ivoire', N'Ivory Coast') WHERE [TextEn] LIKE N'%Côte d''Ivoire%';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
