using iTextSharp.text;
using iTextSharp.text.pdf;
using KHRCafeteria.Commands;
using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories;
using KHRCafeteria.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace KHRCafeteria.ViewModels
{
	public class ReportsViewModel : BaseViewModel
	{
		#region Constructor
		public ReportsViewModel()
		{
			this.Companies = new ObservableCollection<Company>(new CompaniesRepository(new BaseDataContext()).GetAll());
			this.Employees = new ObservableCollection<Employee>(new EmployeesRepository(new BaseDataContext()).GetAll());

			this.SelectionCommand = new RelayCommand(OnSelectionCommandExecuted, CanSelectionCommandExecute);
			this.CreateReportCommand = new RelayCommand(OnCreateReportCommandExecuted, CanCreateReportCommandExecute);
		}
		#endregion

		#region Properties
		private DateTime _StartDate;
		public DateTime StartDate
		{
			get => _StartDate;
			set => Set(ref _StartDate, value);
		}

		private DateTime _EndDate;
		public DateTime EndDate
		{
			get => _EndDate;
			set => Set(ref _EndDate, value);
		}

		private ObservableCollection<Company> _Companies;
		public ObservableCollection<Company> Companies
		{
			get => _Companies;
			set => Set(ref _Companies, value);
		}

		private Company _SelectedCompany;
		public Company SelectedCompany
		{
			get => _SelectedCompany;
			set => Set(ref _SelectedCompany, value);
		}

		private ObservableCollection<Employee> _Employees;
		public ObservableCollection<Employee> Employees
		{
			get => _Employees;
			set => Set(ref _Employees, value);
		}

		private Employee _SelectedEmployee;
		public Employee SelectedEmployee
		{
			get => _SelectedEmployee;
			set => Set(ref _SelectedEmployee, value);
		}
		#endregion

		#region Commands
		public ICommand SelectionCommand { get; }
		private bool CanSelectionCommandExecute(object p) => true;
		private void OnSelectionCommandExecuted(object p)
		{
			this.StartDate = ((SelectedDatesCollection)p).First();
			this.EndDate = ((SelectedDatesCollection)p).Last();
		}

		public ICommand CreateReportCommand { get; }
		private bool CanCreateReportCommandExecute(object p) => this.StartDate != DateTime.MinValue && this.EndDate != DateTime.MinValue && (this.SelectedEmployee != null || this.SelectedCompany != null);
		private void OnCreateReportCommandExecuted(object p)
		{
			List<Lunch> Lunches = new List<Lunch>(new LunchesRepository(new BaseDataContext()).GetAll().Where(l => (l.DateTime.Date >= this.StartDate.Date && l.DateTime.Date <= this.EndDate.Date)));

			if (Lunches.Count == 0)
			{
				MessageBox.Show("В выбранные даты обеды не найдены!");
				return;
			}
			else
			{
				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

				Document doc = new Document(PageSize.A4);
				FileStream file = new FileStream("C:\\Users\\ranenko\\Desktop\\NewReport.pdf", FileMode.OpenOrCreate);
				PdfWriter.GetInstance(doc, file);
				doc.Open();

				string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");

				BaseFont baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
				Font font = new Font(baseFont, Font.DEFAULTSIZE, Font.NORMAL);

				PdfPTable table = new PdfPTable(5);
				PdfPCell cell = new PdfPCell(new Phrase($"Отчет за {DateTime.Now}. Сумма обедов: {Lunches.Sum(l => l.Price)}р.", font));

				cell.Colspan = 5;
				cell.HorizontalAlignment = 1;
				cell.Border = 0;
				table.AddCell(cell);

				table.AddCell(new PdfPCell(new Phrase(new Phrase("#", font))));
				table.AddCell(new PdfPCell(new Phrase(new Phrase("Компания", font))));
				table.AddCell(new PdfPCell(new Phrase(new Phrase("Сотрудник", font))));
				table.AddCell(new PdfPCell(new Phrase(new Phrase("Дата обеда", font))));
				table.AddCell(new PdfPCell(new Phrase(new Phrase("Стоимость", font))));

				foreach (var lunch in Lunches)
				{
					table.AddCell(new Phrase(lunch.Id.ToString(), font));
					table.AddCell(new Phrase(lunch.Employee.Company.Name, font));
					table.AddCell(new Phrase(lunch.Employee.Name, font));
					table.AddCell(new Phrase(lunch.DateTime.ToString(), font));
					table.AddCell(new Phrase(lunch.Price.ToString() + "р.", font));
				}

				doc.Add(table);
				doc.Close();

				MessageBox.Show("Pdf-документ сохранен");
			}
		}
		#endregion
	}
}
