using KHRCafeteria.Models.Base;
using System;

namespace KHRCafeteria.Models
{
	public class Lunch : Entity
	{
		private string _EmployeeName;
		public string EmployeeName
		{
			get => _EmployeeName;
			set => Set(ref _EmployeeName, value);
		}

		private string _CompanyName;
		public string CompanyName
		{
			get => _CompanyName;
			set => Set(ref _CompanyName, value);
		}

		private string _CardUID;
		public string CardUID
		{
			get => _CardUID;
			set => Set(ref _CardUID, value);
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

		private bool _IsCompleted;
		public bool IsCompleted
		{
			get => _IsCompleted;
			set => Set(ref _IsCompleted, value);
		}
	}
}
