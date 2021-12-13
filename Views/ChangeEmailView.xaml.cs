using KHRCafeteria.ViewModels;
using System.Windows;

namespace KHRCafeteria.Views
{
	/// <summary>
	/// Логика взаимодействия для ChangeEmailView.xaml
	/// </summary>
	public partial class ChangeEmailView : Window
	{
		public ChangeEmailView(ReportsViewModel reportsViewModel)
		{
			InitializeComponent();

			this.DataContext = reportsViewModel;
		}
	}
}
