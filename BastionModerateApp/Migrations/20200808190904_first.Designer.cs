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
    [Migration("20200808190904_first")]
    partial class first
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("BastionModerateApp.Entities.ContentTemplate", b =>
                {
                    b.Property<int>("ContentTemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("content_template_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ContentName")
                        .IsRequired()
                        .HasColumnName("content_name")
                        .HasColumnType("text");

                    b.Property<int>("ContentTypeId")
                        .HasColumnName("content_type_id")
                        .HasColumnType("integer");

                    b.Property<string>("QuestUrl")
                        .HasColumnName("quest_url")
                        .HasColumnType("text");

                    b.Property<string>("ShortcutName")
                        .IsRequired()
                        .HasColumnName("shortcut_name")
                        .HasColumnType("text");

                    b.HasKey("ContentTemplateId")
                        .HasName("pk_content_templates");

                    b.HasIndex("ContentTypeId")
                        .HasName("ix_content_templates_content_type_id");

                    b.ToTable("content_templates","master");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.ContentType", b =>
                {
                    b.Property<int>("ContentTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("content_type_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnName("type_name")
                        .HasColumnType("text");

                    b.HasKey("ContentTypeId")
                        .HasName("pk_content_types");

                    b.ToTable("content_types","master");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("job_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasColumnName("job_name")
                        .HasColumnType("text");

                    b.Property<string>("ShortcutName")
                        .IsRequired()
                        .HasColumnName("shortcut_name")
                        .HasColumnType("text");

                    b.HasKey("JobId")
                        .HasName("pk_jobs");

                    b.ToTable("jobs","master");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.PartyInvite", b =>
                {
                    b.Property<int>("PartyInviteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("party_invite_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ContentTemplateId")
                        .HasColumnName("content_template_id")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnName("end_date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsFinished")
                        .HasColumnName("is_finished")
                        .HasColumnType("boolean");

                    b.Property<decimal>("MessageId")
                        .HasColumnName("message_id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<int>("Purpose")
                        .HasColumnName("purpose")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnName("start_date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("integer");

                    b.HasKey("PartyInviteId")
                        .HasName("pk_party_invites");

                    b.HasIndex("ContentTemplateId")
                        .HasName("ix_party_invites_content_template_id");

                    b.HasIndex("UserId")
                        .HasName("ix_party_invites_user_id");

                    b.ToTable("party_invites","transaction");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.PartyInviteEntry", b =>
                {
                    b.Property<int>("PartyInviteEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("party_invite_entry_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("JobId")
                        .HasColumnName("job_id")
                        .HasColumnType("integer");

                    b.Property<int>("PartyInviteId")
                        .HasColumnName("party_invite_id")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("integer");

                    b.HasKey("PartyInviteEntryId")
                        .HasName("pk_party_invite_entries");

                    b.HasIndex("JobId")
                        .HasName("ix_party_invite_entries_job_id");

                    b.HasIndex("PartyInviteId")
                        .HasName("ix_party_invite_entries_party_invite_id");

                    b.HasIndex("UserId")
                        .HasName("ix_party_invite_entries_user_id");

                    b.ToTable("party_invite_entries","transaction");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("user_id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("DiscordId")
                        .HasColumnName("discord_id")
                        .HasColumnType("numeric(20,0)");

                    b.Property<long>("PlayerId")
                        .HasColumnName("player_id")
                        .HasColumnType("bigint");

                    b.Property<string>("PlayerName")
                        .IsRequired()
                        .HasColumnName("player_name")
                        .HasColumnType("text");

                    b.HasKey("UserId")
                        .HasName("pk_users");

                    b.ToTable("users","transaction");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.ContentTemplate", b =>
                {
                    b.HasOne("BastionModerateApp.Entities.ContentType", "ContentType")
                        .WithMany()
                        .HasForeignKey("ContentTypeId")
                        .HasConstraintName("fk_content_templates_content_types_content_type_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BastionModerateApp.Entities.PartyInvite", b =>
                {
                    b.HasOne("BastionModerateApp.Entities.ContentTemplate", "ContentTemplate")
                        .WithMany()
                        .HasForeignKey("ContentTemplateId")
                        .HasConstraintName("fk_party_invites_content_templates_content_template_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BastionModerateApp.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_party_invites_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BastionModerateApp.Entities.PartyInviteEntry", b =>
                {
                    b.HasOne("BastionModerateApp.Entities.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .HasConstraintName("fk_party_invite_entries_jobs_job_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BastionModerateApp.Entities.PartyInvite", "PartyInvite")
                        .WithMany("PartyInviteEntries")
                        .HasForeignKey("PartyInviteId")
                        .HasConstraintName("fk_party_invite_entries_party_invites_party_invite_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BastionModerateApp.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_party_invite_entries_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
