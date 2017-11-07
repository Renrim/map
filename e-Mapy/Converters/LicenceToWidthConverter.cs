using System;
using System.Globalization;
using System.Windows.Data;
using eMapy.Models;

namespace eMapy.Converters
{
    public class LicenceToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = (string) parameter;
            var licence = (KindOfLicence)value;
            if (param == "Ustawienia")
            {
                if (licence == KindOfLicence.Basic)
                {
                    //return 5;
                    return 237;
                }
                if (licence == KindOfLicence.Demo)
                {
                    return 237;
                }
                if (licence == KindOfLicence.Full)
                {
                    return 237;
                }
                return 237;
            }
            if (param == "DataGrid")
            {
                if (licence == KindOfLicence.Basic)
                {
                    return 5;
                }
                if (licence == KindOfLicence.Demo)
                {
                    return 250;
                }
                if (licence == KindOfLicence.Full)
                {
                    return 250;
                }
            }
        
            return 250;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}