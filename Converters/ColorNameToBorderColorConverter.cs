using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Converters;

public class ColorNameToBorderColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string colorName)
        {
            switch (colorName)
            {
                case "Red":
                    return Application.Current.FindResource("ResultBorderRedBrush");
                case "Black":
                    return Application.Current.FindResource("ResultBorderBlackBrush");
                case "Green":
                    return Application.Current.FindResource("ResultBorderGreenBrush");
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