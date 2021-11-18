using KHRCafeteria.Models.Base;
using System;

namespace KHRCafeteria.Models
{
	public class Lunch : Entity
	{
		private Employee _Employee;
		public Employee Employee
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

		private bool _IsCompleted;
		public bool IsCompleted
		{
			get => _IsCompleted;
			set => Set(ref _IsCompleted, value);
		}
	}
}
