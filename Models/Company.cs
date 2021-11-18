using KHRCafeteria.Models.Base;
using System.Collections.ObjectModel;

namespace KHRCafeteria.Models
{
	public class Company : Entity
	{
		private string _Name;
		public string Name
		{
			get => _Name;
			set => Set(ref _Name, value);
		}

		private ObservableCollection<Employee> _Employees;
		public ObservableCollection<Employee> Employees
		{
			get => _Employees;
			set => Set(ref _Employees, value);
		}
	}
}
