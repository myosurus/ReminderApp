using ReminderApp.Models;
using ReminderApp.ViewModels;

namespace ReminderApp.Views;

public partial class DetailView : ContentPage
{
	public DetailView(Reminder reminder = null)
	{
		InitializeComponent();

		BindingContext = reminder == null
			? new AddViewModel()
			: new EditViewModel(reminder);
	}
}
