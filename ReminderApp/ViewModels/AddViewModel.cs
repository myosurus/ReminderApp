using System.Windows.Input;
using ReminderApp.Models;

namespace ReminderApp.ViewModels;

public class AddViewModel : BaseViewModel
{
	private string _name;
	private string _description;
	private DateTime _reminderDate = DateTime.Now;
	private TimeSpan _reminderTime = DateTime.Now.TimeOfDay;
	private string _selectedUrgency;

	public List<string> UrgencyOptions { get; } = new() { "Низкий", "Средний", "Высокий" };

	public string PageTitle => "Добавить задачу";
	public string SaveButtonText => "Добавить задачу";
	public bool IsDeleteVisible => false;

	public string Name { get => _name; set => SetProperty(ref _name, value); }
	public string Description { get => _description; set => SetProperty(ref _description, value); }
	public DateTime ReminderDate { get => _reminderDate; set => SetProperty(ref _reminderDate, value); }
	public TimeSpan ReminderTime { get => _reminderTime; set => SetProperty(ref _reminderTime, value); }
	public string SelectedUrgency { get => _selectedUrgency; set => SetProperty(ref _selectedUrgency, value); }

	public ICommand SaveCommand { get; }
	public ICommand CancelCommand { get; }

	public AddViewModel()
	{
		SaveCommand = new Command(async () => await SaveAsync());
		CancelCommand = new Command(async () => await CancelAsync());
	}

	private Urgency ParseUrgency(string urgency) => urgency switch
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

		var reminder = new Reminder
		{
			Name = Name,
			Description = Description,
			ReminderDate = ReminderDate.Date + ReminderTime,
			Urgency = ParseUrgency(SelectedUrgency),
			IsDone = false
		};

		await App.Database.CreateReminderAsync(reminder);
		await Shell.Current.DisplayAlert("Успех", "Задача успешно сохранена", "OK");
		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task CancelAsync()
	{
		bool confirm = await Shell.Current.DisplayAlert("Подтверждение", "Отменить создание задачи?", "Да", "Нет");
		if(confirm)
			await Shell.Current.GoToAsync("//Reminders");
	}
}
