using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBilingualSlideFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Slides",
                newName: "TitleFr");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Slides",
                newName: "TextFr");

            migrationBuilder.RenameColumn(
                name: "Place",
                table: "Slides",
                newName: "PlaceFr");

            migrationBuilder.RenameColumn(
                name: "Kicker",
                table: "Slides",
                newName: "KickerFr");

            migrationBuilder.AddColumn<string>(
                name: "KickerEn",
                table: "Slides",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlaceEn",
                table: "Slides",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextEn",
                table: "Slides",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "Slides",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            // Any row that already existed (this environment's data, not the fixture this repo
            // ships in its own migrations) just lost its only English text to a blank default -
            // backfill with the French text instead of leaving it empty. The follow-up
            // SeedSlideTranslations migration then overwrites the 5 known slides with real
            // English copy by matching on their (stable) French title.
            migrationBuilder.Sql("UPDATE [Slides] SET [KickerEn] = [KickerFr] WHERE [KickerEn] = '';");
            migrationBuilder.Sql("UPDATE [Slides] SET [TitleEn] = [TitleFr] WHERE [TitleEn] = '';");
            migrationBuilder.Sql("UPDATE [Slides] SET [TextEn] = [TextFr] WHERE [TextEn] = '';");
            migrationBuilder.Sql("UPDATE [Slides] SET [PlaceEn] = [PlaceFr] WHERE [PlaceEn] IS NULL AND [PlaceFr] IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KickerEn",
                table: "Slides");

            migrationBuilder.DropColumn(
                name: "PlaceEn",
                table: "Slides");

            migrationBuilder.DropColumn(
                name: "TextEn",
                table: "Slides");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "Slides");

            migrationBuilder.RenameColumn(
                name: "TitleFr",
                table: "Slides",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "TextFr",
                table: "Slides",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "PlaceFr",
                table: "Slides",
                newName: "Place");

            migrationBuilder.RenameColumn(
                name: "KickerFr",
                table: "Slides",
                newName: "Kicker");
        }
    }
}
