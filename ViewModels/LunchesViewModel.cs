using KHRCafeteria.Commands;
using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories;
using KHRCafeteria.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace KHRCafeteria.ViewModels
{
	public class LunchesViewModel : BaseViewModel
	{
		#region Constructor
		public LunchesViewModel()
		{
			this.Lunches = new ObservableCollection<Lunch>(new LunchesRepository(new BaseDataContext()).GetAll());

			this.AddLunchCommand = new RelayCommand(OnAddLunchCommandExecuted, CanAddLunchCommandExecute);
		}
		#endregion

		#region Properties
		private ObservableCollection<Lunch> _Lunches;
		public ObservableCollection<Lunch> Lunches
		{
			get => _Lunches;
			set => Set(ref _Lunches, value);
		}

		private string _CardUID;
		public string CardUID
		{
			get => _CardUID;
			set => Set(ref _CardUID, value);
		}
		#endregion

		#region Commands
		public ICommand AddLunchCommand { get; }
		private bool CanAddLunchCommandExecute(object p) => true;
		public void OnAddLunchCommandExecuted(object p)
		{
			//Проверяем наличие номера карты
			if (this.CardUID != String.Empty)
			{
				CardsRepository cardsRepository = new CardsRepository(new BaseDataContext());
				//Ищем карту с таким UID
				Card findedCard = cardsRepository.GetAll().FirstOrDefault(c => c.UID == this.CardUID);
				if (findedCard != null)
				{
					//Проверяем есть ли у найденной карты владелец. Если нет - ошибка
					if (findedCard.Employee != null)
					{
						//Добавляем новый обед с найденной картой и владельцем этой карты
						this.Lunches.Add(new LunchesRepository(new BaseDataContext()).Create(
							new Lunch
							{
								Employee = findedCard.Employee,
								Card = findedCard,
								Price = 200,
								DateTime = DateTime.Now,
								IsCompleted = findedCard.IsActive
							}));

						//Звук ошибки если карта неактивирована
						if (!findedCard.IsActive)
							new SoundPlayer(Assembly.GetExecutingAssembly().GetManifestResourceStream("Error.wav")).Play();
					}
					else
						MessageBox.Show("У данной карты нет привязки к сотруднику!");
				}
				else
					MessageBox.Show($"Карта с номером '{this.CardUID}' не найдена в системе!");
			}
			this.CardUID = String.Empty;
		}
		#endregion
	}
}
