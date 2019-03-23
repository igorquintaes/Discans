using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Discans.Shared.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mangas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    LastRelease = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mangas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerChannels",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChannelId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrivateTrackings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<ulong>(nullable: false),
                    MangaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateTrackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateTrackings_Mangas_MangaId",
                        column: x => x.MangaId,
                        principalTable: "Mangas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServerAlert",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ServerId = table.Column<ulong>(nullable: false),
                    MangaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerAlert", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerAlert_Mangas_MangaId",
                        column: x => x.MangaId,
                        principalTable: "Mangas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAlert",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ServerId = table.Column<ulong>(nullable: false),
                    MangaId = table.Column<int>(nullable: true),
                    UserId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAlert", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAlert_Mangas_MangaId",
                        column: x => x.MangaId,
                        principalTable: "Mangas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivateTrackings_MangaId",
                table: "PrivateTrackings",
                column: "MangaId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerAlert_MangaId",
                table: "ServerAlert",
                column: "MangaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAlert_MangaId",
                table: "UserAlert",
                column: "MangaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivateTrackings");

            migrationBuilder.DropTable(
                name: "ServerAlert");

            migrationBuilder.DropTable(
                name: "ServerChannels");

            migrationBuilder.DropTable(
                name: "UserAlert");

            migrationBuilder.DropTable(
                name: "Mangas");
        }
    }
}
