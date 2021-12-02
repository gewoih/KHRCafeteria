using KHRCafeteria.ViewModels;
using System.Windows;

namespace KHRCafeteria.Views
{
	/// <summary>
	/// Логика взаимодействия для EditEmployeeView.xaml
	/// </summary>
	public partial class EditEmployeeView : Window
	{
		public EditEmployeeView(EmployeesViewModel employeesViewModel)
		{
			InitializeComponent();

			this.DataContext = employeesViewModel;
		}
	}
}
