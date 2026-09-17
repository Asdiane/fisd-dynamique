using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBilingualEditionFieldsAndCurrent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Editions",
                newName: "TitleFr");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Editions",
                newName: "TextFr");

            migrationBuilder.RenameColumn(
                name: "LocationLabel",
                table: "Editions",
                newName: "LocationLabelFr");

            migrationBuilder.RenameColumn(
                name: "Badge",
                table: "Editions",
                newName: "BadgeFr");

            migrationBuilder.AddColumn<string>(
                name: "BadgeEn",
                table: "Editions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "Editions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LocationLabelEn",
                table: "Editions",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextEn",
                table: "Editions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "Editions",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            // Same safety net as Slides/Pillars - backfill with the French text rather than
            // leaving English blank for any row that already existed in this environment.
            migrationBuilder.Sql("UPDATE [Editions] SET [BadgeEn] = [BadgeFr] WHERE [BadgeEn] = '';");
            migrationBuilder.Sql("UPDATE [Editions] SET [TitleEn] = [TitleFr] WHERE [TitleEn] = '';");
            migrationBuilder.Sql("UPDATE [Editions] SET [TextEn] = [TextFr] WHERE [TextEn] = '';");
            migrationBuilder.Sql("UPDATE [Editions] SET [LocationLabelEn] = [LocationLabelFr] WHERE [LocationLabelEn] IS NULL AND [LocationLabelFr] IS NOT NULL;");
            // If exactly one edition already existed before this migration, make it current by
            // default so the site has something to point "current edition" content at rather
            // than nothing, until an admin picks explicitly.
            migrationBuilder.Sql("""
                IF (SELECT COUNT(*) FROM [Editions]) = 1
                BEGIN
                    UPDATE [Editions] SET [IsCurrent] = 1;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BadgeEn",
                table: "Editions");

            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "Editions");

            migrationBuilder.DropColumn(
                name: "LocationLabelEn",
                table: "Editions");

            migrationBuilder.DropColumn(
                name: "TextEn",
                table: "Editions");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "Editions");

            migrationBuilder.RenameColumn(
                name: "TitleFr",
                table: "Editions",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "TextFr",
                table: "Editions",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "LocationLabelFr",
                table: "Editions",
                newName: "LocationLabel");

            migrationBuilder.RenameColumn(
                name: "BadgeFr",
                table: "Editions",
                newName: "Badge");
        }
    }
}
