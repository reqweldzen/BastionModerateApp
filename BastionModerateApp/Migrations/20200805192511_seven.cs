using Microsoft.EntityFrameworkCore.Migrations;

namespace BastionModerateApp.Migrations
{
    public partial class seven : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "reaction_name",
                table: "party_invite_entries",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(20,0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "reaction_name",
                table: "party_invite_entries",
                type: "numeric(20,0)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
