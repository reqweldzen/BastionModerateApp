using Microsoft.EntityFrameworkCore.Migrations;

namespace BastionModerateApp.Migrations
{
    public partial class sec : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "shortcut_name",
                table: "raid_templates",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shortcut_name",
                table: "raid_templates");
        }
    }
}
