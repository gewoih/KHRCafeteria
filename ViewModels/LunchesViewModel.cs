using KHRCafeteria.Commands;
using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories;
using KHRCafeteria.ViewModels.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Reflection;
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
			this.RemoveLunchCommand = new RelayCommand(OnRemoveLunchCommandExecuted, CanRemoveLunchCommandExecute);
			this.MarkLunchAsPaidCommand = new RelayCommand(OnMarkLunchAsPaidCommandExecuted, CanMarkLunchAsPaidCommandExecute);
			this.MarkLunchAsUnpaidCommand = new RelayCommand(OnMarkLunchAsUnpaidCommandExecuted, CanMarkLunchAsUnpaidCommandExecute);
		}
		#endregion

		#region Properties
		private ObservableCollection<Lunch> _Lunches;
		public ObservableCollection<Lunch> Lunches
		{
			get => _Lunches;
			set => Set(ref _Lunches, value);
		}

		private Lunch _SelectedLunch;
		public Lunch SelectedLunch
		{
			get => _SelectedLunch;
			set => Set(ref _SelectedLunch, value);
		}

		private string _CardUID;
		public string CardUID
		{
			get => _CardUID;
			set => Set(ref _CardUID, value);
		}
		#endregion

		#region Commands
		public ICommand MarkLunchAsPaidCommand { get; }
		private bool CanMarkLunchAsPaidCommandExecute(object p) => p != null && ((IList)p).Count != 0;
		public void OnMarkLunchAsPaidCommandExecuted(object p)
		{
			IEnumerable<Lunch> collection = ((IList)p).Cast<Lunch>();
			List<Lunch> SelectedItems = collection.ToList();

			LunchesRepository lunchesRepository = new LunchesRepository(new BaseDataContext());
			foreach (var l in SelectedItems)
			{
				l.IsPaid = true;
				lunchesRepository.Update(l);
			}
			MessageBox.Show("Пометка об оплате проставлена!");
		}

		public ICommand MarkLunchAsUnpaidCommand { get; }
		private bool CanMarkLunchAsUnpaidCommandExecute(object p) => p != null && ((IList)p).Count != 0;
		public void OnMarkLunchAsUnpaidCommandExecuted(object p)
		{
			IEnumerable<Lunch> collection = ((IList)p).Cast<Lunch>();
			List<Lunch> SelectedItems = collection.ToList();

			LunchesRepository lunchesRepository = new LunchesRepository(new BaseDataContext());
			foreach (var l in SelectedItems)
			{
				l.IsPaid = false;
				lunchesRepository.Update(l);
			}
			MessageBox.Show("Пометка об оплате снята!");
		}

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
				this.CreateLunch(findedCard);
			}
			this.CardUID = String.Empty;
		}

		public ICommand RemoveLunchCommand { get; }
		private bool CanRemoveLunchCommandExecute(object p) => this.SelectedLunch != null && this.SelectedLunch.Id != 0;
		private void OnRemoveLunchCommandExecuted(object p)
		{
			//Спрашиваем пользователя
			DialogResult dialogResult = MessageBox.Show($"Вы действительно хотите удалить обед [{this.SelectedLunch.Id}] сотрудника [{this.SelectedLunch.Employee.Name}]?",
														"Удаление обеда",
														MessageBoxButtons.YesNo);

			if (dialogResult == DialogResult.Yes)
			{
				//Сохраняем id удаляемого обеда
				int deletedLunchId = this.SelectedLunch.Id;
				//Удаляем обед из БД
				new LunchesRepository(new BaseDataContext()).Delete(this.SelectedLunch.Id);
				//Удаляем обед из списка обедов
				this.Lunches.Remove(this.SelectedLunch);

				MessageBox.Show($"Обед [{deletedLunchId}] успешно удален.");
			}
		}
		#endregion

		#region Methods
		private void CreateLunch(Card findedCard)
		{
			Lunch newLunch = new Lunch
			{
				Employee = findedCard == null ? null : findedCard.Employee,
				Price = findedCard == null ? 0 : findedCard.Employee.Company.LunchPrice,
				DateTime = DateTime.Now
			};

			//Если карта активна - добавляем обед в базу, если неактивна - выдаем звук с ошибкой
			if (findedCard != null && findedCard.Employee != null && findedCard.IsActive == true)
				new LunchesRepository(new BaseDataContext()).Create(newLunch);
			else
				new SoundPlayer(Assembly.GetExecutingAssembly().GetManifestResourceStream("Error.wav")).Play();

			//Добавляем обед в список обедов
			this.Lunches.Add(newLunch);
		}
		#endregion
	}
}
