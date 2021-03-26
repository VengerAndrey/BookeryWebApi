using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookeryWebApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Containers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Containers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdContainer = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blobs_Containers_IdContainer",
                        column: x => x.IdContainer,
                        principalTable: "Containers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blobs_IdContainer",
                table: "Blobs",
                column: "IdContainer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blobs");

            migrationBuilder.DropTable(
                name: "Containers");
        }
    }
}
