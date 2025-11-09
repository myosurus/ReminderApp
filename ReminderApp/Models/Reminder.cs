using SQLite;

namespace ReminderApp.Models; 
 
public class Reminder 
{ 
	[PrimaryKey, AutoIncrement] 
	public int Id { get; set; } 
	public string Name { get; set; } 
	public string Description { get; set; } 
	public Urgency Urgency { get; set; } 
	public DateTime ReminderDate { get; set; } 
	public DateTime StartReminding { get; set; } 
	public TimeSpan RemindFrequency { get; set; } 
	public bool IsDone { get; set; } 
} 
