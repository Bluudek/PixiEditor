﻿using System.Globalization;
using PixiEditor.AvaloniaUI.Helpers.Converters;

namespace PixiEditor.Helpers.Converters;

internal class IndexToAssociatedKeyConverter : SingleInstanceConverter<IndexToAssociatedKeyConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int index and < 10)
        {
            if (index == 9) return 0;
            return (int?)index + 1;
        }

        return (int?)null;
    }
}
