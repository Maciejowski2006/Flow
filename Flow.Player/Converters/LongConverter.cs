using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Flow.Player.Converters;

public class LongConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		TimeSpan ts = TimeSpan.FromMilliseconds(value is long l ? l : 0);
		return $"{ts.Minutes:00}:{ts.Seconds:00}"; 
	}
	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}