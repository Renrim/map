using System;

using System.Globalization;

using System.Windows.Data;

namespace eMapy.Converters
{
    internal class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool Value = (bool) value;
            if (Value == false)
            {
                return 1;
            }
            else
            {
                return 0.85;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}