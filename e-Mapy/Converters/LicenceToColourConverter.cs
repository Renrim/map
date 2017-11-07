using System;
using System.Globalization;
using System.Windows.Data;
using eMapy.Models;

namespace eMapy.Converters
{
    public class LicenceToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var licence = (KindOfLicence) value;
            if (licence == KindOfLicence.Basic)
            {
                return "Gray";
            }
            if (licence == KindOfLicence.Demo)
            {
                return "#42A940";
            }
            if (licence == KindOfLicence.Full)
            {
                return "#42A940";
            }
            return "#42A940";


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}