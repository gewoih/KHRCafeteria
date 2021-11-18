using KHRCafeteria.ViewModels;
using System.Windows;

namespace KHRCafeteria.Views
{
	/// <summary>
	/// Логика взаимодействия для NewCompanyView.xaml
	/// </summary>
	public partial class NewCompanyView : Window
	{
		public NewCompanyView(CompaniesViewModel companiesViewModel)
		{
			InitializeComponent();

			this.DataContext = companiesViewModel;
		}
	}
}
