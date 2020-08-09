using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BastionModerateApp.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "master");

            migrationBuilder.EnsureSchema(
                name: "transaction");

            migrationBuilder.CreateTable(
                name: "content_types",
                schema: "master",
                columns: table => new
                {
                    content_type_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type_name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_types", x => x.content_type_id);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                schema: "master",
                columns: table => new
                {
                    job_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    job_name = table.Column<string>(nullable: false),
                    shortcut_name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jobs", x => x.job_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "transaction",
                columns: table => new
                {
                    user_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    discord_id = table.Column<decimal>(nullable: false),
                    player_id = table.Column<long>(nullable: false),
                    player_name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "content_templates",
                schema: "master",
                columns: table => new
                {
                    content_template_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content_type_id = table.Column<int>(nullable: false),
                    content_name = table.Column<string>(nullable: false),
                    quest_url = table.Column<string>(nullable: true),
                    shortcut_name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_templates", x => x.content_template_id);
                    table.ForeignKey(
                        name: "fk_content_templates_content_types_content_type_id",
                        column: x => x.content_type_id,
                        principalSchema: "master",
                        principalTable: "content_types",
                        principalColumn: "content_type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "party_invites",
                schema: "transaction",
                columns: table => new
                {
                    party_invite_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(nullable: false),
                    message_id = table.Column<decimal>(nullable: false),
                    content_template_id = table.Column<int>(nullable: false),
                    purpose = table.Column<int>(nullable: false),
                    start_date = table.Column<DateTime>(nullable: false),
                    end_date = table.Column<DateTime>(nullable: true),
                    is_finished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_party_invites", x => x.party_invite_id);
                    table.ForeignKey(
                        name: "fk_party_invites_content_templates_content_template_id",
                        column: x => x.content_template_id,
                        principalSchema: "master",
                        principalTable: "content_templates",
                        principalColumn: "content_template_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_party_invites_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "transaction",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "party_invite_entries",
                schema: "transaction",
                columns: table => new
                {
                    party_invite_entry_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    party_invite_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false),
                    job_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_party_invite_entries", x => x.party_invite_entry_id);
                    table.ForeignKey(
                        name: "fk_party_invite_entries_jobs_job_id",
                        column: x => x.job_id,
                        principalSchema: "master",
                        principalTable: "jobs",
                        principalColumn: "job_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_party_invite_entries_party_invites_party_invite_id",
                        column: x => x.party_invite_id,
                        principalSchema: "transaction",
                        principalTable: "party_invites",
                        principalColumn: "party_invite_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_party_invite_entries_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "transaction",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_content_templates_content_type_id",
                schema: "master",
                table: "content_templates",
                column: "content_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_party_invite_entries_job_id",
                schema: "transaction",
                table: "party_invite_entries",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "ix_party_invite_entries_party_invite_id",
                schema: "transaction",
                table: "party_invite_entries",
                column: "party_invite_id");

            migrationBuilder.CreateIndex(
                name: "ix_party_invite_entries_user_id",
                schema: "transaction",
                table: "party_invite_entries",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_party_invites_content_template_id",
                schema: "transaction",
                table: "party_invites",
                column: "content_template_id");

            migrationBuilder.CreateIndex(
                name: "ix_party_invites_user_id",
                schema: "transaction",
                table: "party_invites",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "party_invite_entries",
                schema: "transaction");

            migrationBuilder.DropTable(
                name: "jobs",
                schema: "master");

            migrationBuilder.DropTable(
                name: "party_invites",
                schema: "transaction");

            migrationBuilder.DropTable(
                name: "content_templates",
                schema: "master");

            migrationBuilder.DropTable(
                name: "users",
                schema: "transaction");

            migrationBuilder.DropTable(
                name: "content_types",
                schema: "master");
        }
    }
}
