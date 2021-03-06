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

		public BaseDataContext()
		{
			//Database.EnsureDeleted();
			//Database.EnsureCreated();
		}

		public BaseDataContext(DbContextOptions<BaseDataContext> contextOptions) : base(contextOptions) { }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseMySql(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString, new MySqlServerVersion(new Version(5, 7, 27)));
				//optionsBuilder.EnableSensitiveDataLogging(true);
				//optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Employee>()
				.HasOne(e => e.Card)
				.WithOne(c => c.Employee)
				.HasForeignKey<Card>(c => c.EmployeeId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
