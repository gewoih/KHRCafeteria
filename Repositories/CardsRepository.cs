using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories.Base;

namespace KHRCafeteria.Repositories
{
	public class CardsRepository : BaseRepository<Card>
	{
		public CardsRepository(BaseDataContext dbContext) : base(dbContext) { }
	}
}
