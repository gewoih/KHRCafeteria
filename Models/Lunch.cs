using KHRCafeteria.Models.Base;
using System;

namespace KHRCafeteria.Models
{
	public class Lunch : Entity
	{
		private Employee _Employee;
		public  Employee Employee
		{
			get => _Employee;
			set => Set(ref _Employee, value);
		}

		private DateTime _DateTime;
		public DateTime DateTime
		{
			get => _DateTime;
			set => Set(ref _DateTime, value);
		}

		private double _Price;
		public double Price
		{
			get => _Price;
			set => Set(ref _Price, value);
		}

		private bool _IsPaid;
		public bool IsPaid
		{
			get => _IsPaid;
			set => Set(ref _IsPaid, value);
		}
	}
}
