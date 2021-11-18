using KHRCafeteria.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;

namespace KHRCafeteria.DataContext
{
	public class BaseDataContext : DbContext
	{
		public DbSet<Employee> Employees { get; set; }
		public DbSet<Company> Companies { get; set; }
		public DbSet<Card> Cards { get; set; }
		public DbSet<Lunch> Lunches { get; set; }

		public BaseDataContext(DbContextOptions<BaseDataContext> contextOptions) : base(contextOptions) { }

		public BaseDataContext()
		{
			Database.EnsureDeleted();
			Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseMySql(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString, new MySqlServerVersion(new Version(5, 7, 27)));
				optionsBuilder.EnableSensitiveDataLogging(true);
				optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
			}
		}
	}
}
