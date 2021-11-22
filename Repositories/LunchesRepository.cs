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
	public class LunchesRepository : BaseRepository<Lunch>
	{
		public LunchesRepository(BaseDataContext dbContext) : base(dbContext) { }

		public override IQueryable<Lunch> GetAll()
		{
			return base.GetAll().Include(l => l.Employee).Include(l => l.Card);
		}
	}
}
