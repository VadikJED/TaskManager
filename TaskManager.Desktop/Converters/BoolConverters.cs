using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace TaskManager.Desktop.Converters;

public class BoolToTextDecorationsConverter : IValueConverter
{
  public static readonly BoolToTextDecorationsConverter Instance = new();

  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    return value is bool boolValue && boolValue ? TextDecorations.Strikethrough : null;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}

public class BoolToOpacityConverter : IValueConverter
{
  public static readonly BoolToOpacityConverter Instance = new();

  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    return value is bool boolValue && boolValue ? 0.6 : 1.0;
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}