using KHRCafeteria.Models.Base;
using System;
using System.Collections.ObjectModel;

namespace KHRCafeteria.Models
{
	public class Employee : Entity
	{
		private string _Name;
		public string Name
		{
			get => _Name;
			set => Set(ref _Name, value);
		}

		private DateTime _DateOfBirth;
		public DateTime DateOfBirth
		{
			get => _DateOfBirth;
			set => Set(ref _DateOfBirth, value);
		}

		private int _CardId;
		public int CardId
		{
			get => _CardId;
			set => Set(ref _CardId, value);
		}

		private Card _Card;
		public Card Card
		{
			get => _Card;
			set => Set(ref _Card, value);
		}

		private Company _Company;
		public Company Company
		{
			get => _Company;
			set => Set(ref _Company, value);
		}

		private ObservableCollection<Lunch> _Lunches;
		public ObservableCollection<Lunch> Lunches
		{
			get => _Lunches;
			set => Set(ref _Lunches, value);
		}
	}
}
