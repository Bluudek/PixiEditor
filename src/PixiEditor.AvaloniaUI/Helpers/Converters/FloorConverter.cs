﻿using System.Globalization;

namespace PixiEditor.AvaloniaUI.Helpers.Converters;

internal class FloorConverter : SingleInstanceConverter<FloorConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Math.Floor((double)value);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
