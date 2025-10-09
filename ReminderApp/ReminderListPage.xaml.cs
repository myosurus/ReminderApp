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
			if(reminder == null)
				return;

			await Navigation.PushAsync(new ReminderDetailPage
			{
				BindingContext = reminder
			});
		});

		BindingContext = this;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		RemindersList.ItemsSource = await App.Database.GetRemindersAsync();
	}
}

