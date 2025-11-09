using ReminderApp.Models;
using ReminderApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace ReminderApp.ViewModels;

public class ListViewModel : BaseViewModel
{
	public ObservableCollection<ReminderItem> Reminders { get; } = new();
	public ICommand LoadRemindersCommand { get; }
	public ICommand ItemTappedCommand { get; }
	public ICommand CheckBoxCheckedCommand { get; }

	private ReminderItem _selectedReminder;
	public ReminderItem SelectedReminder
	{
		get => _selectedReminder;
		set
		{
			if(SetProperty(ref _selectedReminder, value) && value != null)
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
		CheckBoxCheckedCommand = new Command<ReminderItem>(async (reminderItem) =>
		{
			if(reminderItem == null)
				return;

			await OnCheckChanged(reminderItem);
		});

	}

	private async Task LoadReminders()
	{
		if(IsBusy)
			return;

		IsBusy = true;

		try
		{
			var reminders = await App.Database.GetRemindersAsync();
			var activeReminders = reminders?.Where(r => !r.IsDone).ToList() ?? [];

			await MainThread.InvokeOnMainThreadAsync(() =>
			{
				Reminders.Clear();
				foreach(var reminder in activeReminders)
					Reminders.Add(new ReminderItem(reminder));
			});
		}
		catch(Exception ex)
		{
			Debug.WriteLine($"Error loading reminders: {ex}");
		}
		finally
		{
			IsBusy = false;
		}
	}


	private async Task OnCheckChanged(ReminderItem reminderItem)
	{
		if(reminderItem == null)
			return;

		await App.Database.SaveReminderAsync(reminderItem.Reminder);

		MainThread.BeginInvokeOnMainThread(() =>
		{
			if(reminderItem.Reminder.IsDone)
				Reminders.Remove(reminderItem);
		});
	}

	private async Task OnReminderTapped(ReminderItem reminderItem)
	{
		if(reminderItem == null)
			return;

		await Shell.Current.GoToAsync(nameof(DetailView), new Dictionary<string, object>
		{
			["Reminder"] = reminderItem.Reminder
		});
	}
}
