using System.Collections.ObjectModel;
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
				SelectedReminder = null; 
			}
		}
	}

	public ListViewModel()
	{
		LoadRemindersCommand = new Command(async () => await LoadReminders());
		ItemTappedCommand = new Command<ReminderItem>(async (reminderItem) => await OnReminderTapped(reminderItem));
		CheckBoxCheckedCommand = new Command<ReminderItem>(async (reminderItem) => await OnCheckChanged(reminderItem));
	}

	private async Task LoadReminders()
	{
		if(IsBusy)
			return;

		IsBusy = true;

		try
		{
			Reminders.Clear();

			var reminders = await App.Database.GetRemindersAsync();
			var activeReminders = reminders?.Where(r => !r.IsDone).ToList() ?? [];

			foreach(var reminder in activeReminders)
				Reminders.Add(new ReminderItem(reminder));
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task OnReminderTapped(ReminderItem reminderItem)
	{
		if(reminderItem == null)
			return;

		await Shell.Current.GoToAsync(nameof(ReminderDetailPage), new Dictionary<string, object>
		{
			["Reminder"] = reminderItem.Reminder
		});
	}

	private async Task OnCheckChanged(ReminderItem reminderItem)
	{
		if(reminderItem == null)
			return;

		reminderItem.Reminder.IsDone = !reminderItem.Reminder.IsDone;
		await App.Database.SaveReminderAsync(reminderItem.Reminder);
		await LoadReminders();
	}
}
