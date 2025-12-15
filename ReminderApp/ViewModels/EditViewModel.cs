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

		StartRemindingDate = reminder.StartReminding.Date;
		StartRemindingTime = reminder.StartReminding.TimeOfDay;

		if (reminder.RemindFrequency.TotalDays >= 1)
		{
			FrequencyValue = (int)reminder.RemindFrequency.TotalDays;
			FrequencyUnit = "дн";
		}
		else if (reminder.RemindFrequency.TotalHours >= 1)
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
		if (string.IsNullOrWhiteSpace(Name))
		{
			await Shell.Current.DisplayAlert("Ошибка", "Введите название задачи", "OK");
			return;
		}

		_reminder.Name = Name;
		_reminder.Description = Description;
		_reminder.ReminderDate = ReminderDate.Date + ReminderTime;
		_reminder.StartReminding = StartRemindingDate + StartRemindingTime;
        _reminder.RemindFrequency = GetFrequencyTimeSpan();
        while(_reminder.StartReminding < DateTime.Now) _reminder.StartReminding += _reminder.RemindFrequency;

        await App.Database.SaveReminderAsync(_reminder);

        var notifications = await App.Database.GetNotificationsByReminderIdAsync(_reminder.Id);

        foreach (var n in notifications)
        {
            NotificationService.CancelNotification(n.Id);

            await App.Database.SaveNotificationAsync(n);

            NotificationService.ScheduleReminder(_reminder, n.Id);
        }

        await Shell.Current.DisplayAlert("Успех", "Изменения сохранены", "OK");
		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task DeleteAsync()
	{
		bool confirm = await Shell.Current.DisplayAlert("Удалить", "Удалить эту задачу?", "Да", "Нет");
		if (!confirm) return;

        var notifications = await App.Database.GetNotificationsByReminderIdAsync(_reminder.Id);

        foreach (var n in notifications)
        {
            NotificationService.CancelNotification(n.Id);
            await App.Database.DeleteNotificationAsync(n);
        }

        await App.Database.DeleteReminderAsync(_reminder);

		await Shell.Current.GoToAsync("//Reminders");
	}

	private async Task CancelAsync()
	{
		bool confirm = await Shell.Current.DisplayAlert("Подтверждение", "Отменить редактирование задачи?", "Да", "Нет");
		if (confirm)
			await Shell.Current.GoToAsync("//Reminders");
	}


	public Color UrgencyColor => GetUrgencyColor(); //добавлено
	public Urgency Urgency => GetUrgency(); //добавлено

	public bool IsDone //добавлено
	{
		get => _reminder.IsDone;
		set
		{
			_reminder.IsDone = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(UrgencyColor));
			OnPropertyChanged(nameof(Urgency));
		}
	}

	private Urgency GetUrgency() //добавлено
	{
		var daysLeft = (ReminderDate.Date - DateTime.Today).TotalDays;

		if (IsDone) return Urgency.Low;
		if (daysLeft < 0) return Urgency.High;
		if (daysLeft == 0) return Urgency.High;
		if (daysLeft <= 1) return Urgency.High;
		if (daysLeft <= 3) return Urgency.Medium;

		return Urgency.Low;
	}

	private Color GetUrgencyColor() //добавлено
	{
		if (IsDone) return Color.FromArgb("#BDBDBD");

		var daysLeft = (ReminderDate.Date - DateTime.Today).TotalDays;

		if (daysLeft < 0) return Color.FromArgb("#FF0000");
		if (daysLeft == 0) return Color.FromArgb("#F44336");
		if (daysLeft <= 1) return Color.FromArgb("#ffaa22");
		if (daysLeft <= 3) return Color.FromArgb("#ffed22");

		return Color.FromArgb("#4CAF50");
	}

	public string UrgencyText =>
	Urgency switch
	{
		Urgency.High => "Срочно",
		Urgency.Medium => "Важно",
		Urgency.Low => "Не срочно",
		_ => "—"
	};

	public string DueStatusText
	{
        //get
        //{
        //	var now = DateTime.Now;
        //	var deadline = ReminderDate + ReminderTime;

        //	if (deadline < now)
        //		return "Срок истёк";

        //	var days = (deadline - now).TotalDays;

        //	if (days < 1)
        //		return "Сегодня";

        //	if (days < 2)
        //		return "До окончания: 1 день";

        //	return $"До окончания: {Math.Floor(days)} дней";
        //}
        get
        {
            var now = DateTime.Now;
            var deadline = ReminderDate + ReminderTime;

            // 1) Просрочено
            if (deadline < now)
                return "Срок истёк";

            var daysLeft = (deadline.Date - now.Date).TotalDays;

            // 2) Сегодня (0 дней)
            if (daysLeft == 0)
                return "Сегодня";

            // 3) Завтра (1 день)
            if (daysLeft == 1)
                return "Остался 1 день";

            // 4) 2–3 дня
            if (daysLeft <= 3)
                return $"Осталось {daysLeft} дня";

            // 5) Больше чем 3 дня
            return $"Осталось {daysLeft} дней";
        }

    }
}

