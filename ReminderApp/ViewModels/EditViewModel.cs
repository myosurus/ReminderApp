using ReminderApp.Models;
using System.Windows.Input;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace ReminderApp.ViewModels;

public class EditViewModel : BaseReminderViewModel
{
	private readonly Reminder _reminder;

	public ICommand SaveCommand { get; }
	public ICommand CancelCommand { get; }
	public ICommand DeleteCommand { get; }

	private DateTime _startRemindingDate;
	public DateTime StartRemindingDate
	{
		get => _startRemindingDate;
		set => SetProperty(ref _startRemindingDate, value);
	}

	private TimeSpan _startRemindingTime;
	public TimeSpan StartRemindingTime
	{
		get => _startRemindingTime;
		set => SetProperty(ref _startRemindingTime, value);
	}

	public EditViewModel(Reminder reminder)
	{
		_reminder = reminder;

		Name = reminder.Name;
		Description = reminder.Description;
		ReminderDate = reminder.ReminderDate.Date;
		ReminderTime = reminder.ReminderDate.TimeOfDay;
		SelectedUrgency = UrgencyToText(reminder.Urgency);

		StartRemindingDate = reminder.StartReminding.Date;
		StartRemindingTime = reminder.StartReminding.TimeOfDay;

		if(reminder.RemindFrequency.TotalDays >= 1)
		{
			FrequencyValue = (int)reminder.RemindFrequency.TotalDays;
			FrequencyUnit = "дн";
		}
		else if(reminder.RemindFrequency.TotalHours >= 1)
		{
			FrequencyValue = (int)reminder.RemindFrequency.TotalHours;
			FrequencyUnit = "ч";
		}
		else
		{
			FrequencyValue = (int)reminder.RemindFrequency.TotalMinutes;
			FrequencyUnit = "мин";
		}

		SaveCommand = new Command(async () => await SaveAsync());
		CancelCommand = new Command(async () => await CancelAsync());
		DeleteCommand = new Command(async () => await DeleteAsync());
	}

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
		_reminder.StartReminding = StartRemindingDate + StartRemindingTime;
		_reminder.RemindFrequency = GetFrequencyTimeSpan();

		await App.Database.SaveReminderAsync(_reminder);

		NotificationService.CancelReminder(_reminder.Id);
		NotificationService.ScheduleReminder(_reminder);

		await Shell.Current.DisplayAlert("Успех", "Изменения сохранены", "OK");
		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task DeleteAsync()
	{
		bool confirm = await Shell.Current.DisplayAlert("Удалить", "Удалить эту задачу?", "Да", "Нет");
		if(!confirm) return;

		NotificationService.CancelReminder(_reminder.Id);
		await App.Database.DeleteReminderAsync(_reminder);
		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task CancelAsync()
	{
		bool confirm = await Shell.Current.DisplayAlert("Подтверждение", "Отменить редактирование задачи?", "Да", "Нет");
		if(confirm)
			await Shell.Current.GoToAsync("//Reminders");
	}
}

