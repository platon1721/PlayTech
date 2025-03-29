using System;
using Avalonia.Data.Converters;
using Avalonia.Data;

namespace Converters
{
    public class MultiplierVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int multiplier)
            {
                return multiplier > 0;
            }
            
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}