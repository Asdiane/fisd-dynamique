using Microsoft.EntityFrameworkCore.Migrations;
namespace Fisd.Persistence.Migrations;
public partial class AddEditorialContentAndTestimonialImages : Migration
{
 protected override void Up(MigrationBuilder migrationBuilder) {
  migrationBuilder.AddColumn<string>(name: "EditorialJson", table: "SiteSettings", type: "nvarchar(max)", nullable: true);
  migrationBuilder.AddColumn<string>(name: "ImageUrl", table: "Testimonials", type: "nvarchar(max)", nullable: true);
 }
 protected override void Down(MigrationBuilder migrationBuilder) {
  migrationBuilder.DropColumn(name: "EditorialJson", table: "SiteSettings");
  migrationBuilder.DropColumn(name: "ImageUrl", table: "Testimonials");
 }
}
