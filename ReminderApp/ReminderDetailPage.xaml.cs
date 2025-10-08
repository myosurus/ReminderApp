namespace ReminderApp;

public partial class ReminderDetailPage : ContentPage
{
	public ReminderDetailPage()
	{
		InitializeComponent();
		UrgencyPicker.ItemsSource = Enum.GetValues(typeof(Urgency)).Cast<Urgency>().ToList();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		if(BindingContext == null || ((Reminder)BindingContext).Id != 0 && Shell.Current.CurrentItem.Title == "Add")
		{
			BindingContext = new Reminder
			{
				ReminderDate = DateTime.Now,
				IsDone = false
			};
		}

		var reminder = (Reminder)BindingContext;

		DeleteButton.IsVisible = reminder.Id != 0;

		UrgencyPicker.SelectedItem = reminder.Urgency;
		ReminderDatePicker.Date = reminder.ReminderDate.Date;
		ReminderTimePicker.Time = reminder.ReminderDate.TimeOfDay;

		Title = reminder.Id == 0 ? "Add Reminder" : "Edit Reminder";
	}


	async void OnSaveClicked(object sender, EventArgs e)
	{
		if(BindingContext is not Reminder reminder) return;

		reminder.Urgency = (Urgency)(UrgencyPicker.SelectedItem ?? Urgency.Medium);

		var date = ReminderDatePicker.Date;
		var time = ReminderTimePicker.Time;
		reminder.ReminderDate = date + time;

		await App.Database.SaveReminderAsync(reminder);
		await Shell.Current.GoToAsync("//Reminders");
	}

	async void OnDeleteClicked(object sender, EventArgs e)
	{
		if(BindingContext is not Reminder reminder) return;

		bool confirm = await DisplayAlert("Delete", "Delete this reminder?", "Yes", "No");
		if(!confirm) return;

		await App.Database.DeleteReminderAsync(reminder);

		await Shell.Current.GoToAsync("//Reminders");
	}
}

