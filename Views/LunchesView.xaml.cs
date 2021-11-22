using System.ComponentModel;
using System.Windows.Controls;

namespace KHRCafeteria.Views
{
	/// <summary>
	/// Логика взаимодействия для LunchesView.xaml
	/// </summary>
	public partial class LunchesView : UserControl
	{
		public LunchesView()
		{
			InitializeComponent();

			this.DataGrid.Items.SortDescriptions.Add(new SortDescription("DateTime", ListSortDirection.Descending));
		}
	}
}
