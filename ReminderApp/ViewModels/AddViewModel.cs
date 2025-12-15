using ReminderApp.Models;
using System.Windows.Input;

namespace ReminderApp.ViewModels;

public class AddViewModel : BaseReminderViewModel
{
	public ICommand SaveCommand { get; }
	public ICommand CancelCommand { get; }

	private DateTime _startRemindingDate = DateTime.Now.Date;
	public DateTime StartRemindingDate
	{
		get => _startRemindingDate;
		set => SetProperty(ref _startRemindingDate, value);
	}

	private TimeSpan _startRemindingTime = DateTime.Now.TimeOfDay;
	public TimeSpan StartRemindingTime
	{
		get => _startRemindingTime;
		set => SetProperty(ref _startRemindingTime, value);
	}

	public AddViewModel()
	{
		SaveCommand = new Command(async () => await SaveAsync());
		CancelCommand = new Command(async () => await CancelAsync());
	}

	private async Task SaveAsync()
	{
		if(string.IsNullOrWhiteSpace(Name))
		{
			await Shell.Current.DisplayAlert("Ошибка", "Введите название задачи", "OK");
			return;
		}

		var newReminder = new Reminder
		{
			Name = Name,
			Description = Description,
			ReminderDate = ReminderDate.Date + ReminderTime,
			//Urgency = TextToUrgency(SelectedUrgency),
			IsDone = false,
			StartReminding = StartRemindingDate + StartRemindingTime,
			RemindFrequency = GetFrequencyTimeSpan()
        };

		await App.Database.CreateReminderAsync(newReminder);
		NotificationService.ScheduleReminder(newReminder);

		await Shell.Current.DisplayAlert("Успех", "Задача добавлена", "OK");
		ResetFields();
		await Shell.Current.GoToAsync("..");
		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task CancelAsync()
	{
		bool confirm = await Shell.Current.DisplayAlert("Подтверждение", "Отменить создание задачи?", "Да", "Нет");
		if(confirm)
		{
			ResetFields();
			await Shell.Current.GoToAsync("..");
			await Shell.Current.GoToAsync("//Reminders");
		}
	}

	private void ResetFields()
	{
		Name = string.Empty;
		Description = string.Empty;
		ReminderDate = DateTime.Now;
		ReminderTime = DateTime.Now.TimeOfDay;
		StartRemindingDate = DateTime.Now.Date;
		StartRemindingTime = DateTime.Now.TimeOfDay;
		FrequencyValue = 30;
		FrequencyUnit = "мин";
	}
}