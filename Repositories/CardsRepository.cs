using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KHRCafeteria.Repositories
{
	public class CardsRepository : BaseRepository<Card>
	{
		public CardsRepository(BaseDataContext dbContext) : base(dbContext) { }

		public override IQueryable<Card> GetAll()
		{
			return base.GetAll().Include(c => c.Employee.Company);
		}
	}
}
