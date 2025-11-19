using ReminderApp.Models;

namespace ReminderApp.ViewModels;

public abstract class BaseReminderViewModel : BaseViewModel
{
	private string _name;
	private string _description;
	private DateTime _reminderDate = DateTime.Now;
	private TimeSpan _reminderTime = DateTime.Now.TimeOfDay;
	private string _selectedUrgency = "Средний";

	public List<string> UrgencyOptions { get; } = new() { "Низкий", "Средний", "Высокий" };

	public string Name { get => _name; set => SetProperty(ref _name, value); }
	public string Description { get => _description; set => SetProperty(ref _description, value); }
	public DateTime ReminderDate { get => _reminderDate; set => SetProperty(ref _reminderDate, value); }
	public TimeSpan ReminderTime { get => _reminderTime; set => SetProperty(ref _reminderTime, value); }
	public string SelectedUrgency { get => _selectedUrgency; set => SetProperty(ref _selectedUrgency, value); }

	protected Urgency TextToUrgency(string urgency) => urgency switch
	{
		"Низкий" => Urgency.Low,
		"Средний" => Urgency.Medium,
		"Высокий" => Urgency.High,
		_ => Urgency.Medium
	};

	protected string UrgencyToText(Urgency urgency) => urgency switch
	{
		Urgency.Low => "Низкий",
		Urgency.Medium => "Средний",
		Urgency.High => "Высокий",
		_ => "Средний"
	};
}
