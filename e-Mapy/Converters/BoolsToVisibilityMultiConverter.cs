using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace eMapy.Converters
{
    public class BoolsToVisibilityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var adresGridVisibility = (bool) values[0];
            var changingCoordsPermitted = (bool) values[1];

            if (!changingCoordsPermitted)
            {
                return Visibility.Collapsed;
            }
            if (!adresGridVisibility)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}