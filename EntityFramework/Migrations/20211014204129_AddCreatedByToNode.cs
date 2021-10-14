using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFramework.Migrations
{
    public partial class AddCreatedByToNode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Nodes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_CreatedById",
                table: "Nodes",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Users_CreatedById",
                table: "Nodes",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Users_CreatedById",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_CreatedById",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Nodes");
        }
    }
}
