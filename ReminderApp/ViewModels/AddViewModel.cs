using ReminderApp.Models;
using System.Windows.Input;

namespace ReminderApp.ViewModels;

public class AddViewModel : BaseReminderViewModel
{
	public ICommand SaveCommand { get; }
	public ICommand CancelCommand { get; }

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
			Urgency = TextToUrgency(SelectedUrgency),
			IsDone = false
		};

		await App.Database.CreateReminderAsync(newReminder);
		await Shell.Current.DisplayAlert("Успех", "Задача добавлена", "OK");

		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task CancelAsync()
	{
		bool confirm = await Shell.Current.DisplayAlert(
			"Подтверждение",
			"Отменить создание задачи?",
			"Да", "Нет");

		if(confirm)
			await Shell.Current.GoToAsync("//Reminders");
	}
}


