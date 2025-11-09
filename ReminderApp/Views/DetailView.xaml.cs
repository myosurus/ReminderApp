using ReminderApp.Models;
using ReminderApp.ViewModels;

namespace ReminderApp.Views;

[QueryProperty(nameof(Reminder), "Reminder")]
public partial class DetailView : ContentPage
{
	private Reminder _reminder;
	public Reminder Reminder
	{
		get => _reminder;
		set
		{
			_reminder = value;
			BindingContext = new DetailViewModel(_reminder);
		}
	}

	public DetailView()
	{
		InitializeComponent();
		BindingContext = new DetailViewModel();
	}
}
