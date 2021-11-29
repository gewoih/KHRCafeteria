using KHRCafeteria.Commands;
using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories;
using KHRCafeteria.ViewModels.Base;
using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace KHRCafeteria.ViewModels
{
	public class LunchPricesViewModel : BaseViewModel
	{
		#region Constructor
		public LunchPricesViewModel()
		{
			this.NewLunchPrice = new LunchPrice();
			this.AddLunchPriceCommand = new RelayCommand(OnAddLunchPriceCommandExecuted, CanAddLunchPriceCommandExecute);
		}
		#endregion

		private LunchPrice _NewLunchPrice;
		public LunchPrice NewLunchPrice
		{
			get => _NewLunchPrice;
			set => Set(ref _NewLunchPrice, value);
		}

		public ICommand AddLunchPriceCommand { get; }
		private bool CanAddLunchPriceCommandExecute(object p) => true;
		private void OnAddLunchPriceCommandExecuted(object p)
		{
			new LunchPricesRepository(new BaseDataContext()).Create(new LunchPrice { Price = this.NewLunchPrice.Price, DateTime = DateTime.Now });
			MessageBox.Show("Стоимость обеда изменена.");
		}
	}
}
