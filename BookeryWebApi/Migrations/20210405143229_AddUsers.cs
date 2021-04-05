using Microsoft.EntityFrameworkCore.Migrations;

namespace BookeryWebApi.Migrations
{
    public partial class AddUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerLogin",
                table: "Containers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Login = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Login);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Containers_OwnerLogin",
                table: "Containers",
                column: "OwnerLogin");

            migrationBuilder.AddForeignKey(
                name: "FK_Containers_Users_OwnerLogin",
                table: "Containers",
                column: "OwnerLogin",
                principalTable: "Users",
                principalColumn: "Login",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Containers_Users_OwnerLogin",
                table: "Containers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Containers_OwnerLogin",
                table: "Containers");

            migrationBuilder.DropColumn(
                name: "OwnerLogin",
                table: "Containers");
        }
    }
}
