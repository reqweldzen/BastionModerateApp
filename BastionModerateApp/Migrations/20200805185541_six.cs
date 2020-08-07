using Microsoft.EntityFrameworkCore.Migrations;

namespace BastionModerateApp.Migrations
{
    public partial class six : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "purpose",
                table: "party_invites",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "purpose",
                table: "party_invites");
        }
    }
}
