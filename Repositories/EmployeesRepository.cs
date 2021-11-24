using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KHRCafeteria.Repositories
{
	public class EmployeesRepository : BaseRepository<Employee>
	{
		public EmployeesRepository(BaseDataContext dbContext) : base(dbContext) { }

		public override IQueryable<Employee> GetAll()
		{
			return base.GetAll().Include(e => e.Company).Include(e => e.Card).Include(e => e.Lunches);
		}

		public override void Delete(int id)
		{
			Employee EmployeeToDelete = this.GetAll().FirstOrDefault(e => e.Id == id);

			base._dbContext.Cards.Remove(EmployeeToDelete.Card);
			base._dbContext.Lunches.RemoveRange(EmployeeToDelete.Lunches);
			base.Delete(id);
		}
	}
}
