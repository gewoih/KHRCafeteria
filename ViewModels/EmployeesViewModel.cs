using KHRCafeteria.Commands;
using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories;
using KHRCafeteria.ViewModels.Base;
using KHRCafeteria.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace KHRCafeteria.ViewModels
{
	public class EmployeesViewModel : BaseViewModel
	{
		#region Constructor
		public EmployeesViewModel()
		{
			this.ShowNewEmployeeWindowCommand = new RelayCommand(OnShowNewEmployeeWindowCommandExecuted, CanShowNewEmployeeWindowCommandExecute);
			this.AddEmployeeCommand = new RelayCommand(OnAddEmployeeCommandExecuted, CanAddEmployeeCommandExecute);
		}
		#endregion

		#region Properties
		private NewEmployeeView _NewEmployeeView;
		private List<Company> _Companies;

		private ObservableCollection<Employee> _Employees;
		public ObservableCollection<Employee> Employees
		{
			get => _Employees;
			set => Set(ref _Employees, value);
		}

		private Employee _SelectedEmployee;
		public Employee SelectedEmployee
		{
			get => _SelectedEmployee;
			set => Set(ref _SelectedEmployee, value);
		}

		private Employee _NewEmployee;
		public Employee NewEmployee
		{
			get => _NewEmployee;
			set => Set(ref _NewEmployee, value);
		}
		#endregion

		#region Commands
		public ICommand ShowNewEmployeeWindowCommand { get; }
		private bool CanShowNewEmployeeWindowCommandExecute(object p) => true;
		private void OnShowNewEmployeeWindowCommandExecuted(object p)
		{
			this.NewEmployee = new Employee();
			this._Companies = new CompaniesRepository(new BaseDataContext()).GetAll().ToList();

			this._NewEmployeeView = new NewEmployeeView(this);
			this._NewEmployeeView.ShowDialog();
		}

		public ICommand AddEmployeeCommand { get; }
		private bool CanAddEmployeeCommandExecute(object p) => true;
		private void OnAddEmployeeCommandExecuted(object p)
		{
			if (this.NewEmployee.Name == String.Empty)
				MessageBox.Show("Введите имя сотрудника!");
			else if (this.NewEmployee.DateOfBirth == DateTime.MinValue)
				MessageBox.Show("Введите дату рождения сотрудника!");
			else if (this.NewEmployee.Card.UID == String.Empty)
				MessageBox.Show("Отсканируйте карту для привязки!");
			else if (this.NewEmployee.Company == null)
				MessageBox.Show("Выберите компанию для привязки!");
			else
			{
				MessageBox.Show("Сотрудник успешно добавлен.");
			}
		}
		#endregion
	}
}
