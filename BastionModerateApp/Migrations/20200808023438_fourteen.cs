using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BastionModerateApp.Migrations
{
    public partial class fourteen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_party_invite_entries",
                table: "party_invite_entries");

            migrationBuilder.DropIndex(
                name: "ix_party_invite_entries_party_invite_id",
                table: "party_invite_entries");

            migrationBuilder.DropColumn(
                name: "discord_user_id",
                table: "users");

            migrationBuilder.AddColumn<decimal>(
                name: "discord_id",
                table: "users",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "url",
                table: "users",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "party_invite_entry_id",
                table: "party_invite_entries",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_party_invite_entries",
                table: "party_invite_entries",
                columns: new[] { "party_invite_id", "party_invite_entry_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_party_invite_entries",
                table: "party_invite_entries");

            migrationBuilder.DropColumn(
                name: "discord_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "url",
                table: "users");

            migrationBuilder.AddColumn<decimal>(
                name: "discord_user_id",
                table: "users",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "party_invite_entry_id",
                table: "party_invite_entries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_party_invite_entries",
                table: "party_invite_entries",
                column: "party_invite_entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_party_invite_entries_party_invite_id",
                table: "party_invite_entries",
                column: "party_invite_id");
        }
    }
}
