using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFramework.Migrations
{
    public partial class AddSharesUsersTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "ShareUser",
                table => new
                {
                    SharesId = table.Column<Guid>("uniqueidentifier", nullable: false),
                    UsersId = table.Column<int>("int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShareUser", x => new {x.SharesId, x.UsersId});
                    table.ForeignKey(
                        "FK_ShareUser_Shares_SharesId",
                        x => x.SharesId,
                        "Shares",
                        "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        "FK_ShareUser_Users_UsersId",
                        x => x.UsersId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                "IX_ShareUser_UsersId",
                "ShareUser",
                "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "ShareUser");
        }
    }
}