using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EntityFramework.Migrations
{
    public partial class AddTimestampToNode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreationTimestamp",
                table: "Nodes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ModificationTimestamp",
                table: "Nodes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "Nodes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ModifiedById",
                table: "Nodes",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Users_ModifiedById",
                table: "Nodes",
                column: "ModifiedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Users_ModifiedById",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_ModifiedById",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "CreationTimestamp",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "ModificationTimestamp",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Nodes");
        }
    }
}
