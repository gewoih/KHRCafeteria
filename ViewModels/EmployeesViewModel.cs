using KHRCafeteria.Commands;
using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories;
using KHRCafeteria.ViewModels.Base;
using KHRCafeteria.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace KHRCafeteria.ViewModels
{
	public class EmployeesViewModel : BaseViewModel
	{
		#region Constructor
		public EmployeesViewModel()
		{
			this.Employees = new ObservableCollection<Employee>(new EmployeesRepository(new BaseDataContext()).GetAll());

			this.ShowNewEmployeeWindowCommand = new RelayCommand(OnShowNewEmployeeWindowCommandExecuted, CanShowNewEmployeeWindowCommandExecute);
			this.AddEmployeeCommand = new RelayCommand(OnAddEmployeeCommandExecuted, CanAddEmployeeCommandExecute);
			this.RemoveEmployeeCommand = new RelayCommand(OnRemoveEmployeeCommandExecuted, CanRemoveEmployeeCommandExecute);
		}
		#endregion

		#region Properties
		//Форма создания нового сотрудника
		private NewEmployeeView _NewEmployeeView;

		//Список всех сотрудников
		private ObservableCollection<Employee> _Employees;
		public ObservableCollection<Employee> Employees
		{
			get => _Employees;
			set => Set(ref _Employees, value);
		}

		//Список всех компаний для отображения в ComboBox
		private ObservableCollection<Company> _Companies;
		public ObservableCollection<Company> Companies
		{
			get => _Companies;
			set => Set(ref _Companies, value);
		}

		//Выбранный сотрудник
		private Employee _SelectedEmployee;
		public Employee SelectedEmployee
		{
			get => _SelectedEmployee;
			set => Set(ref _SelectedEmployee, value);
		}

		//Новый сотрудник (при создании через форму)
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
			//Инициализируем нового сотрудника и его поля
			this.NewEmployee = new Employee { DateOfBirth = DateTime.Now, Company = new Company(), Card = new Card() };
			//Список компаний для их отображения в ComboBox
			this._Companies = new ObservableCollection<Company>(new CompaniesRepository(new BaseDataContext()).GetAll());

			//Создание и вызов окна для создания сотрудника
			this._NewEmployeeView = new NewEmployeeView(this);
			this._NewEmployeeView.ShowDialog();
		}

		public ICommand AddEmployeeCommand { get; }
		private bool CanAddEmployeeCommandExecute(object p) => true;
		private void OnAddEmployeeCommandExecuted(object p)
		{
			//Проверки на валидность
			if (this.NewEmployee.Name == String.Empty)
				MessageBox.Show("Введите имя сотрудника!");
			else if (this.NewEmployee.DateOfBirth == DateTime.MinValue)
				MessageBox.Show("Введите дату рождения сотрудника!");
			else if (this.NewEmployee.Company.Id == 0)
				MessageBox.Show("Выберите компанию для привязки!");
			else if (this.NewEmployee.Card.UID == String.Empty)
				MessageBox.Show("Отсканируйте карту для привязки!");
			else
			{
				//Локальный репозиторий для работы с картами
				CardsRepository CardsRepository = new CardsRepository(new BaseDataContext());

				//Проверка зарегистрирована ли такая карта
				if (CardsRepository.GetAll().FirstOrDefault(c => c.UID == this.NewEmployee.Card.UID) != null)
					MessageBox.Show("Данная карта уже привязана к сотруднику!");
				else
				{
					//Создаем сотрудника
					this.NewEmployee = new EmployeesRepository(new BaseDataContext()).Create(this.NewEmployee);
					//Создаем карту
					NewEmployee.Card = CardsRepository.Create(new Card { UID = this.NewEmployee.Card.UID, Employee = NewEmployee });
					//Добавляем созданного сотрудника в список
					this.Employees.Add(NewEmployee);

					//Закрываем окно создания сотрудника
					this._NewEmployeeView.Close();
					MessageBox.Show("Сотрудник успешно добавлен.");
				}
			}
		}

		public ICommand RemoveEmployeeCommand { get; }
		private bool CanRemoveEmployeeCommandExecute(object p) => this.SelectedEmployee != null;
		private void OnRemoveEmployeeCommandExecuted(object p)
		{
			//Спрашиваем пользователя
			DialogResult dialogResult = MessageBox.Show($"Вы действительно хотите удалить сотрудника {this.SelectedEmployee.Name} [{this.SelectedEmployee.Id}]?",
														"Удаление сотрудника",
														MessageBoxButtons.YesNo);

			if (dialogResult == DialogResult.Yes)
			{
				//Сохраняем имя удаляемого сотрудника
				string deletedEmployeeName = this.SelectedEmployee.Name;
				//Удаляем сотрудника с таким Id из БД
				new EmployeesRepository(new BaseDataContext()).Delete(this.SelectedEmployee.Id);
				//Удаляем сотрудника из списка сотрудников
				this.Employees.Remove(this.SelectedEmployee);

				MessageBox.Show($"Сотрудник '{deletedEmployeeName}' успешно удален.");
			}
		}
		#endregion
	}
}
