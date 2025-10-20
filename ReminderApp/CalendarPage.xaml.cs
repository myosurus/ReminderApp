using System.Globalization;

namespace ReminderApp;

public partial class CalendarPage : ContentPage
{
    private DateTime _currentMonth;
    private DateTime? _selectedDate;
    private List<Reminder> _allReminders;

    public CalendarPage()
    {
        InitializeComponent();
        _currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        LoadReminders();
        UpdateCalendar();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadReminders();
        UpdateCalendar();
    }

    private async Task LoadReminders()
    {
        _allReminders = await App.Database.GetRemindersAsync() ?? new List<Reminder>();
    }


    private void UpdateCalendar()
    {
        // Обновляем заголовок
        MonthYearLabel.Text = _currentMonth.ToString("MMMM yyyy", new CultureInfo("ru-RU"));

        // Очищаем календарную сетку
        CalendarGrid.Children.Clear();
        CalendarGrid.RowDefinitions.Clear();

        // Добавляем 6 строк для календаря
        for (int i = 0; i < 6; i++)
        {
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
        }

        // Получаем первый день месяца и количество дней
        var firstDay = _currentMonth;
        var daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
        var startDay = (int)firstDay.DayOfWeek;

        // Заполняем календарь
        int row = 0;
        int col = startDay;

        for (int day = 1; day <= daysInMonth; day++)
        {
            var currentDate = new DateTime(_currentMonth.Year, _currentMonth.Month, day);
            var dayReminders = _allReminders?.Where(r => r.ReminderDate.Date == currentDate.Date).ToList();

            // Создаем контейнер для дня
            var dayContainer = new Grid
            {
                HeightRequest = 45,
                WidthRequest = 45
            };

            // Кнопка дня
            var dayButton = new Button
            {
                Text = day.ToString(),
                FontSize = 14,
                BackgroundColor = Colors.Transparent,
                TextColor = GetDayTextColor(currentDate, dayReminders),
                BorderWidth = 0,
                CornerRadius = 22,
                HorizontalOptions = LayoutOptions.Center, // ИСПРАВЛЕНО
                VerticalOptions = LayoutOptions.Center,   // ИСПРАВЛЕНО
                HeightRequest = 40,
                WidthRequest = 40
            };

            dayButton.Clicked += (s, e) => OnDaySelected(currentDate);

            // Выделяем выбранный день
            if (_selectedDate?.Date == currentDate.Date)
            {
                dayButton.BackgroundColor = Color.FromArgb("#E3F2FD");
                dayButton.TextColor = Color.FromArgb("#1976D2");
                dayButton.FontAttributes = FontAttributes.Bold;
            }

            // Выделяем сегодняшний день
            if (currentDate.Date == DateTime.Today)
            {
                dayButton.BorderColor = Color.FromArgb("#1976D2");
                dayButton.BorderWidth = 2;
            }

            dayContainer.Children.Add(dayButton);

            // Добавляем индикаторы задач
            var indicators = CreateTaskIndicators(dayReminders);
            if (indicators != null)
            {
                var mainStack = new VerticalStackLayout { Spacing = 2 };
                mainStack.Children.Add(dayContainer);
                mainStack.Children.Add(indicators);

                CalendarGrid.Add(mainStack, col, row);
            }
            else
            {
                CalendarGrid.Add(dayContainer, col, row);
            }

            // Переход на следующую строку
            col++;
            if (col > 6)
            {
                col = 0;
                row++;
            }
        }
    }

    private Color GetDayTextColor(DateTime date, List<Reminder> reminders)
    {
        var today = DateTime.Today;

        if (date.Date == today.Date)
            return Color.FromArgb("#1976D2"); // Сегодня - синий
        else if (date.Date == today.AddDays(1).Date)
            return Color.FromArgb("#4CAF50"); // Завтра - зеленый
        else if (date < today && reminders?.Any(r => !r.IsDone) == true)
            return Color.FromArgb("#F44336"); // Просроченные - красный
        else if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            return Color.FromArgb("#666666"); // Выходные - серый
        else
            return Color.FromArgb("#333333"); // Обычные дни - черный
    }


