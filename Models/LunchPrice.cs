using KHRCafeteria.Models.Base;
using System;

namespace KHRCafeteria.Models
{
	public class LunchPrice : Entity
	{
		private double _Price;
		public double Price
		{
			get => _Price;
			set => Set(ref _Price, value);
		}

		private DateTime _DateTime;
		public DateTime DateTime
		{
			get => _DateTime;
			set => Set(ref _DateTime, value);
		}
	}
}
