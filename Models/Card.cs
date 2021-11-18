using KHRCafeteria.Models.Base;

namespace KHRCafeteria.Models
{
	public class Card : Entity
	{
		private string _UID;
		public string UID
		{
			get => _UID;
			set => Set(ref _UID, value);
		}

		private bool _IsActive;
		public bool IsActive
		{
			get => _IsActive;
			set => Set(ref _IsActive, value);
		}
	}
}
