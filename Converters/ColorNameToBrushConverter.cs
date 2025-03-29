using System;
using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Converters
{
    public class ColorNameToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string colorName)
            {
                switch (colorName)
                {
                    case "Red":
                        return Application.Current.FindResource("ResultRedBrush");
                    case "Black":
                        return Application.Current.FindResource("ResultBlackBrush");
                    case "Green":
                        return Application.Current.FindResource("ResultGreenBrush");
                    default:
                        return Brushes.Gray;
                }
            }
            
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}