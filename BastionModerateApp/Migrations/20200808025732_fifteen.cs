using Microsoft.EntityFrameworkCore.Migrations;

namespace BastionModerateApp.Migrations
{
    public partial class fifteen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "url",
                table: "users");

            migrationBuilder.AddColumn<int>(
                name: "player_id",
                table: "users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "player_id",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "url",
                table: "users",
                type: "text",
                nullable: true);
        }
    }
}
