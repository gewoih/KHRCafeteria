using iText.Pdfa;
using KHRCafeteria.Commands;
using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories;
using KHRCafeteria.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace KHRCafeteria.ViewModels
{
	public class ReportsViewModel : BaseViewModel
	{
		#region Constructor
		public ReportsViewModel()
		{
			this.Companies = new ObservableCollection<Company>(new CompaniesRepository(new BaseDataContext()).GetAll());
			this.Employees = new ObservableCollection<Employee>(new EmployeesRepository(new BaseDataContext()).GetAll());

			this.SelectionCommand = new RelayCommand(OnSelectionCommandExecuted, CanSelectionCommandExecute);
			this.CreateReportCommand = new RelayCommand(OnCreateReportCommandExecuted, CanCreateReportCommandExecute);
		}
		#endregion

		#region Properties
		private DateTime _StartDate;
		public DateTime StartDate
		{
			get => _StartDate;
			set => Set(ref _StartDate, value);
		}

		private DateTime _EndDate;
		public DateTime EndDate
		{
			get => _EndDate;
			set => Set(ref _EndDate, value);
		}

		private ObservableCollection<Company> _Companies;
		public ObservableCollection<Company> Companies
		{
			get => _Companies;
			set => Set(ref _Companies, value);
		}

		private Company _SelectedCompany;
		public Company SelectedCompany
		{
			get => _SelectedCompany;
			set => Set(ref _SelectedCompany, value);
		}

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
		#endregion

		#region Commands
		public ICommand SelectionCommand { get; }
		private bool CanSelectionCommandExecute(object p) => true;
		private void OnSelectionCommandExecuted(object p)
		{
			this.StartDate = ((SelectedDatesCollection)p).First();
			this.EndDate = ((SelectedDatesCollection)p).Last();
		}

		public ICommand CreateReportCommand { get; }
		private bool CanCreateReportCommandExecute(object p) => this.StartDate != DateTime.MinValue && this.EndDate != DateTime.MinValue && (this.SelectedEmployee != null || this.SelectedCompany != null);
		private void OnCreateReportCommandExecuted(object p)
		{
			//
		}
		#endregion
	}
}
