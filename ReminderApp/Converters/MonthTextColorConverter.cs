using System.Globalization;

namespace ReminderApp.Converters;

public class MonthTextColorConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		bool isCurrentMonth = (bool)value;
		return isCurrentMonth ? Colors.Black : Color.FromArgb("#CCCCCC");
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		=> throw new NotImplementedException();
}
