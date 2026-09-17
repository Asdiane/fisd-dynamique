using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fisd.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBilingualParticipantFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Participants",
                newName: "NameFr");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Participants",
                newName: "DescriptionFr");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Participants",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Participants",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE [Participants] SET [NameEn] = [NameFr] WHERE [NameEn] = '';");
            migrationBuilder.Sql("UPDATE [Participants] SET [DescriptionEn] = [DescriptionFr] WHERE [DescriptionEn] = '';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Participants");

            migrationBuilder.RenameColumn(
                name: "NameFr",
                table: "Participants",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DescriptionFr",
                table: "Participants",
                newName: "Description");
        }
    }
}
