using KHRCafeteria.Commands;
using KHRCafeteria.ViewModels.Base;
using KHRCafeteria.Views;
using System;
using System.Windows.Input;

namespace KHRCafeteria.ViewModels
{
	public class MainWindowViewModel : BaseViewModel
	{
		#region Constructor
		public MainWindowViewModel()
		{
			this.ChangeMenuIndexCommand = new RelayCommand(OnChangeMenuIndexCommandExecuted, CanChangeMenuIndexCommandExecute);
		}
		#endregion

		#region Properties
		private object _MainContentControl;
		public object MainContentControl
		{
			get => _MainContentControl;
			set => Set(ref _MainContentControl, value);
		}

		private int _SelectedMenuIndex = 0;
		public int SelectedMenuIndex
		{
			get => _SelectedMenuIndex;
			set => Set(ref _SelectedMenuIndex, value);
		}
		#endregion

		#region Commands
		public ICommand ChangeMenuIndexCommand { get; }
		private bool CanChangeMenuIndexCommandExecute(object p) => true;
		private void OnChangeMenuIndexCommandExecuted(object p)
		{
			this.SelectedMenuIndex = Convert.ToInt32(p);

			switch (this.SelectedMenuIndex)
			{
				case 1:
					//
					break;
				case 2:
					if (MainContentControl is not CompaniesView)
						MainContentControl = new CompaniesView();
					break;
				case 3:
					if (MainContentControl is not EmployeesView)
						MainContentControl = new EmployeesView();
					break;
				case 4:
					//
					break;
			}
		}
		#endregion
	}
}