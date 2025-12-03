namespace ReminderApp.Models;

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

    public Urgency Urgency
    {
        get
        {
            var daysLeft = (ReminderDate.Date - DateTime.Today).TotalDays;

            if (IsDone) return Urgency.Low; // выполненные — не срочные
            if (daysLeft < 0) return Urgency.High;     // просрочено
            if (daysLeft == 0) return Urgency.High;     // просрочено
            if (daysLeft <= 1) return Urgency.High;    // сегодня или завтра
            if (daysLeft <= 3) return Urgency.Medium;  // до 3 дней
            return Urgency.Low;                        // больше 3 дней
        }
    }

    public Color UrgencyColor //динамические индикаторы сроков выполнения
    {
        get
        {
            if (IsDone) return Color.FromArgb("#BDBDBD"); //серый

            var daysLeft = (ReminderDate.Date - DateTime.Today).TotalDays;

            if (daysLeft < 0) return Color.FromArgb("#FF0000"); // просрочено (красный)
            if (daysLeft == 0) return Color.FromArgb("#F44336"); // просрочено (красный)
            if (daysLeft <= 1) return Color.FromArgb("#ffaa22"); // сегодня/завтра (оранжевый)
            if (daysLeft <= 3) return Color.FromArgb("#ffed22"); // 2–3 дня (желтый)
            return Color.FromArgb("#4CAF50"); // всё спокойно (зеленый)
        }
    }

    // НОВЫЕ СВОЙСТВА — вставь сюда:
    public string DueStatusText => GetDueStatusText();
    public Color DueStatusColor => GetDueStatusColor();

    private string GetDueStatusText()
    {
        if (IsDone) return "Выполнено";

        var dueDateTime = ReminderDate.Date + Reminder.ReminderTime;
        var now = DateTime.Now;
        var diff = dueDateTime - now;

        if (diff <= TimeSpan.Zero)
            return "Просрочено";

        if (diff.TotalMinutes < 1)
            return "Менее минуты";

        if (diff.TotalMinutes < 60)
            return $"Осталось {(int)diff.TotalMinutes} мин";

        if (diff.TotalHours < 24)
            return $"Осталось {(int)diff.TotalHours} ч {diff.Minutes:00} мин";

        if (diff.TotalDays < 7)
            return $"Осталось {(int)diff.TotalDays} дн.";

        return "Время есть";
    }

    private Color GetDueStatusColor()
    {
        if (IsDone) return Color.FromArgb("#BDBDBD");

        var dueDateTime = ReminderDate.Date + Reminder.ReminderTime;
        var diff = dueDateTime - DateTime.Now;

        if (diff <= TimeSpan.Zero)
            return Colors.Red;

        if (diff.TotalHours <= 1)
            return Color.FromArgb("#FF5722");

        if (diff.TotalHours <= 6)
            return Color.FromArgb("#FF9800");

        if (diff.TotalHours <= 24)
            return Color.FromArgb("#FFC107");

        return Color.FromArgb("#4CAF50");
    }
    // ←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←←


}