using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BastionModerateApp.Migrations
{
    public partial class ten : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "character_job_id",
                table: "party_invite_entries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "character_jobs",
                columns: table => new
                {
                    character_job_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    job_name = table.Column<string>(nullable: true),
                    reaction_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_jobs", x => x.character_job_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_jobs");

            migrationBuilder.DropColumn(
                name: "character_job_id",
                table: "party_invite_entries");
        }
    }
}
