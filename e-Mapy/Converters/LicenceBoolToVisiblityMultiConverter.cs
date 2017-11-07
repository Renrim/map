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
    public class LicenceBoolToVisiblityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var licence = (KindOfLicence)values[0];
            var showHiddenPoints = (bool)values[1];
            if (licence == KindOfLicence.Basic)
            {
                return Visibility.Collapsed;
            }
            if (showHiddenPoints)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
