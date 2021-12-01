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

		public override void Delete(int id)
		{
			Company CompanyToDelete = this.GetAll().FirstOrDefault(c => c.Id == id);

			base._dbContext.Cards.RemoveRange(base._dbContext.Cards.Where(l => CompanyToDelete.Employees.Contains(l.Employee)));
			base._dbContext.Lunches.RemoveRange(base._dbContext.Lunches.Where(l => CompanyToDelete.Employees.Contains(l.Employee)));
			base._dbContext.Employees.RemoveRange(CompanyToDelete.Employees);

			base.Delete(id);
		}
	}
}
