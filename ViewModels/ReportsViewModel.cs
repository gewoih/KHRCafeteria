﻿using iTextSharp.text;
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
			set
			{
				if (this.SelectedEmployee != null)
					this.SelectedEmployee = null;
				Set(ref _SelectedCompany, value);
			}
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
			set
			{
				if (this.SelectedCompany != null)
					this.SelectedCompany = null;
				Set(ref _SelectedEmployee, value);
			}
		}

		private bool _OnlyUnpaid;
		public bool OnlyUnpaid
		{
			get => _OnlyUnpaid;
			set => Set(ref _OnlyUnpaid, value);
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
		private bool CanCreateReportCommandExecute(object p) => this.StartDate != DateTime.MinValue && this.EndDate != DateTime.MinValue;
		private void OnCreateReportCommandExecuted(object p)
		{
			List<Lunch> Lunches = new List<Lunch>(new LunchesRepository(new BaseDataContext()).GetAll().Where(l => (l.DateTime.Date >= this.StartDate.Date && l.DateTime.Date <= this.EndDate.Date))).OrderByDescending(l => l.DateTime).ToList();
			string reportHeader = $"Общий отчет за период {this.StartDate.ToShortDateString()}-{this.EndDate.ToShortDateString()}";

			if (this.SelectedCompany != null)
			{
				Lunches = Lunches.Where(l => l.Employee.Company.Id == this.SelectedCompany.Id).ToList();
				reportHeader = $"Отчет по компании '{this.SelectedCompany.Name}' " +
								$"за период {this.StartDate.ToShortDateString()}-{this.EndDate.ToShortDateString()}";
			}
			else if (this.SelectedEmployee != null)
			{
				Lunches = Lunches.Where(l => l.Employee.Id == this.SelectedEmployee.Id).ToList();
				reportHeader = $"Отчет по сотруднику '{this.SelectedEmployee.Name}' " +
								$"за период {this.StartDate.ToShortDateString()}-{this.EndDate.ToShortDateString()}";
			}

			reportHeader += $"\nОбщая сумма обедов: {Lunches.Sum(l => l.Price)}р." +
							$"\nСумма оплаченных обедов: {Lunches.Where(l => l.IsPaid == true).Sum(l => l.Price)}р." +
							$"\nСумма неоплаченных обедов: {Lunches.Where(l => l.IsPaid == false).Sum(l => l.Price)}р.";

			if (this.OnlyUnpaid == true)
				Lunches = Lunches.Where(l => l.IsPaid == false).ToList();

			if (Lunches.Count == 0)
			{
				MessageBox.Show("В выбранные даты обеды не найдены!");
				return;
			}
			else
			{
				Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
				SaveFileDialog saveDialog = new SaveFileDialog();
				saveDialog.Title = "Сохранение отчета";
				saveDialog.InitialDirectory = Directory.GetCurrentDirectory();
				saveDialog.FileName = $"Отчет {DateTime.Now.ToString("F").Replace(":", " ")}";
				saveDialog.Filter = "PDF файлы | *.pdf";
				if (saveDialog.ShowDialog() == DialogResult.Cancel)
					return;

				Document doc = new Document(PageSize.A4);
				FileStream file = new FileStream(saveDialog.FileName, FileMode.OpenOrCreate);
				PdfWriter.GetInstance(doc, file);
				doc.Open();

				string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");

				BaseFont baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
				Font font = new Font(baseFont, Font.DEFAULTSIZE, Font.NORMAL);

				PdfPTable table = new PdfPTable(6);
				PdfPCell cell = new PdfPCell(new Phrase(reportHeader, font));

				cell.Colspan = 6;
				cell.HorizontalAlignment = 1;
				cell.Border = 0;
				table.AddCell(cell);

				table.AddCell(new PdfPCell(new Phrase(new Phrase("#", font))));
				table.AddCell(new PdfPCell(new Phrase(new Phrase("Компания", font))));
				table.AddCell(new PdfPCell(new Phrase(new Phrase("Сотрудник", font))));
				table.AddCell(new PdfPCell(new Phrase(new Phrase("Дата обеда", font))));
				table.AddCell(new PdfPCell(new Phrase(new Phrase("Стоимость", font))));
				table.AddCell(new PdfPCell(new Phrase(new Phrase("Оплачен", font))));

				foreach (var lunch in Lunches)
				{
					table.AddCell(new Phrase(lunch.Id.ToString(), font));
					table.AddCell(new Phrase(lunch.Employee.Company.Name, font));
					table.AddCell(new Phrase(lunch.Employee.Name, font));
					table.AddCell(new Phrase(lunch.DateTime.ToString(), font));
					table.AddCell(new Phrase(lunch.Price.ToString() + "р.", font));
					table.AddCell(new Phrase(Convert.ToInt32(lunch.IsPaid).ToString(), font));
				}

				doc.Add(table);
				doc.Close();

				MessageBox.Show("Pdf-документ сохранен");
			}
		}
		#endregion
	}
}
