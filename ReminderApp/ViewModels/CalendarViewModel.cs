using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using ReminderApp.Models;
using ReminderApp.Views;

namespace ReminderApp.ViewModels;

public class CalendarViewModel : BaseViewModel
{
	private DateTime _currentMonth;
	private DateTime? _selectedDate;
	private string _monthYearText;
	private string _selectedDateText;
	private bool _isTasksSectionVisible;

	public ObservableCollection<Reminder> AllReminders { get; set; } = new();
	public ObservableCollection<TaskItem> DayTasks { get; set; } = new();
	public ObservableCollection<CalendarDay> CalendarDays { get; set; } = new();

	public DateTime CurrentMonth
	{
		get => _currentMonth;
		set
		{
			if(SetProperty(ref _currentMonth, value))
				UpdateCalendar();
		}
	}

	public DateTime? SelectedDate
	{
		get => _selectedDate;
		set
		{
			if(SetProperty(ref _selectedDate, value))
				LoadTasksForSelectedDate();
		}
	}

	public string MonthYearText
	{
		get => _monthYearText;
		set => SetProperty(ref _monthYearText, value);
	}

	public string SelectedDateText
	{
		get => _selectedDateText;
		set => SetProperty(ref _selectedDateText, value);
	}

	public bool IsTasksSectionVisible
	{
		get => _isTasksSectionVisible;
		set => SetProperty(ref _isTasksSectionVisible, value);
	}

	public ICommand NextMonthCommand { get; }
	public ICommand PrevMonthCommand { get; }
	public ICommand SelectDateCommand { get; }
	public ICommand AddTaskCommand { get; }

	public CalendarViewModel()
	{
		_currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

		NextMonthCommand = new Command(async () => await ChangeMonth(1));
		PrevMonthCommand = new Command(async () => await ChangeMonth(-1));
		SelectDateCommand = new Command<CalendarDay>(OnDaySelected);
		AddTaskCommand = new Command(async () => await AddTaskForSelectedDate());

		_ = InitializeAsync();
	}

	private async Task InitializeAsync()
	{
		await LoadReminders();
		UpdateCalendar();
	}

	private async Task LoadReminders()
	{
		var reminders = await App.Database.GetRemindersAsync() ?? new List<Reminder>();

		MainThread.BeginInvokeOnMainThread(() =>
		{
			AllReminders.Clear();
			foreach(var r in reminders)
				AllReminders.Add(r);

			UpdateCalendar();
		});
	}

	private void UpdateCalendar()
	{
		MonthYearText = _currentMonth.ToString("MMMM yyyy", new CultureInfo("ru-RU"));

		CalendarDays.Clear();

		DateTime firstOfMonth = new(_currentMonth.Year, _currentMonth.Month, 1);
		int firstDayOfWeek = ((int)firstOfMonth.DayOfWeek + 6) % 7; // Monday=0
		DateTime startDate = firstOfMonth.AddDays(-firstDayOfWeek);

		for(int i = 0; i < 42; i++)
		{
			DateTime date = startDate.AddDays(i);
			bool hasReminders = AllReminders.Any(r => r.ReminderDate.Date == date.Date);

			CalendarDays.Add(new CalendarDay
			{
				Date = date,
				IsCurrentMonth = date.Month == _currentMonth.Month,
				IsToday = date.Date == DateTime.Today,
				HasReminders = hasReminders,
				IsSelected = _selectedDate.HasValue && _selectedDate.Value.Date == date.Date
			});
		}
	}

	private async Task ChangeMonth(int delta)
	{
		CurrentMonth = _currentMonth.AddMonths(delta);
		SelectedDate = null;
		IsTasksSectionVisible = false;
		await LoadReminders();
	}

	private void OnDaySelected(CalendarDay day)
	{
		if(day == null) return;

		SelectedDate = day.Date;

		foreach(var d in CalendarDays)
			d.IsSelected = d.Date == day.Date;

		OnPropertyChanged(nameof(CalendarDays));
	}

	private void LoadTasksForSelectedDate()
	{
		if(!_selectedDate.HasValue)
		{
			IsTasksSectionVisible = false;
			return;
		}

		var dayReminders = AllReminders
			.Where(r => r.ReminderDate.Date == _selectedDate.Value.Date)
			.OrderBy(r => r.ReminderDate)
			.ToList();

		SelectedDateText = $"Задачи на {_selectedDate:dd MMMM yyyy}";

		DayTasks.Clear();
		foreach(var reminder in dayReminders)
		{
			DayTasks.Add(new TaskItem
			{
				Name = reminder.Name,
				Time = reminder.ReminderDate.ToString("HH:mm"),
				Color = GetTaskColor(reminder)
			});
		}

		IsTasksSectionVisible = true;
	}

	private Color GetTaskColor(Reminder reminder)
	{
		if(reminder.IsDone)
			return Color.FromArgb("#9E9E9E");

		return reminder.Urgency switch
		{
			Urgency.High => Color.FromArgb("#FF9800"),
			Urgency.Medium => Color.FromArgb("#2196F3"),
			Urgency.Low => Color.FromArgb("#4CAF50"),
			_ => Color.FromArgb("#2196F3")
		};
	}

	private async Task AddTaskForSelectedDate()
	{
		if(!_selectedDate.HasValue)
		{
			await Shell.Current.DisplayAlert(
				"Выберите дату",
				"Пожалуйста, выберите день для добавления задачи",
				"OK");
			return;
		}

		await Shell.Current.GoToAsync(
			$"{nameof(AddView)}?date={_selectedDate.Value:yyyy-MM-dd}", true);
	}
}
