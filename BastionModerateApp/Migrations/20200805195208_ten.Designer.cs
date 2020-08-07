﻿// <auto-generated />
using System;
using BastionModerateApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BastionModerateApp.Migrations
{
    [DbContext(typeof(BastionContext))]
    [Migration("20200805195208_ten")]
    partial class ten
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("BastionModerateApp.Models.CharacterJob", b =>
                {
                    b.Property<int>("CharacterJobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("character_job_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("JobName")
                        .HasColumnName("job_name")
                        .HasColumnType("text");

                    b.Property<string>("ReactionName")
                        .HasColumnName("reaction_name")
                        .HasColumnType("text");

                    b.HasKey("CharacterJobId")
                        .HasName("pk_character_jobs");

                    b.ToTable("character_jobs");
                });

            modelBuilder.Entity("BastionModerateApp.Models.PartyInvite", b =>
                {
                    b.Property<int>("PartyInviteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("party_invite_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Purpose")
                        .HasColumnName("purpose")
                        .HasColumnType("integer");

                    b.Property<int>("RaidTemplateId")
                        .HasColumnName("raid_template_id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnName("start_date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("PartyInviteId")
                        .HasName("pk_party_invites");

                    b.HasIndex("RaidTemplateId")
                        .HasName("ix_party_invites_raid_template_id");

                    b.ToTable("party_invites");
                });

            modelBuilder.Entity("BastionModerateApp.Models.PartyInviteEntry", b =>
                {
                    b.Property<int>("PartyInviteEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("party_invite_entry_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CharacterJobId")
                        .HasColumnName("character_job_id")
                        .HasColumnType("integer");

                    b.Property<int>("PartyInviteId")
                        .HasColumnName("party_invite_id")
                        .HasColumnType("integer");

                    b.Property<string>("ReactionName")
                        .HasColumnName("reaction_name")
                        .HasColumnType("text");

                    b.Property<decimal>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("PartyInviteEntryId")
                        .HasName("pk_party_invite_entries");

                    b.HasIndex("PartyInviteId")
                        .HasName("ix_party_invite_entries_party_invite_id");

                    b.ToTable("party_invite_entries");
                });

            modelBuilder.Entity("BastionModerateApp.Models.RaidTemplate", b =>
                {
                    b.Property<int>("RaidTemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("raid_template_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("QuestUrl")
                        .HasColumnName("quest_url")
                        .HasColumnType("text");

                    b.Property<string>("RaidName")
                        .HasColumnName("raid_name")
                        .HasColumnType("text");

                    b.Property<string>("ShortcutName")
                        .HasColumnName("shortcut_name")
                        .HasColumnType("text");

                    b.HasKey("RaidTemplateId")
                        .HasName("pk_raid_templates");

                    b.ToTable("raid_templates");
                });

            modelBuilder.Entity("BastionModerateApp.Models.PartyInvite", b =>
                {
                    b.HasOne("BastionModerateApp.Models.RaidTemplate", "RaidTemplate")
                        .WithMany()
                        .HasForeignKey("RaidTemplateId")
                        .HasConstraintName("fk_party_invites_raid_templates_raid_template_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BastionModerateApp.Models.PartyInviteEntry", b =>
                {
                    b.HasOne("BastionModerateApp.Models.PartyInvite", "PartyInvite")
                        .WithMany("PartyInviteEntries")
                        .HasForeignKey("PartyInviteId")
                        .HasConstraintName("fk_party_invite_entries_party_invites_party_invite_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
