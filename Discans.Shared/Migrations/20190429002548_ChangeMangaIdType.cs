using Microsoft.EntityFrameworkCore.Migrations;

namespace Discans.Shared.Migrations
{
    public partial class ChangeMangaIdType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MangaSiteId",
                table: "Mangas",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MangaSiteId",
                table: "Mangas",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
