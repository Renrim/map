using System;
using System.Globalization;
using System.Windows.Data;
using eMapy.Models;

namespace eMapy.Converters
{
    internal class KindToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var kind = (Kind) value; // Dostaje START

            if (kind == Kind.Start )
            {
                return "Start";
            }
            if (kind == Kind.Stop)
            {
                return "Stop";
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}