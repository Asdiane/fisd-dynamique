using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBilingualTestimonialFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Testimonials",
                newName: "ContentFr");

            migrationBuilder.RenameColumn(
                name: "AuthorRole",
                table: "Testimonials",
                newName: "AuthorRoleFr");

            migrationBuilder.AddColumn<string>(
                name: "AuthorRoleEn",
                table: "Testimonials",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentEn",
                table: "Testimonials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE [Testimonials] SET [AuthorRoleEn] = [AuthorRoleFr] WHERE [AuthorRoleEn] IS NULL AND [AuthorRoleFr] IS NOT NULL;");
            migrationBuilder.Sql("UPDATE [Testimonials] SET [ContentEn] = [ContentFr] WHERE [ContentEn] = '';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorRoleEn",
                table: "Testimonials");

            migrationBuilder.DropColumn(
                name: "ContentEn",
                table: "Testimonials");

            migrationBuilder.RenameColumn(
                name: "ContentFr",
                table: "Testimonials",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "AuthorRoleFr",
                table: "Testimonials",
                newName: "AuthorRole");
        }
    }
}
