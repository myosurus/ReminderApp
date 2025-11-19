using ReminderApp.Models;
using System.Windows.Input;

namespace ReminderApp.ViewModels;

public class EditViewModel : BaseReminderViewModel
{
	private readonly Reminder _reminder;

	public ICommand SaveCommand { get; }
	public ICommand CancelCommand { get; }
	public ICommand DeleteCommand { get; }

	public EditViewModel(Reminder reminder)
	{
		_reminder = reminder;

		Name = reminder.Name;
		Description = reminder.Description;
		ReminderDate = reminder.ReminderDate.Date;
		ReminderTime = reminder.ReminderDate.TimeOfDay;
		SelectedUrgency = UrgencyToText(reminder.Urgency);

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

		await App.Database.SaveReminderAsync(_reminder);
		await Shell.Current.DisplayAlert("Успех", "Изменения сохранены", "OK");

		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task DeleteAsync()
	{
		bool confirm = await Shell.Current.DisplayAlert(
			"Удалить",
			"Удалить эту задачу?",
			"Да", "Нет");

		if(!confirm) return;

		await App.Database.DeleteReminderAsync(_reminder);
		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task CancelAsync()
	{
		bool confirm = await Shell.Current.DisplayAlert(
			"Подтверждение",
			"Отменить редактирование задачи?",
			"Да", "Нет");

		if(confirm)
			await Shell.Current.GoToAsync("//Reminders");
	}
}
