﻿using System.IO;
using BastionModerateApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable 8618

namespace BastionModerateApp
{
	public class BastionContext : DbContext
	{
		public static bool IsMigration { get; set; } = true;

		public DbSet<User> Users { get; set; }
		
		public DbSet<Job> Jobs { get; set; }
		
		public DbSet<ContentTemplate> ContentTemplates { get; set; }
		
		public DbSet<ContentType> ContentTypes { get; set; }

		public DbSet<PartyInvite> PartyInvites { get; set; }
		public DbSet<PartyInviteEntry> PartyInviteEntries { get; set; }

		public BastionContext()
		{
		}
		
		public BastionContext(DbContextOptions<BastionContext> options) : base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (IsMigration)
			{
				var configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile($"appsettings.json", optional: false)
					.Build();
				
				optionsBuilder.UseNpgsql(configuration.GetConnectionString("Default"));
				optionsBuilder.UseLazyLoadingProxies();
				optionsBuilder.UseSnakeCaseNamingConvention();
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PartyInviteEntry>().HasKey(x => new {x.PartyInviteEntryId});

			modelBuilder.Entity<PartyInviteEntry>()
				.HasOne(x => x.PartyInvite)
				.WithMany(x => x.PartyInviteEntries)
				.HasForeignKey(x => x.PartyInviteId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}