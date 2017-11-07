using System;
using System.Globalization;
using System.Windows.Data;
using eMapy.Models;

namespace eMapy.Converters
{
    public class LicenceToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((KindOfLicence)value == KindOfLicence.Basic)
            {
                return false;
            }
            if ((KindOfLicence)value == KindOfLicence.Demo)
            {
                return true;
            }
            if ((KindOfLicence)value == KindOfLicence.Full)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}