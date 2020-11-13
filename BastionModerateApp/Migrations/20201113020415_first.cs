using System;
using Microsoft.EntityFrameworkCore.Migrations;

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
                    ContentTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TypeName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_types", x => x.ContentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                schema: "master",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JobName = table.Column<string>(type: "TEXT", nullable: false),
                    ShortcutName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobs", x => x.JobId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "transaction",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "content_templates",
                schema: "master",
                columns: table => new
                {
                    ContentTemplateId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContentTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContentName = table.Column<string>(type: "TEXT", nullable: false),
                    QuestUrl = table.Column<string>(type: "TEXT", nullable: true),
                    ShortcutName = table.Column<string>(type: "TEXT", nullable: false),
                    MaxPlayer = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_templates", x => x.ContentTemplateId);
                    table.ForeignKey(
                        name: "FK_content_templates_content_types_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "master",
                        principalTable: "content_types",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "party_invites",
                schema: "transaction",
                columns: table => new
                {
                    PartyInviteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    MessageId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MemberListMessageId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ContentTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    Purpose = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsFinished = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_party_invites", x => x.PartyInviteId);
                    table.ForeignKey(
                        name: "FK_party_invites_content_templates_ContentTemplateId",
                        column: x => x.ContentTemplateId,
                        principalSchema: "master",
                        principalTable: "content_templates",
                        principalColumn: "ContentTemplateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_party_invites_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "transaction",
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "party_invite_entries",
                schema: "transaction",
                columns: table => new
                {
                    PartyInviteEntryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartyInviteId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    JobId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_party_invite_entries", x => x.PartyInviteEntryId);
                    table.ForeignKey(
                        name: "FK_party_invite_entries_jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "master",
                        principalTable: "jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_party_invite_entries_party_invites_PartyInviteId",
                        column: x => x.PartyInviteId,
                        principalSchema: "transaction",
                        principalTable: "party_invites",
                        principalColumn: "PartyInviteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_party_invite_entries_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "transaction",
                        principalTable: "users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_content_templates_ContentTypeId",
                schema: "master",
                table: "content_templates",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_party_invite_entries_JobId",
                schema: "transaction",
                table: "party_invite_entries",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_party_invite_entries_PartyInviteId",
                schema: "transaction",
                table: "party_invite_entries",
                column: "PartyInviteId");

            migrationBuilder.CreateIndex(
                name: "IX_party_invite_entries_UserId",
                schema: "transaction",
                table: "party_invite_entries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_party_invites_ContentTemplateId",
                schema: "transaction",
                table: "party_invites",
                column: "ContentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_party_invites_UserId",
                schema: "transaction",
                table: "party_invites",
                column: "UserId");
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
