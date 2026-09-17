using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBilingualPillarFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Pillars",
                newName: "TitleFr");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Pillars",
                newName: "TextFr");

            migrationBuilder.AddColumn<string>(
                name: "TextEn",
                table: "Pillars",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "Pillars",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            // Same safety net as the Slides migration - backfill with the French text rather
            // than leaving English blank for any row that already existed in this environment.
            migrationBuilder.Sql("UPDATE [Pillars] SET [TitleEn] = [TitleFr] WHERE [TitleEn] = '';");
            migrationBuilder.Sql("UPDATE [Pillars] SET [TextEn] = [TextFr] WHERE [TextEn] = '';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextEn",
                table: "Pillars");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "Pillars");

            migrationBuilder.RenameColumn(
                name: "TitleFr",
                table: "Pillars",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "TextFr",
                table: "Pillars",
                newName: "Text");
        }
    }
}
