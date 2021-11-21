using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KHRCafeteria.Repositories
{
	public class EmployeesRepository : BaseRepository<Employee>
	{
		public EmployeesRepository(BaseDataContext dbContext) : base(dbContext) { }

		public override IQueryable<Employee> GetAll()
		{
			return base.GetAll().Include(e => e.Company).Include(e => e.Card).Include(e => e.Lunches);
		}
	}
}
