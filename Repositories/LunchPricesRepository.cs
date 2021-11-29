using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories.Base;

namespace KHRCafeteria.Repositories
{
	public class LunchPricesRepository : BaseRepository<LunchPrice>
	{
		public LunchPricesRepository(BaseDataContext dbContext) : base(dbContext) { }
	}
}
