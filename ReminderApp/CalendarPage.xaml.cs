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



    //private void UpdateCalendar()
    //{
    //    // Очистить старое содержимое
    //    CalendarGrid.Children.Clear();

    //    // Заголовок месяца
    //    MonthYearLabel.Text = _currentMonth.ToString("MMMM yyyy", new CultureInfo("ru-RU"));

    //    // Первый день месяца
    //    var firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);

    //    // День недели первого дня месяца (понедельник — первый)
    //    int offset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

    //    // Начальная дата для календаря
    //    var startDate = firstDayOfMonth.AddDays(-offset);

    //    // Заполняем сетку датами (6 строк × 7 столбцов)
    //    for (int row = 0; row < 6; row++)
    //    {
    //        for (int col = 0; col < 7; col++)
    //        {
    //            var date = startDate.AddDays(row * 7 + col);

    //            // Все напоминания на эту дату
    //            var dayReminders = _allReminders?
    //                .Where(r => r.ReminderDate.Date == date.Date)
    //                .ToList();

    //            // Кнопка дня
    //            var dayButton = new Button
    //            {
    //                Text = date.Day.ToString(),
    //                FontSize = 14,
    //                Padding = new Thickness(0),
    //                HorizontalOptions = LayoutOptions.Center,
    //                VerticalOptions = LayoutOptions.Center,
    //                BackgroundColor = Colors.Transparent,
    //                TextColor = GetDayTextColor(date, dayReminders),
    //                WidthRequest = 40,
    //                HeightRequest = 40,
    //                CornerRadius = 20
    //            };

    //            // Подсветка выбранного дня
    //            if (_selectedDate.HasValue && date.Date == _selectedDate.Value.Date)
    //            {
    //                dayButton.BorderColor = Colors.DeepSkyBlue;
    //                dayButton.BorderWidth = 2;
    //            }

    //            // Обработка клика
    //            dayButton.Clicked += (s, e) => OnDaySelected(date);

    //            // Контейнер для кнопки и индикаторов
    //            var dayContainer = new VerticalStackLayout
    //            {
    //                Spacing = 2,
    //                HorizontalOptions = LayoutOptions.Center,
    //                VerticalOptions = LayoutOptions.Center
    //            };

    //            dayContainer.Children.Add(dayButton);

    //            // Индикаторы задач под числом
    //            var taskIndicators = CreateTaskIndicators(dayReminders);
    //            if (taskIndicators != null)
    //                dayContainer.Children.Add(taskIndicators);

    //            // Добавляем день в сетку
    //            CalendarGrid.Add(dayContainer, col, row + 1);
    //        }
    //    }
    //}

    private void UpdateCalendar()
    {
        // Очистить старое содержимое
        CalendarGrid.Children.Clear();

        // Заголовок месяца
        MonthYearLabel.Text = _currentMonth.ToString("MMMM yyyy", new CultureInfo("ru-RU"));

        // Первый день месяца
        var firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);

        // День недели первого дня месяца (понедельник — первый)
        int offset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

        // Начальная дата для календаря (понедельник первой недели)
        var startDate = firstDayOfMonth.AddDays(-offset);

        // Заполняем сетку датами (6 строк × 7 столбцов)
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                var date = startDate.AddDays(row * 7 + col);

                // Все напоминания на эту дату
                var dayReminders = _allReminders?
                    .Where(r => r.ReminderDate.Date == date.Date)
                    .ToList();

                // Определяем цвет текста
                Color textColor;
                if (date.Month != _currentMonth.Month)
                {
                    // День не из текущего месяца → делаем светло-серым
                    textColor = Color.FromArgb("#CCCCCC");
                }
                else
                {
                    // Цвет для дней текущего месяца
                    textColor = GetDayTextColor(date, dayReminders);
                }

                // Кнопка дня
                var dayButton = new Button
                {
                    Text = date.Day.ToString(),
                    FontSize = 14,
                    Padding = new Thickness(0),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    BackgroundColor = Colors.Transparent,
                    TextColor = textColor,
                    WidthRequest = 40,
                    HeightRequest = 40,
                    CornerRadius = 20
                };

                // Подсветка выбранного дня
                if (_selectedDate.HasValue && date.Date == _selectedDate.Value.Date)
                {
                    dayButton.BorderColor = Colors.DeepSkyBlue;
                    dayButton.BorderWidth = 2;
                }

                // Обработка клика
                dayButton.Clicked += (s, e) => OnDaySelected(date);

                // Контейнер для кнопки и индикаторов
                var dayContainer = new VerticalStackLayout
                {
                    Spacing = 2,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                dayContainer.Children.Add(dayButton);

                // Индикаторы задач под числом (только для текущего месяца)
                if (date.Month == _currentMonth.Month)
                {
                    var taskIndicators = CreateTaskIndicators(dayReminders);
                    if (taskIndicators != null)
                        dayContainer.Children.Add(taskIndicators);
                }

                // Добавляем день в сетку
                CalendarGrid.Add(dayContainer, col, row + 1);
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