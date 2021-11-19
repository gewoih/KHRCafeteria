using KHRCafeteria.ViewModels;
using System.Windows;

namespace KHRCafeteria.Views
{
	/// <summary>
	/// Логика взаимодействия для NewEmployeeView.xaml
	/// </summary>
	public partial class NewEmployeeView : Window
	{
		public NewEmployeeView(EmployeesViewModel viewModel)
		{
			InitializeComponent();

			this.DataContext = viewModel;
		}
	}
}
