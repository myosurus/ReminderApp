using ReminderApp.ViewModels;

namespace ReminderApp.Views;

[QueryProperty(nameof(ReminderId), "id")]
public partial class EditView : ContentPage
{
	private int _reminderId;
	public int ReminderId
	{
		get => _reminderId;
		set
		{
			_reminderId = value;
			LoadReminder(value);
		}
	}

	public EditView()
	{
		InitializeComponent();
	}

	private async void LoadReminder(int id)
	{
		var reminder = await App.Database.GetReminderAsync(id);

		if(reminder == null)
		{
			await DisplayAlert("Ошибка", "Задача не найдена", "OK");
			await Shell.Current.GoToAsync(".."); // go back
			return;
		}

		BindingContext = new EditViewModel(reminder);
	}
}
