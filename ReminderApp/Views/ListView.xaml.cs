using ReminderApp.ViewModels;

namespace ReminderApp.Views;

public partial class ListView : ContentPage
{
	private ListViewModel ViewModel => BindingContext as ListViewModel;

	public ListView()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		ViewModel.LoadRemindersCommand.Execute(null);
	}
}