    private HorizontalStackLayout CreateTaskIndicators(List<Reminder> reminders)
    {
        if (reminders == null || !reminders.Any())
            return null;

        var indicators = new HorizontalStackLayout
        {
            Spacing = 2,
            HorizontalOptions = LayoutOptions.Center,
            HeightRequest = 4
        };

        // Ограничиваем количество индикаторов
        var tasksToShow = reminders.Where(r => !r.IsDone).Take(3);

        foreach (var task in tasksToShow)
        {
            var color = task.Urgency switch
            {
                Urgency.High => Color.FromArgb("#FF9800"),
                Urgency.Medium => Color.FromArgb("#2196F3"),
                Urgency.Low => Color.FromArgb("#4CAF50"),
                _ => Color.FromArgb("#2196F3")
            };

            indicators.Children.Add(new Frame
            {
                BackgroundColor = color,
                HeightRequest = 4,
                WidthRequest = 4,
                Padding = 0,
                HasShadow = false,
                CornerRadius = 2
            });
        }

        return indicators;
    }

    private async void OnDaySelected(DateTime selectedDate)
    {
        _selectedDate = selectedDate;

        // Обновляем календарь для выделения выбранного дня
        UpdateCalendar();

        // Показываем задачи на выбранный день
        await ShowTasksForSelectedDate(selectedDate);
    }

    private async Task ShowTasksForSelectedDate(DateTime selectedDate)
    {
        var dayReminders = _allReminders?
            .Where(r => r.ReminderDate.Date == selectedDate.Date)
            .OrderBy(r => r.ReminderDate)
            .ToList();

        SelectedDateLabel.Text = $"Задачи на {selectedDate:dd MMMM yyyy}";

        if (dayReminders?.Any() == true)
        {
            var taskItems = dayReminders.Select(r => new TaskItem
            {
                Name = r.Name,
                Time = r.ReminderDate.ToString("HH:mm"),
                Color = GetTaskColor(r)
            }).ToList();

            TasksCollectionView.ItemsSource = taskItems;
            TasksSection.IsVisible = true;
        }
        else
        {
            TasksCollectionView.ItemsSource = null;
            TasksSection.IsVisible = true;
        }
    }

    private Color GetTaskColor(Reminder reminder)
    {
        if (reminder.IsDone)
            return Color.FromArgb("#9E9E9E"); // Серый для выполненных

        return reminder.Urgency switch
        {
            Urgency.High => Color.FromArgb("#FF9800"),   // Оранжевый для срочных
            Urgency.Medium => Color.FromArgb("#2196F3"), // Синий для средних
            Urgency.Low => Color.FromArgb("#4CAF50"),    // Зеленый для низких
            _ => Color.FromArgb("#2196F3")
        };
    }

    private async void OnPreviousMonthClicked(object sender, EventArgs e)
    {
        _currentMonth = _currentMonth.AddMonths(-1);
        _selectedDate = null;
        TasksSection.IsVisible = false;
        await LoadReminders();
        UpdateCalendar();
    }

    private async void OnNextMonthClicked(object sender, EventArgs e)
    {
        _currentMonth = _currentMonth.AddMonths(1);
        _selectedDate = null;
        TasksSection.IsVisible = false;
        await LoadReminders();
        UpdateCalendar();
    }

    private async void OnAddTaskForSelectedDateClicked(object sender, EventArgs e)
    {
        if (_selectedDate.HasValue)
        {
            var newReminder = new Reminder
            {
                ReminderDate = _selectedDate.Value
            };

            await Navigation.PushAsync(new ReminderDetailPage
            {
                BindingContext = newReminder
            });
        }
        else
        {
            await DisplayAlert("Выберите дату", "Пожалуйста, выберите день для добавления задачи", "OK");
        }
    }
}

// Вспомогательный класс для отображения задач
public class TaskItem
{
    public string Name { get; set; }
    public string Time { get; set; }
    public Color Color { get; set; }
}