using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using eMapy.Models;

namespace eMapy.Converters
{
    public class LicenceToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var licence = (KindOfLicence) value;
            if (licence == KindOfLicence.Basic)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
