using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBilingualEngagementActionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // EF's auto-generated rename heuristic matched the old "Detail" column to the new
            // "TextEn" column instead of "DetailFr" (both are similarly-shaped nvarchar columns,
            // and it has no way to know they're semantically unrelated) - that would have
            // silently moved existing Detail content into TextEn and left DetailFr/DetailEn both
            // empty. Rewritten by hand below to rename each column to its correct *Fr twin.
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "EngagementActions",
                newName: "TitleFr");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "EngagementActions",
                newName: "TextFr");

            migrationBuilder.RenameColumn(
                name: "Detail",
                table: "EngagementActions",
                newName: "DetailFr");

            migrationBuilder.RenameColumn(
                name: "Cta",
                table: "EngagementActions",
                newName: "CtaFr");

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "EngagementActions",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TextEn",
                table: "EngagementActions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DetailEn",
                table: "EngagementActions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CtaEn",
                table: "EngagementActions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            // Same safety net as the other content tables - backfill with the French text
            // rather than leaving English blank for any row that already existed.
            migrationBuilder.Sql("UPDATE [EngagementActions] SET [TitleEn] = [TitleFr] WHERE [TitleEn] = '';");
            migrationBuilder.Sql("UPDATE [EngagementActions] SET [TextEn] = [TextFr] WHERE [TextEn] = '';");
            migrationBuilder.Sql("UPDATE [EngagementActions] SET [DetailEn] = [DetailFr] WHERE [DetailEn] = '';");
            migrationBuilder.Sql("UPDATE [EngagementActions] SET [CtaEn] = [CtaFr] WHERE [CtaEn] = '';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "EngagementActions");

            migrationBuilder.DropColumn(
                name: "TextEn",
                table: "EngagementActions");

            migrationBuilder.DropColumn(
                name: "DetailEn",
                table: "EngagementActions");

            migrationBuilder.DropColumn(
                name: "CtaEn",
                table: "EngagementActions");

            migrationBuilder.RenameColumn(
                name: "TitleFr",
                table: "EngagementActions",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "TextFr",
                table: "EngagementActions",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "DetailFr",
                table: "EngagementActions",
                newName: "Detail");

            migrationBuilder.RenameColumn(
                name: "CtaFr",
                table: "EngagementActions",
                newName: "Cta");
        }
    }
}
