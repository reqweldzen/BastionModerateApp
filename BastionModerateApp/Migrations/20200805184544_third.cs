using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BastionModerateApp.Migrations
{
    public partial class third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "party_invites",
                columns: table => new
                {
                    party_invite_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_party_invites", x => x.party_invite_id);
                });

            migrationBuilder.CreateTable(
                name: "party_invite_entries",
                columns: table => new
                {
                    party_invite_entry_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    party_invite_id = table.Column<int>(nullable: false),
                    user_id = table.Column<decimal>(nullable: false),
                    reaction_name = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_party_invite_entries", x => x.party_invite_entry_id);
                    table.ForeignKey(
                        name: "fk_party_invite_entries_party_invites_party_invite_id",
                        column: x => x.party_invite_id,
                        principalTable: "party_invites",
                        principalColumn: "party_invite_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_party_invite_entries_party_invite_id",
                table: "party_invite_entries",
                column: "party_invite_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "party_invite_entries");

            migrationBuilder.DropTable(
                name: "party_invites");
        }
    }
}
