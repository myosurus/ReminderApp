using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReminderApp.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
	bool isBusy;
	public bool IsBusy
	{
		get => isBusy;
		set => SetProperty(ref isBusy, value);
	}

	protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
	{
		if(EqualityComparer<T>.Default.Equals(storage, value))
			return false;

		storage = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	public event PropertyChangedEventHandler PropertyChanged;
	protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
