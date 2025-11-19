namespace ReminderApp.ViewModels;

public class CalendarDay : BaseViewModel
{
	private DateTime _date;
	private bool _isCurrentMonth;
	private bool _isToday;
	private bool _hasReminders;
	private bool _isSelected;

	public DateTime Date
	{
		get => _date;
		set => SetProperty(ref _date, value);
	}

	public bool IsCurrentMonth
	{
		get => _isCurrentMonth;
		set => SetProperty(ref _isCurrentMonth, value);
	}

	public bool IsToday
	{
		get => _isToday;
		set => SetProperty(ref _isToday, value);
	}

	public bool HasReminders
	{
		get => _hasReminders;
		set => SetProperty(ref _hasReminders, value);
	}

	public bool IsSelected
	{
		get => _isSelected;
		set => SetProperty(ref _isSelected, value);
	}
}
