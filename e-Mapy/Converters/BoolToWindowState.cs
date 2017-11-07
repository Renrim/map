using System;
using System.Globalization;
using System.Windows.Data;

namespace eMapy.Converters
{
    internal class BoolToWindowState : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool Value = (bool) value;
            if (Value == true)
            {
                return System.Windows.WindowState.Minimized;
            }
            else
            {
                return System.Windows.WindowState.Maximized;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.WindowState.Maximized;
        }
    }
}