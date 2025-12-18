using CommunityToolkit.Maui.Alerts;
using ReminderApp.Models;
using ReminderApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace ReminderApp.ViewModels;

public class ListViewModel : BaseViewModel
{
	public ObservableCollection<ReminderItem> Reminders { get; } = new();
	public ICommand LoadRemindersCommand { get; }
	public ICommand ItemTappedCommand { get; }
	//public ICommand CheckBoxCheckedCommand { get; }
    public ICommand CompleteReminderCommand { get; } //добавлено


    private ReminderItem _selectedReminder;




	public ReminderItem SelectedReminder
	{
		get => _selectedReminder;
		set
		{
			if (SetProperty(ref _selectedReminder, value) && value != null)
			{
				ItemTappedCommand.Execute(value);
				MainThread.BeginInvokeOnMainThread(() => SelectedReminder = null);
			}
		}
	}

	public ListViewModel()
	{
		LoadRemindersCommand = new Command(async () => await LoadReminders());
		ItemTappedCommand = new Command<ReminderItem>(async (reminderItem) => await OnReminderTapped(reminderItem));
        //CheckBoxCheckedCommand = new Command<ReminderItem>(async (reminderItem) =>
        //{
        //	if (reminderItem == null)
        //		return;

        //	await OnCheckChanged(reminderItem);
        //});
        CompleteReminderCommand = new Command<ReminderItem>(async (item) =>
        {
            if (item == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Подтверждение",
                $"Отметить задачу «{item.Name}» как выполненную?",
                "Выполнено",
                "Отмена");

            if (!confirm)
                return;

            await CompleteReminderAsync(item);
        });

    }

    private async Task CompleteReminderAsync(ReminderItem item)
    {
        // 1️⃣ помечаем как выполненную
        item.Reminder.IsDone = true;

        // 2️⃣ сохраняем в БД (НЕ удаляем!)
        await App.Database.SaveReminderAsync(item.Reminder);

        // 3️⃣ убираем из списка задач
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Reminders.Remove(item);
        }); 
		
		var notifications = await App.Database.GetNotificationsByReminderIdAsync(item.Reminder.Id);

        foreach (var n in notifications)
        {
            NotificationService.CancelNotification(n.Id);
            await App.Database.DeleteNotificationAsync(n);
        }

        // 4️⃣ уведомление пользователю
        await Toast.Make("Задача выполнена").Show();
    }


    private async Task LoadReminders()
	{
		if (IsBusy)
			return;

		IsBusy = true;

		try
		{
			var reminders = await App.Database.GetRemindersAsync();
			var activeReminders = reminders?.Where(r => !r.IsDone).ToList() ?? [];

			await MainThread.InvokeOnMainThreadAsync(() =>
			{
				Reminders.Clear();
				foreach (var reminder in activeReminders)
					Reminders.Add(new ReminderItem(reminder));
			});
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error loading reminders: {ex}");
		}
		finally
		{
			IsBusy = false;
		}
	}


	//private async Task OnCheckChanged(ReminderItem reminderItem)
	//{
	//	if (reminderItem == null)
	//		return;

	//	await App.Database.SaveReminderAsync(reminderItem.Reminder);

	//	MainThread.BeginInvokeOnMainThread(() =>
	//	{
	//		if (reminderItem.Reminder.IsDone)
	//			Reminders.Remove(reminderItem);
	//	});
	//}



	private async Task OnReminderTapped(ReminderItem reminderItem)
	{
		if (reminderItem == null)
			return;

		await Shell.Current.GoToAsync($"{nameof(EditView)}?id={reminderItem.Reminder.Id}");
	}
}
