using System.Windows.Input;
using ReminderApp.Models;

namespace ReminderApp.ViewModels;

public class EditViewModel : BaseViewModel
{
	private readonly Reminder _reminder;

	private string _name;
	private string _description;
	private DateTime _reminderDate;
	private TimeSpan _reminderTime;
	private string _selectedUrgency;

	public List<string> UrgencyOptions { get; } = new() { "Низкий", "Средний", "Высокий" };

	public string PageTitle => "Редактировать задачу";
	public string SaveButtonText => "Сохранить изменения";
	public bool IsDeleteVisible => true;

	public string Name { get => _name; set => SetProperty(ref _name, value); }
	public string Description { get => _description; set => SetProperty(ref _description, value); }
	public DateTime ReminderDate { get => _reminderDate; set => SetProperty(ref _reminderDate, value); }
	public TimeSpan ReminderTime { get => _reminderTime; set => SetProperty(ref _reminderTime, value); }
	public string SelectedUrgency { get => _selectedUrgency; set => SetProperty(ref _selectedUrgency, value); }

	public ICommand SaveCommand { get; }
	public ICommand DeleteCommand { get; }
	public ICommand CancelCommand { get; }

	public EditViewModel(Reminder reminder)
	{
		_reminder = reminder;

		Name = reminder.Name;
		Description = reminder.Description;
		ReminderDate = reminder.ReminderDate.Date;
		ReminderTime = reminder.ReminderDate.TimeOfDay;
		SelectedUrgency = UrgencyToText(reminder.Urgency);

		SaveCommand = new Command(async () => await SaveAsync());
		DeleteCommand = new Command(async () => await DeleteAsync());
		CancelCommand = new Command(async () => await CancelAsync());
	}

	private string UrgencyToText(Urgency urgency) => urgency switch
	{
		Urgency.Low => "Низкий",
		Urgency.Medium => "Средний",
		Urgency.High => "Высокий",
		_ => "Средний"
	};

	private Urgency TextToUrgency(string urgency) => urgency switch
	{
		"Низкий" => Urgency.Low,
		"Средний" => Urgency.Medium,
		"Высокий" => Urgency.High,
		_ => Urgency.Medium
	};

	private async Task SaveAsync()
	{
		if(string.IsNullOrWhiteSpace(Name))
		{
			await Shell.Current.DisplayAlert("Ошибка", "Введите название задачи", "OK");
			return;
		}

		_reminder.Name = Name;
		_reminder.Description = Description;
		_reminder.ReminderDate = ReminderDate.Date + ReminderTime;
		_reminder.Urgency = TextToUrgency(SelectedUrgency);

		await App.Database.SaveReminderAsync(_reminder);
		await Shell.Current.DisplayAlert("Успех", "Изменения сохранены", "OK");
		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task DeleteAsync()
	{
		bool confirm = await Shell.Current.DisplayAlert("Удалить", "Удалить эту задачу?", "Да", "Нет");
		if(!confirm) return;

		await App.Database.DeleteReminderAsync(_reminder);
		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task CancelAsync()
	{
		await Shell.Current.GoToAsync("//Reminders");
	}
}
