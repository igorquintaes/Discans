using Microsoft.EntityFrameworkCore.Migrations;

namespace Discans.Shared.Migrations
{
    public partial class NewSites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MangaSite",
                table: "Mangas",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MangaSiteId",
                table: "Mangas",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MangaSite",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "MangaSiteId",
                table: "Mangas");
        }
    }
}
