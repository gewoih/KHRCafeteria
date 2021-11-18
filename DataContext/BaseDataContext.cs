using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;

namespace KHRCafeteria.DataContext
{
	public class BaseDataContext : DbContext
	{
		/*public DbSet<User> Users { get; set; }
		public DbSet<Stock> Stocks { get; set; }
		public DbSet<StockQuote> StocksQuotes { get; set; }
		public DbSet<LogRecord> LogRecords { get; set; }*/

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
