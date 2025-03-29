using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Converters;

public class ColorNameToShadowColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string colorName)
        {
            switch (colorName)
            {
                case "Red":
                    return Application.Current.FindResource("ResultBorderShadowRedColor");
                case "Black":
                    return Application.Current.FindResource("ResultBorderShadowBlackColor");
                case "Green":
                    return Application.Current.FindResource("ResultBorderShadowGreenColor");
                default:
                    return Brushes.Yellow;
            }
        }
            
        return Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}