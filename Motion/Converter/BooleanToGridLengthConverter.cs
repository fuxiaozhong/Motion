using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Motion.Converter
{
    public class BooleanToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isOpen && parameter is string widthString && double.TryParse(widthString, out double width))
            {
                return isOpen ? new GridLength(width) : new GridLength(50);
            }
            return new GridLength(50);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}