using System.Windows.Input;

namespace ReminderApp;

public partial class ReminderListPage : ContentPage
{
    public ICommand ItemTappedCommand { get; }

    public ReminderListPage()
    {
        InitializeComponent();

        ItemTappedCommand = new Command<Reminder>(async (reminder) =>
        {
            if (reminder == null)
                return;

            await Navigation.PushAsync(new ReminderDetailPage
            {
                BindingContext = reminder
            });
        });

        BindingContext = this;
    }

    protected override async void OnAppearing() //загрузка бд
    {
        base.OnAppearing();
        await LoadReminders();
    }

    private async Task LoadReminders()
    {
        var reminders = await App.Database.GetRemindersAsync();

        if (reminders != null && reminders.Any())
        {
            // 🔹 Показываем только невыполненные задачи
            var activeReminders = reminders.Where(r => !r.IsDone).ToList();

            var reminderItems = activeReminders.Select(r => new ReminderItem(r)).ToList();
            RemindersCollectionView.ItemsSource = reminderItems;
        }
        else
        {
            RemindersCollectionView.ItemsSource = null;
        }
    }

    private async void OnReminderSelected(object sender, SelectionChangedEventArgs e) 
    {
        if (e.CurrentSelection.FirstOrDefault() is ReminderItem selectedReminder)
        {
            // Переходим к редактированию задачи
            await Navigation.PushAsync(new ReminderDetailPage
            {
                BindingContext = selectedReminder.Reminder
            });

            // Снимаем выделение
            RemindersCollectionView.SelectedItem = null;
        }
    }

    private async void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e) 
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is ReminderItem reminderItem)
        {
            // Обновляем статус задачи в базе данных
            reminderItem.Reminder.IsDone = e.Value;
            await App.Database.SaveReminderAsync(reminderItem.Reminder);

            // Перезагружаем список для обновления цветов
            await LoadReminders();
        }
    }
}

// Класс-обертка для отображения задач с дополнительными свойствами
public class ReminderItem
{
    public Reminder Reminder { get; set; }

    public ReminderItem(Reminder reminder)
    {
        Reminder = reminder;
    }

    public string Name => Reminder.Name;
    public string Description => Reminder.Description;
    public DateTime ReminderDate => Reminder.ReminderDate;
    public bool IsDone => Reminder.IsDone;

    public bool IsOverdue => !Reminder.IsDone && Reminder.ReminderDate < DateTime.Now;

    public Color UrgencyColor => Reminder.Urgency switch
    {
        Urgency.High => Color.FromArgb("#FF9800"),
        Urgency.Medium => Color.FromArgb("#2196F3"),
        Urgency.Low => Color.FromArgb("#4CAF50"),
        _ => Color.FromArgb("#2196F3")
    };

    public Color OverdueColor => IsOverdue ? Color.FromArgb("#F44336") : Colors.Transparent;
}

