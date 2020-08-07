using System.IO;
using BastionModerateApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BastionModerateApp
{
	public class BastionContext : DbContext
	{
		public static bool IsMigration { get; set; } = true;
		
		public DbSet<RaidTemplate> RaidTemplates { get; set; }
		public DbSet<PartyInvite> PartyInvites { get; set; }
		public DbSet<PartyInviteEntry> PartyInviteEntries { get; set; }
		
		public DbSet<CharacterJob> CharacterJobs { get; set; }

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
				
				optionsBuilder.UseNpgsql(configuration.GetConnectionString($"Default"));
				optionsBuilder.UseLazyLoadingProxies();
				optionsBuilder.UseSnakeCaseNamingConvention();
			}
		}
	}
}