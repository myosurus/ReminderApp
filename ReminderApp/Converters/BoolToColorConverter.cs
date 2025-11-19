using System.Globalization;

namespace ReminderApp.Converters;

public class BoolToColorConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		bool isSelected = (bool)value;
		return isSelected ? Color.FromArgb("#E3F2FD") : Colors.White;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		=> throw new NotImplementedException();
}
