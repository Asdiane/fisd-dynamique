using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBilingualSpeakerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Speakers",
                newName: "RoleFr");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Speakers",
                newName: "DescriptionFr");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Speakers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoleEn",
                table: "Speakers",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE [Speakers] SET [RoleEn] = [RoleFr] WHERE [RoleEn] = '';");
            migrationBuilder.Sql("UPDATE [Speakers] SET [DescriptionEn] = [DescriptionFr] WHERE [DescriptionEn] = '';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "RoleEn",
                table: "Speakers");

            migrationBuilder.RenameColumn(
                name: "RoleFr",
                table: "Speakers",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "DescriptionFr",
                table: "Speakers",
                newName: "Description");
        }
    }
}
