using Microsoft.EntityFrameworkCore.Migrations;

namespace BastionModerateApp.Migrations
{
    public partial class four : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "raid_template_id",
                table: "party_invites",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_party_invites_raid_template_id",
                table: "party_invites",
                column: "raid_template_id");

            migrationBuilder.AddForeignKey(
                name: "fk_party_invites_raid_templates_raid_template_id",
                table: "party_invites",
                column: "raid_template_id",
                principalTable: "raid_templates",
                principalColumn: "raid_template_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_party_invites_raid_templates_raid_template_id",
                table: "party_invites");

            migrationBuilder.DropIndex(
                name: "ix_party_invites_raid_template_id",
                table: "party_invites");

            migrationBuilder.DropColumn(
                name: "raid_template_id",
                table: "party_invites");
        }
    }
}
