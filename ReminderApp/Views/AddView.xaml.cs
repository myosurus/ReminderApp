using ReminderApp.ViewModels;

namespace ReminderApp.Views;

[QueryProperty(nameof(PreselectedDate), "date")]
public partial class AddView : ContentPage
{
	public string PreselectedDate
	{
		set
		{
			if(BindingContext is AddViewModel vm &&
				DateTime.TryParse(value, out var date))
			{
				vm.ReminderDate = date;
			}
		}
	}

	public AddView()
	{
		InitializeComponent();
		BindingContext = new AddViewModel();
	}
}
