using System.Windows.Input;
using ReminderApp.Models;

namespace ReminderApp.ViewModels;

public class DetailViewModel : BaseViewModel
{
	private readonly Reminder _reminder;
	private bool _isEditMode;

	private string _name;
	private string _description;
	private DateTime _reminderDate = DateTime.Now;
	private TimeSpan _reminderTime = DateTime.Now.TimeOfDay;
	private string _selectedUrgency = "Средний";

	public List<string> UrgencyOptions { get; } = new() { "Низкий", "Средний", "Высокий" };

	public string PageTitle => _isEditMode ? "Редактировать задачу" : "Добавить задачу";
	public string SaveButtonText => _isEditMode ? "Сохранить изменения" : "Добавить задачу";
	public bool IsDeleteVisible => _isEditMode;

	public string Name { get => _name; set => SetProperty(ref _name, value); }
	public string Description { get => _description; set => SetProperty(ref _description, value); }
	public DateTime ReminderDate { get => _reminderDate; set => SetProperty(ref _reminderDate, value); }
	public TimeSpan ReminderTime { get => _reminderTime; set => SetProperty(ref _reminderTime, value); }
	public string SelectedUrgency { get => _selectedUrgency; set => SetProperty(ref _selectedUrgency, value); }

	public ICommand SaveCommand { get; }
	public ICommand CancelCommand { get; }
	public ICommand DeleteCommand { get; }

	public DetailViewModel(Reminder reminder = null)
	{
		_reminder = reminder;
		_isEditMode = reminder != null;

		if(_isEditMode)
		{
			Name = _reminder.Name;
			Description = _reminder.Description;
			ReminderDate = _reminder.ReminderDate.Date;
			ReminderTime = _reminder.ReminderDate.TimeOfDay;
			SelectedUrgency = UrgencyToText(_reminder.Urgency);
		}

		SaveCommand = new Command(async () => await SaveAsync());
		CancelCommand = new Command(async () => await CancelAsync());
		DeleteCommand = new Command(async () => await DeleteAsync());
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

		if(_isEditMode)
		{
			_reminder.Name = Name;
			_reminder.Description = Description;
			_reminder.ReminderDate = ReminderDate.Date + ReminderTime;
			_reminder.Urgency = TextToUrgency(SelectedUrgency);

			await App.Database.SaveReminderAsync(_reminder);
			await Shell.Current.DisplayAlert("Успех", "Изменения сохранены", "OK");
		}
		else
		{
			var newReminder = new Reminder
			{
				Name = Name,
				Description = Description,
				ReminderDate = ReminderDate.Date + ReminderTime,
				Urgency = TextToUrgency(SelectedUrgency),
				IsDone = false
			};

			await App.Database.CreateReminderAsync(newReminder);
			await Shell.Current.DisplayAlert("Успех", "Задача успешно добавлена", "OK");
		}

		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task DeleteAsync()
	{
		if(!_isEditMode) return;

		bool confirm = await Shell.Current.DisplayAlert("Удалить", "Удалить эту задачу?", "Да", "Нет");
		if(!confirm) return;

		await App.Database.DeleteReminderAsync(_reminder);
		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task CancelAsync()
	{
		bool confirm = _isEditMode
			? await Shell.Current.DisplayAlert("Подтверждение", "Отменить редактирование задачи?", "Да", "Нет")
			: await Shell.Current.DisplayAlert("Подтверждение", "Отменить создание задачи?", "Да", "Нет");

		if(confirm)
			await Shell.Current.GoToAsync("//Reminders");
	}
}

