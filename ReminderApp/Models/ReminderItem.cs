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

	public Color UrgencyColor => Reminder.Urgency switch
	{
		Urgency.High => Color.FromArgb("#FF9800"),
		Urgency.Medium => Color.FromArgb("#2196F3"),
		Urgency.Low => Color.FromArgb("#4CAF50"),
		_ => Color.FromArgb("#2196F3")
	};

	public Color OverdueColor => IsOverdue ? Color.FromArgb("#F44336") : Colors.Transparent;
}