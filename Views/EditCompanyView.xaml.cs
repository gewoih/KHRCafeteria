using KHRCafeteria.ViewModels;
using System.Windows;

namespace KHRCafeteria.Views
{
	/// <summary>
	/// Логика взаимодействия для EditCompanyView.xaml
	/// </summary>
	public partial class EditCompanyView : Window
	{
		public EditCompanyView(CompaniesViewModel companiesViewModel)
		{
			InitializeComponent();

			this.DataContext = companiesViewModel;
		}
	}
}
