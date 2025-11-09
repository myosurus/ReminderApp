namespace ReminderApp.Views;

public partial class CalendarView : ContentPage
{
	public CalendarView()
	{
		InitializeComponent();
		BindingContext = new ViewModels.CalendarViewModel();
	}
}