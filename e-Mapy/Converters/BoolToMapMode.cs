using Microsoft.Maps.MapControl.WPF;
using System;
using System.Globalization;
using System.Windows.Data;
using BingMapsRESTToolkit;

namespace eMapy.Converters
{
    public class BoolToMapMode : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var MapMode = (bool)value;

            if (!MapMode)
            {
                var aerialMode = new AerialMode();
                aerialMode.Labels = true;

                return aerialMode;
            }
            var roadMode = new RoadMode();
            return roadMode;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}