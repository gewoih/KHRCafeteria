using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KHRCafeteria.Repositories
{
	public class CompaniesRepository : BaseRepository<Company>
	{
		public CompaniesRepository(BaseDataContext dbContext) : base(dbContext) { }

		public override IQueryable<Company> GetAll()
		{
			return base.GetAll().Include(c => c.Employees);
		}
	}
}
