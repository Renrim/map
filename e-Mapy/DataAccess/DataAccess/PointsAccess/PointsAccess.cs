//using BingMapsRESTToolkit;
//using DataLayer;
//using eMapy.Models;
//using Microsoft.SqlServer.Types;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Data.Entity.Spatial;
//using System.Globalization;
//using System.Linq;
//using DistanceAndDuration = DataLayer.DistanceAndDuration;
//using Location = Microsoft.Maps.MapControl.WPF.Location;

//namespace eMapy.DataAccess.DataAccess.PointsAccess
//{
//    public class PointsAccess : BaseAccess.BaseAccess
//    {
//        private eMapyEntities2 dc = OpenConnection();

//        public ObservableCollection<EmapyPoint> GetAllPoints()
//        {
//            try
//            {
//                List<eMapyPoint> allPoints = dc.eMapyPoints.ToList();
//                ObservableCollection<EmapyPoint> result = new ObservableCollection<EmapyPoint>();
//                foreach (var item in allPoints)
//                {
//                    var newItem = new EmapyPoint
//                    {
//                        Name = item.NazwaKlienta,
//                        Marker = item.Marker,
//                        Catalog = item.Katalog,
//                        Description = item.Opis,
//                        ShortName = item.SkroconaNazwa,

//                        Adress =
//                            {
//                                SymfoniaAdress = item.SymfoniaAdress,
//                                TypeOfAdress = item.RodzajAdresu,
//                                City = item.Miasto,
//                                HouseNumber = item.NumerDomu,
//                                FlatNumber = item.NumerMieszkania,
//                                Kraj = item.Kraj,
//                                Wojewodztwo = item.Wojewodztwo,
//                                PostPlace = item.PostPlace,
//                            }
//                    };
//                    newItem.Location = new Location();
//                    newItem.Location = new Location(Convert.ToDouble(item.Szerokosc), Convert.ToDouble(item.Dlugosc));
//                    result.Add(newItem);
//                }

//                return result;
//            }
//            catch (Exception e)
//            {
//                e.ToString();
//                return null;
//            }
//        }

//        public void AddDistanceAndDuration(int distance, int duration, SimpleWaypoint startWaypoint, SimpleWaypoint stopWaypoint)
//        {
//            using (dc = OpenConnection())
//            {
//                DistanceAndDuration distanceAndDuration = new DistanceAndDuration()
//                {
//                    CreateDate = DateTime.Now,
//                    Distance = distance,
//                    Duration = duration,
//                    StartPoint = ConvertLatLonToDbGeography(startWaypoint.Coordinate.Latitude, startWaypoint.Coordinate.Longitude),
//                    StopPoint = ConvertLatLonToDbGeography(stopWaypoint.Coordinate.Latitude, stopWaypoint.Coordinate.Longitude),
//                    StopPointLat = Convert.ToDecimal(stopWaypoint.Coordinate.Latitude),
//                    StopPointLong = Convert.ToDecimal(stopWaypoint.Coordinate.Longitude),
//                    StartPointLong = Convert.ToDecimal(startWaypoint.Coordinate.Longitude),
//                    StartPointLat = Convert.ToDecimal(startWaypoint.Coordinate.Latitude)
//                };

//                dc.DistanceAndDurations.Add(distanceAndDuration);

//                dc.SaveChanges();
//            }
//        }

//        public DistanceAndDuration FindDurationAndDistance(Coordinate startPoint, Coordinate stopPoint)
//        {
//            using (dc = OpenConnection())
//            {
//                var startPointLat = Convert.ToDecimal(startPoint.Latitude);
//                var startPointLong = Convert.ToDecimal(startPoint.Longitude);
//                var stopPointLat =  Convert.ToDecimal(stopPoint.Latitude);
//                var stopPointLong = Convert.ToDecimal(stopPoint.Longitude);

//                var newItem = (from databaseItem in dc.DistanceAndDurations.Where(x =>
//                            (x.StartPointLat == startPointLat)
//                             &&  (x.StartPointLong == startPointLong) && x.StopPointLat == stopPointLat && x.StopPointLong == stopPointLong)
//                               select databaseItem).FirstOrDefault();
//                return newItem;
//            }
//        }

//        public SqlGeography ConvertDbToSqlGeography(DbGeography theGeography)
//        {
//            DbGeography heGeography = theGeography;
//            SqlGeography newGeography = SqlGeography.Parse(heGeography.AsText()).MakeValid();

//            return newGeography;
//        }

//        public static DbGeography ConvertLatLonToDbGeography(double longitude, double latitude)
//        {
//            try
//            {
//                var point = string.Format($"POINT({latitude.ToString(CultureInfo.InvariantCulture)} {longitude.ToString(CultureInfo.InvariantCulture)})");
//                return DbGeography.FromText(point);
//            }
//            catch (Exception s)
//            {
//                s.ToString();
//                throw;
//            }
//        }
//    }
//}