﻿// <auto-generated />
using System;
using BastionModerateApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BastionModerateApp.Migrations
{
    [DbContext(typeof(BastionContext))]
    partial class BastionContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("BastionModerateApp.Entities.ContentTemplate", b =>
                {
                    b.Property<int>("ContentTemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ContentName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ContentTypeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxPlayer")
                        .HasColumnType("INTEGER");

                    b.Property<string>("QuestUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("ShortcutName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ContentTemplateId");

                    b.HasIndex("ContentTypeId");

                    b.ToTable("content_templates", "master");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.ContentType", b =>
                {
                    b.Property<int>("ContentTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ContentTypeId");

                    b.ToTable("content_types", "master");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.Job", b =>
                {
                    b.Property<int>("JobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("JobName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ShortcutName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("JobId");

                    b.ToTable("jobs", "master");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.PartyInvite", b =>
                {
                    b.Property<int>("PartyInviteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ContentTemplateId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsFinished")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("MemberListMessageId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("MessageId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Purpose")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PartyInviteId");

                    b.HasIndex("ContentTemplateId");

                    b.HasIndex("UserId");

                    b.ToTable("party_invites", "transaction");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.PartyInviteEntry", b =>
                {
                    b.Property<int>("PartyInviteEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("JobId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PartyInviteId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PartyInviteEntryId");

                    b.HasIndex("JobId");

                    b.HasIndex("PartyInviteId");

                    b.HasIndex("UserId");

                    b.ToTable("party_invite_entries", "transaction");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("DiscordId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId");

                    b.ToTable("users", "transaction");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.ContentTemplate", b =>
                {
                    b.HasOne("BastionModerateApp.Entities.ContentType", "ContentType")
                        .WithMany()
                        .HasForeignKey("ContentTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentType");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.PartyInvite", b =>
                {
                    b.HasOne("BastionModerateApp.Entities.ContentTemplate", "ContentTemplate")
                        .WithMany()
                        .HasForeignKey("ContentTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BastionModerateApp.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentTemplate");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.PartyInviteEntry", b =>
                {
                    b.HasOne("BastionModerateApp.Entities.Job", "Job")
                        .WithMany()
                        .HasForeignKey("JobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BastionModerateApp.Entities.PartyInvite", "PartyInvite")
                        .WithMany("PartyInviteEntries")
                        .HasForeignKey("PartyInviteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BastionModerateApp.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Job");

                    b.Navigation("PartyInvite");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BastionModerateApp.Entities.PartyInvite", b =>
                {
                    b.Navigation("PartyInviteEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
