using System;
using BingMapsRESTToolkit;
using Microsoft.Maps.MapControl.WPF;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using eMapy.Models;
using eMapy.ViewModels;
using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace eMapy.Core
{
    public class MapInteractivity : DependencyObject
    {
        #region RouteResult

        // Using a DependencyProperty as the backing store for Cadre.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CadreProperty =
            DependencyProperty.RegisterAttached("Cadre", typeof(bool), typeof(MapInteractivity), new PropertyMetadata(false, OnCadreChanged));

        public static bool GetCadre(DependencyObject target)
        {
            return (bool)target.GetValue(CadreProperty);
        }

        public static void SetCadre(DependencyObject target, bool value)
        {
            target.SetValue(CadreProperty, value);
        }

        private static void OnCadreChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OnCadreChanged((Map)o, (bool)e.OldValue, (bool)e.NewValue);
        }

        public static void OnCadreChanged(Map map, bool oldValue, bool newValue)
        {
            ;
            var dataContext = map.DataContext as MainWindowViewModel;
            var punktynamapie = new List<Location>();
            if (dataContext.SelectedPoints.Count > 0 && dataContext.Options.ProgressBarAnimation == true)
            {
                CadreToAll(dataContext, punktynamapie, map);
                return;
            }
            CadreToAll(dataContext, punktynamapie, map);
        }

        private static void CadreToAll(MainWindowViewModel dataContext, List<Location> punktynamapie, Map map1)
        {
            foreach (var item in dataContext.PointsOnMap)
            {
                punktynamapie.Add(item?.Location);
            }
            foreach (var item in dataContext?.StartAndStopPoints)
            {
                punktynamapie.Add(item.Location);
            }
            try
            {
                map1.SetView(punktynamapie, new Thickness(260, 50, 20, 50), 0);
            }
            catch
            {
                // ignored
            }
        }

        public static readonly DependencyProperty RouteResultProperty = DependencyProperty.RegisterAttached("RouteResult", typeof(List<Route>), typeof(MapInteractivity), new UIPropertyMetadata(null, OnRouteResultChanged));

        public static List<Route> GetRouteResult(DependencyObject target)
        {
            return (List<Route>)target.GetValue(RouteResultProperty);
        }

        public static void SetRouteResult(DependencyObject target, List<Route> value)
        {
            target.SetValue(RouteResultProperty, value);
        }

        private static void OnRouteResultChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OnRouteResultChanged((Map)o, (List<Route>)e.OldValue, (List<Route>)e.NewValue);
        }

        public static void OnRouteResultChanged(Map map, List<Route> oldValue, List<Route> newValue)
        {
            var routeLineLayer = GetRouteLineLayer(map);
            if (routeLineLayer == null)
            {
                routeLineLayer = new MapLayer();
                SetRouteLineLayer(map, routeLineLayer);
            }

            routeLineLayer.Children.Clear();

            foreach (Route item in newValue)
            {
                MapPolyline routeLine = new MapPolyline
                {
                    Locations = new LocationCollection(),
                    Opacity = 0.65,
                    Stroke = new SolidColorBrush(Colors.Blue),
                    StrokeThickness = 3.0
                };

                int bound = item.RoutePath.Line.Coordinates.GetUpperBound(0);

                for (int i = 0; i < bound; i++)
                {
                    routeLine.Locations.Add(new Location
                    {
                        Latitude = item.RoutePath.Line.Coordinates[i][0],
                        Longitude = item.RoutePath.Line.Coordinates[i][1]
                    });
                }
                routeLineLayer.Children.Add(routeLine);
            }

            //Set the map view
            //  LocationRect rect = new LocationRect(routeLine.Locations[0], routeLine.Locations[routeLine.Locations.Count - 1]);
            //map.SetView(rect);
        }

        #endregion RouteResult

        #region RouteLineLayer

        public static readonly DependencyProperty RouteLineLayerProperty = DependencyProperty.RegisterAttached("RouteLineLayer", typeof(MapLayer), typeof(MapInteractivity), new UIPropertyMetadata(null, OnRouteLineLayerChanged));

        public static MapLayer GetRouteLineLayer(DependencyObject target)
        {
            return (MapLayer)target.GetValue(RouteLineLayerProperty);
        }

        public static void SetRouteLineLayer(DependencyObject target, MapLayer value)
        {
            target.SetValue(RouteLineLayerProperty, value);
        }

        private static void OnRouteLineLayerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OnRouteLineLayerChanged((Map)o, (MapLayer)e.OldValue, (MapLayer)e.NewValue);
        }

        private static void OnRouteLineLayerChanged(Map map, MapLayer oldValue, MapLayer newValue)
        {
            if (!map.Children.Contains(newValue))
                map.Children.Add(newValue);
        }

        #endregion RouteLineLayer
    }
}