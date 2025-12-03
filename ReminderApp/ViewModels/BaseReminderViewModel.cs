using ReminderApp.Models;

namespace ReminderApp.ViewModels;

public abstract class BaseReminderViewModel : BaseViewModel
{
	private string _name;
	private string _description;
	private DateTime _reminderDate = DateTime.Now;
	private TimeSpan _reminderTime = DateTime.Now.TimeOfDay;

	public string Name { get => _name; set => SetProperty(ref _name, value); }
	public string Description { get => _description; set => SetProperty(ref _description, value); }
	public DateTime ReminderDate { get => _reminderDate; set => SetProperty(ref _reminderDate, value); }
	public TimeSpan ReminderTime { get => _reminderTime; set => SetProperty(ref _reminderTime, value); }

	private int _frequencyValue = 30;
	public int FrequencyValue
	{
		get => _frequencyValue;
		set => SetProperty(ref _frequencyValue, value);
	}

	private string _frequencyUnit = "мин";
	public string FrequencyUnit
	{
		get => _frequencyUnit;
		set => SetProperty(ref _frequencyUnit, value);
	}

	public List<string> FrequencyUnits { get; } = new() { "мин", "ч", "дн" };

	public TimeSpan GetFrequencyTimeSpan()
	{
		return FrequencyUnit switch
		{
			"мин" => TimeSpan.FromMinutes(FrequencyValue),
			"ч" => TimeSpan.FromHours(FrequencyValue),
			"дн" => TimeSpan.FromDays(FrequencyValue),
			_ => TimeSpan.FromMinutes(30)
		};
	}
}

