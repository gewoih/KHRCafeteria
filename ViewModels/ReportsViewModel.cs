using iTextSharp.text;
using iTextSharp.text.pdf;
using KHRCafeteria.Commands;
using KHRCafeteria.DataContext;
using KHRCafeteria.Models;
using KHRCafeteria.Repositories;
using KHRCafeteria.ViewModels.Base;
using KHRCafeteria.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
			this.Companies = new ObservableCollection<Company>(new CompaniesRepository(new BaseDataContext()).GetAll().OrderBy(c => c.Name));
			this.Employees = new ObservableCollection<Employee>(new EmployeesRepository(new BaseDataContext()).GetAll().OrderBy(e => e.Name));

			this.SelectionCommand = new RelayCommand(OnSelectionCommandExecuted, CanSelectionCommandExecute);
			this.CreateReportCommand = new RelayCommand(OnCreateReportCommandExecuted, CanCreateReportCommandExecute);
			this.ShowChangeEmailWindowCommand = new RelayCommand(OnShowChangeEmailWindowCommandExecute, CanShowChangeEmailWindowCommandExecuted);
			this.SaveNewEmailCommand = new RelayCommand(OnSaveNewEmailCommandExecute, CanSaveNewEmailCommandExecuted);
		}
		#endregion

		#region Properties
		private ChangeEmailView ChangeEmailView;

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

		private bool _SendToEmail;
		public bool SendToEmail
		{
			get => _SendToEmail;
			set => Set(ref _SendToEmail, value);
		}

		private string _Email;
		public string Email
		{
			get => _Email;
			set => Set(ref _Email, value);
		}

		private string _Password;
		public string Password
		{
			get => _Password;
			set => Set(ref _Password, value);
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
				System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
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

				if (this.SendToEmail == true)
				{
					if (this.SendReportToEmail(saveDialog.FileName))
						MessageBox.Show("Отчет отправлен");
					else
						MessageBox.Show("При отправке отчета произошла ошибка!");
				}
			}
		}

		public ICommand ShowChangeEmailWindowCommand { get; }
		private bool CanShowChangeEmailWindowCommandExecuted(object p) => true;
		private void OnShowChangeEmailWindowCommandExecute(object p)
		{
			this.ChangeEmailView = new ChangeEmailView(this);
			this.ChangeEmailView.ShowDialog();
		}

		public ICommand SaveNewEmailCommand { get; }
		private bool CanSaveNewEmailCommandExecuted(object p) => true;
		private void OnSaveNewEmailCommandExecute(object p)
		{
			if (!String.IsNullOrEmpty(this.Email) && !String.IsNullOrEmpty(this.Password))
			{
				FileStream fstream = new FileStream($"config.bin", FileMode.OpenOrCreate);
				
				byte[] array = System.Text.Encoding.Default.GetBytes($"{this.Email};{this.Password}");
				fstream.Write(array, 0, array.Length);
				fstream.Close();
				MessageBox.Show("Email сохранен.");

				this.ChangeEmailView.Close();
			}
			else
				MessageBox.Show("Заполните все поля!");
		}
		#endregion

		#region Methods
		private bool SendReportToEmail(string reportPath)
		{
			string toMail;
			if (this.SelectedCompany != null)
				toMail = this.SelectedCompany.Email;
			else if (this.SelectedEmployee != null)
				toMail = this.SelectedEmployee.Email;
			else
				return false;

			if (String.IsNullOrEmpty(toMail))
				return false;
			else
			{
				FileStream fstream = File.OpenRead("config.bin");
				byte[] array = new byte[fstream.Length];
				fstream.Read(array, 0, array.Length);
				string textFromFile = System.Text.Encoding.Default.GetString(array);
				fstream.Close();
				if (String.IsNullOrEmpty(textFromFile))
					return false;

				try
				{
					MailAddress from = new MailAddress(textFromFile.Split(';')[0], "КХ 'Россия'");
					MailAddress to = new MailAddress(toMail);
					MailMessage mail = new MailMessage(from, to);

					mail.Subject = "Отчет по обедам";
					mail.Attachments.Add(new Attachment(reportPath));

					SmtpClient smtp = new SmtpClient("mail.hosting.reg.ru", 587);
					smtp.Credentials = new NetworkCredential(textFromFile.Split(';')[0], textFromFile.Split(';')[1]);
					smtp.EnableSsl = false;

					smtp.Send(mail);
				}
				catch
				{
					return false;
				}
			}

			return true;
		}
		#endregion
	}
}
