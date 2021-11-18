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
			this.NewCompany = new Company();

			this.ShowNewCompanyWindowCommand = new RelayCommand(OnShowNewCompanyWindowCommandExecuted, CanShowNewCompanyWindowCommandExecute);
			this.AddCompanyCommand = new RelayCommand(OnAddCompanyCommandExecuted, CanAddCompanyCommandExecute);
			this.RemoveCompanyCommand = new RelayCommand(OnRemoveCompanyCommandExecuted, CanRemoveCompanyCommandExecute);
		}
		#endregion

		#region Properties
		private ObservableCollection<Company> _Companies;
		public ObservableCollection<Company> Companies
		{
			get => _Companies;
			set => Set(ref _Companies, value);
		}

		private NewCompanyView _NewCompanyView;
		public NewCompanyView NewCompanyView
		{
			get => _NewCompanyView;
			set => Set(ref _NewCompanyView, value);
		}

		private Company _NewCompany;
		public Company NewCompany
		{
			get => _NewCompany;
			set => Set(ref _NewCompany, value);
		}

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
			this.NewCompanyView = new NewCompanyView(this);
			this.NewCompanyView.ShowDialog();
		}

		public ICommand AddCompanyCommand { get; }
		private bool CanAddCompanyCommandExecute(object p) => true;
		private void OnAddCompanyCommandExecuted(object p)
		{
			if (this.NewCompany.Name != String.Empty)
			{
				CompaniesRepository repository = new CompaniesRepository(new BaseDataContext());
				if (repository.GetAll().FirstOrDefault(c => c.Name == this.NewCompany.Name) == null)
				{
					this.Companies.Add(repository.Create(this.NewCompany));
					this.NewCompany = new Company();
					this.NewCompanyView.Close();

					MessageBox.Show($"Компания '{this.NewCompany.Name}' успешно добавлена.");
				}
				else
					MessageBox.Show("Компания с таким названием уже существует.");
			}
			else
				MessageBox.Show("Введите название компании!");
		}

		public ICommand RemoveCompanyCommand { get; }
		private bool CanRemoveCompanyCommandExecute(object p) => SelectedCompany != null;
		private void OnRemoveCompanyCommandExecuted(object p)
		{
			DialogResult dialogResult = MessageBox.Show($"Вы действительно хотите удалить компанию {this.SelectedCompany.Name}?", 
														"Удаление компании", 
														MessageBoxButtons.YesNo);

			if (dialogResult == DialogResult.Yes)
			{
				CompaniesRepository repository = new CompaniesRepository(new BaseDataContext());
				repository.Delete(this.SelectedCompany.Id);
				MessageBox.Show($"Компания '{this.SelectedCompany.Name}' успешно удалена.");
				this.Companies.Remove(this.SelectedCompany);
			}
		}
		#endregion
	}
}
