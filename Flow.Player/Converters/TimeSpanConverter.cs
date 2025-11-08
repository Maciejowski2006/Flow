using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Flow.Player.Converters;

public class TimeSpanConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not TimeSpan ts)
			return null;

		return ts.Hours > 0 ? $"{ts.Hours}:{ts.Minutes:00}:{ts.Seconds:00}" : $"{ts.Minutes:00}:{ts.Seconds:00}";
	}
	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) { throw new NotSupportedException(); }
}