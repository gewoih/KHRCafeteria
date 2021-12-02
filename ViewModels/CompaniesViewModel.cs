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
	public class CompaniesViewModel : BaseViewModel
	{
		#region Constructor
		public CompaniesViewModel()
		{
			this.Companies = new ObservableCollection<Company>(new CompaniesRepository(new BaseDataContext()).GetAll());

			this.ShowNewCompanyWindowCommand = new RelayCommand(OnShowNewCompanyWindowCommandExecuted, CanShowNewCompanyWindowCommandExecute);
			this.AddCompanyCommand = new RelayCommand(OnAddCompanyCommandExecuted, CanAddCompanyCommandExecute);
			this.RemoveCompanyCommand = new RelayCommand(OnRemoveCompanyCommandExecuted, CanRemoveCompanyCommandExecute);
			this.ShowEditCompanyWindowCommand = new RelayCommand(OnShowEditCompanyWindowCommandExecuted, CanShowEditCompanyWindowCommandExecute);
			this.EditCompanyCommand = new RelayCommand(OnEditCompanyCommandExecuted, CanEditCompanyCommandExecute);
		}
		#endregion

		#region Properties
		//Форма для создания новой компании
		private NewCompanyView NewCompanyView;
		//Форма для редактирования компании
		private EditCompanyView EditCompanyView;

		//Список компаний для вывода
		private ObservableCollection<Company> _Companies;
		public ObservableCollection<Company> Companies
		{
			get => _Companies;
			set => Set(ref _Companies, value);
		}

		//Новая компания
		private Company _NewCompany;
		public Company NewCompany
		{
			get => _NewCompany;
			set => Set(ref _NewCompany, value);
		}

		//Выбранная компания
		private Company _SelectedCompany;
		public Company SelectedCompany
		{
			get => _SelectedCompany;
			set => Set(ref _SelectedCompany, value);
		}
		#endregion

		#region Commands
		public ICommand ShowNewCompanyWindowCommand { get; }
		private bool CanShowNewCompanyWindowCommandExecute(object p) => true;
		private void OnShowNewCompanyWindowCommandExecuted(object p)
		{
			//Инициализируем новую компнанию
			this.NewCompany = new Company();

			//Инициализируем форму для создания компании и вызываем ее
			this.NewCompanyView = new NewCompanyView(this);
			this.NewCompanyView.ShowDialog();
		}

		public ICommand AddCompanyCommand { get; }
		private bool CanAddCompanyCommandExecute(object p) => true;
		private void OnAddCompanyCommandExecuted(object p)
		{
			//Пустое ли имя компании?
			if (this.NewCompany.Name != String.Empty && this.NewCompany.LunchPrice != 0)
			{
				CompaniesRepository CompaniesRepository = new CompaniesRepository(new BaseDataContext());
				//Проверяем есть ли в базе компания с таким именем (без учета регистра)
				if (CompaniesRepository.GetAll().FirstOrDefault(c => c.Name.ToLower() == this.NewCompany.Name.ToLower()) == null)
				{
					//Если такая компания не найдена - создаем
					this.Companies.Add(CompaniesRepository.Create(this.NewCompany));

					//Закрываем форму создания компании
					this.NewCompanyView.Close();
					MessageBox.Show($"Компания '{this.NewCompany.Name}' успешно добавлена.");
				}
				else
					MessageBox.Show("Компания с таким названием уже существует.");
			}
			else
				MessageBox.Show("Заполните все поля!");
		}

		public ICommand RemoveCompanyCommand { get; }
		private bool CanRemoveCompanyCommandExecute(object p) => this.SelectedCompany != null;
		private void OnRemoveCompanyCommandExecuted(object p)
		{
			//Спрашиваем пользователя
			DialogResult dialogResult = MessageBox.Show($"Вы действительно хотите удалить компанию {this.SelectedCompany.Name} [{this.SelectedCompany.Id}]?", 
														"Удаление компании", 
														MessageBoxButtons.YesNo);

			if (dialogResult == DialogResult.Yes)
			{
				//Сохраняем имя удаляемой компании
				string deletedCompanyName = this.SelectedCompany.Name;
				//Удаляем компанию с таким Id из БД
				new CompaniesRepository(new BaseDataContext()).Delete(this.SelectedCompany.Id);
				//Удаляем компанию из списка компаний
				this.Companies.Remove(this.SelectedCompany);

				MessageBox.Show($"Компания '{deletedCompanyName}' успешно удалена.");
			}
		}

		public ICommand ShowEditCompanyWindowCommand { get; }
		private bool CanShowEditCompanyWindowCommandExecute(object p) => this.SelectedCompany != null;
		private void OnShowEditCompanyWindowCommandExecuted(object p)
		{
			//Инициализируем форму для создания компании и вызываем ее
			this.EditCompanyView = new EditCompanyView(this);
			this.EditCompanyView.ShowDialog();
		}

		public ICommand EditCompanyCommand { get; }
		private bool CanEditCompanyCommandExecute(object p) => this.SelectedCompany != null;
		private void OnEditCompanyCommandExecuted(object p)
		{
			//Пустое ли имя компании?
			if (this.SelectedCompany.Name != String.Empty && this.SelectedCompany.LunchPrice != 0)
			{
				CompaniesRepository CompaniesRepository = new CompaniesRepository(new BaseDataContext());
				//Спрашиваем пользователя
				DialogResult dialogResult = MessageBox.Show($"Вы действительно хотите сохранить информацию о компанию '{this.SelectedCompany.Name}' [{this.SelectedCompany.Id}]?",
															"Сохранение информации о компании",
															MessageBoxButtons.YesNo);

				if (dialogResult == DialogResult.Yes)
				{
					new CompaniesRepository(new BaseDataContext()).Update(this.SelectedCompany);
					this.EditCompanyView.Close();

					MessageBox.Show($"Информация о компании обновлена.");
				};
			}
			else
				MessageBox.Show("Заполните все поля!");
		}
		#endregion
	}
}
