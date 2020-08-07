using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BastionModerateApp.Migrations
{
    public partial class thirteen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_character_jobs",
                table: "character_jobs");

            migrationBuilder.DropColumn(
                name: "character_job_id",
                table: "character_jobs");

            migrationBuilder.AddColumn<int>(
                name: "job_id",
                table: "character_jobs",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_character_jobs",
                table: "character_jobs",
                column: "job_id");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    discord_user_id = table.Column<decimal>(nullable: false),
                    player_name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_character_jobs",
                table: "character_jobs");

            migrationBuilder.DropColumn(
                name: "job_id",
                table: "character_jobs");

            migrationBuilder.AddColumn<int>(
                name: "character_job_id",
                table: "character_jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_character_jobs",
                table: "character_jobs",
                column: "character_job_id");
        }
    }
}
