using eMapy.DataAccess.DataAccess.ProtoBuf;
using eMapy.Models;
using eMapy.Utils;
using eMapy.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using eMapy.Managers;
using Location = Microsoft.Maps.MapControl.WPF.Location;
using MessageBox = System.Windows.MessageBox;

namespace eMapy.BusinessLogic.Services
{
    public class PointsServices : PointBase
    {
        public static string StartStopAdress;
        public static Int32 UserId;
        public static string UserCity;
        public static string UserHouseNumber;
        public static string UserStreetAdres;
        public static string UserPostalCode;
        public static string UserLogin;
        public static ObservableCollection<StartStopData> StartStopDataList = ProtoMethods.Deserialize<ObservableCollection<StartStopData>>(ProtoMethods.StartStopDataListPath);

        public static void UpdateLoadedPoint(LoadedPoint loadedPoint, EmapyPoint epoint)
        {
            loadedPoint.LocationLati = epoint.Location.Latitude;
            loadedPoint.LocationLong = epoint.Location.Longitude;
            loadedPoint.MapyAdress = epoint.MapyAdress;
            loadedPoint.MapyAdress = epoint.SymfoniaAdress;
        }

        public static LoadedPoint FindLoadedDataDataByAdress(string adress, string khCode, List<LoadedPoint> bufforedList)
        {
            try
            {
                var LoadedPoint = (from loadedPoint in bufforedList.Where(x => x.SymfoniaAdress == adress && x.KodKontrahenta == khCode) select loadedPoint)
                    .FirstOrDefault();
                return LoadedPoint;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool CheckIfContractorIsAdded(string symfoniaKod, string adress, ObservableCollection<EmapyPoint> finalEmapyPointsList)
        {
            var x = (from item in finalEmapyPointsList.Where(point =>
                    point.SymfoniaKod == symfoniaKod && point.SymfoniaAdress == adress)
                     select item).FirstOrDefault();
            if (x == null)
            {
                return false;
            }
            return true;
        }

        public static async Task<ObservableCollection<EmapyPoint>> GetPointsAsync()
        {
            try
            {
                List<LoadedPoint> bufforedList = ProtoMethods.Deserialize<List<LoadedPoint>>(ProtoMethods.LoadedPointsPath);
                DataSetMapy.SchowekKHDataTable receivedDataFromSymfonia = AddKH.ReadFileXml(GlobalMethods.GetPathXmlFile());

                ObservableCollection<EmapyPoint> finalEmapyPointsList = new ObservableCollection<EmapyPoint>();
                if (receivedDataFromSymfonia.Count == 0)
                {
                    return new ObservableCollection<EmapyPoint>();
                }
                int index = 1;
                foreach (var symfoniaItem in receivedDataFromSymfonia)
                {
                    if (symfoniaItem.Adress == null || symfoniaItem.Adress == " ,  , " || symfoniaItem.Adress == " ,  , Polska")
                    {
                        continue;
                    }
                    var x = CheckIfContractorIsAdded(symfoniaItem.Kod, symfoniaItem.Adress, finalEmapyPointsList);
                    if (x)
                    {
                        continue;
                    }

                    var point = SymfoniaPointToMapyPoint(symfoniaItem, index);
                    index++;
                    LoadedPoint foundedPoint = FindLoadedDataDataByAdress(point.SymfoniaAdress, point.SymfoniaKod, bufforedList);
                    if (foundedPoint == null)
                    {
                        LoadedPoint newLoadedPoint = new LoadedPoint();
                        var location = await GlobalMethods.GetLocationAsync(symfoniaItem.Adress).ConfigureAwait(false);
                        if (location == null)
                        {
                            continue;
                        }
                        if (location.Point != null)
                        {
                            MainManager.PointLocationComparator(point, location);
                            CreateBufforPoint(newLoadedPoint, point, location);
                            bufforedList.Add(newLoadedPoint);
                            finalEmapyPointsList.Add(point);
                        }
                    }
                    else
                    {
                        LoadedPointToMapyPointComparator(point, foundedPoint);
                        finalEmapyPointsList.Add(point);
                    }
                }

                ProtoMethods.Serialize(bufforedList, ProtoMethods.LoadedPointsPath);

                return finalEmapyPointsList;
            }
            catch (Exception e)
            {
                MessageBox.Show("Problem z wczytaniem punktów");
                return null;
            }
        }

        private static EmapyPoint SymfoniaPointToMapyPoint(DataSetMapy.SchowekKHRow symfoniaItem, int index)
        {
            EmapyPoint point = new EmapyPoint()
            {
                Id = symfoniaItem.Id,
                Catalog = symfoniaItem.Katalog,
                Name = symfoniaItem.Nazwa,
                Marker = ConvertMarker(symfoniaItem.ZnacznikShortCut),
                Lp = index,
                //PhoneNumber = symfoniaItem.PhoneNumber,
                //Email = symfoniaItem.email,
                SymfoniaKod = symfoniaItem.Kod,
                //email
                SymfoniaAdress = symfoniaItem.Adress,
                Rodzaj = Kind.Zwykly,
                Adress =
                {
                    City = symfoniaItem.Miasto,
                    Kraj = symfoniaItem.Kraj,
                    PostCode = symfoniaItem.KodPocztowy,
                    // TypeOfAdress = symfoniaItem.RodzajAdresu, //TODO: Nie obsługujemy tego w tej wersji
                    Ulica = symfoniaItem.Ulica,
                    //FlatNumber =
                    //PostPlace =      //TODO ODEBRA POCZTE Z ERP
                  // Wojewodztwo = symfoniaItem.Wojewodztwo
                    //HouseNumber =
                }
            };
            return point;
        }

        private static void LoadedPointToMapyPointComparator(EmapyPoint point, LoadedPoint foundedPoint)
        {
            point.Location = new Location(foundedPoint.LocationLati, foundedPoint.LocationLong);

            point.Adress.ConfidenceLevel = foundedPoint.AdressConfidence;
            point.MapyAdress = foundedPoint.MapyAdress;
        }

        private static void CreateBufforPoint(LoadedPoint newLoadedPoint, EmapyPoint point, BingMapsRESTToolkit.Location location)
        {
            newLoadedPoint.LocationLong = point.Location.Longitude;
            newLoadedPoint.LocationLati = point.Location.Latitude;
            newLoadedPoint.Id = point.Id;
            newLoadedPoint.CreateDate = DateTime.Now;
            newLoadedPoint.AdressConfidence = location.Confidence;
            newLoadedPoint.MapyAdress = location.Address.FormattedAddress;
            newLoadedPoint.SymfoniaAdress = point.SymfoniaAdress;
            newLoadedPoint.KodKontrahenta = point.SymfoniaKod;
        }

        public static string ConvertMarker(string znacznikShortCut)
        {
            string Znacznik = String.Empty;
            if (znacznikShortCut != "" && znacznikShortCut != "")
            {
                if (Convert.ToInt32(znacznikShortCut) > 90 && Convert.ToInt32(znacznikShortCut) < 101)
                {
                    Znacznik = (Convert.ToInt32(znacznikShortCut) - 91).ToString();
                }
                else
                {
                    Znacznik = Convert.ToChar(Convert.ToInt32(znacznikShortCut)).ToString();
                }
            }

            if (Znacznik == "\0")
            {
                Znacznik = " ";
            }
            return Znacznik;
        }

        private static List<StartStopData> FindStartStopById(List<StartStopData> list)
        {
            try
            {
                List<StartStopData> result = new List<StartStopData>();
                var start = (from item in list.Where(x => x.UserId == UserId)
                             select item).First();
                var stop = (from item in list.Where(x => x.UserId == UserId)
                            select item).Last();
                result.Add(start);
                result.Add(stop);

                return result;
            }
            catch (Exception)
            {
                List<StartStopData> result = new List<StartStopData>();
                return result;
            }
        }

        public static async Task<ObservableCollection<EmapyPoint>> GetStartStopAsync()
        {

            ObservableCollection<EmapyPoint> FoundedPoints = new ObservableCollection<EmapyPoint>();
            List<StartStopData> FoundedPoint = new List<StartStopData>();
            if (StartStopDataList != null)
            {
                if (StartStopDataList.Count != 0)
                {
                    FoundedPoint = FindStartStopById(StartStopDataList.ToList());
                }
            }

            if (FoundedPoint == null || !FoundedPoint.Any())
            {

                var location = await GlobalMethods.GetLocationAsync(StartStopAdress).ConfigureAwait(false);

                StartStopDataList = new ObservableCollection<StartStopData>();
                var startPoint = CreateStart(location);
                var stopPoint = CreateStop(location);

                FoundedPoints.Add(startPoint);
                FoundedPoints.Add(stopPoint);
                return FoundedPoints;
            }
            {
                foreach (var item in FoundedPoint)
                {
                    var newItem = CreateStartStop(item);
                    newItem.Adress.ConfidenceLevel = "Low";
                    FoundedPoints.Add(newItem);
                }

                return FoundedPoints;
            }

        }

        private static EmapyPoint CreateStop(BingMapsRESTToolkit.Location location)
        {
            EmapyPoint stopPoint = new EmapyPoint()
            {
                Name = "Stop",
                Id = UserId,
                Title = "Stop",
                IsHidden = false,
                MapyAdress = location.Address.FormattedAddress,

                Location = new Location()
                {
                    Latitude = location.Point.Coordinates[0],
                    Longitude = location.Point.Coordinates[1],
                },
                Rodzaj = Kind.Stop,
                Adress =
                {
                    ConfidenceLevel = "Low",
                }
            };
            if (location.Address.FormattedAddress == null)
            {
                stopPoint.MapyAdress = StartStopAdress;
            }
            return stopPoint;
        }

        private static EmapyPoint CreateStart(BingMapsRESTToolkit.Location location)
        {
            EmapyPoint startPoint = new EmapyPoint()
            {
                Name = "Start",
                Id = UserId,
                Title = "Start",
                IsHidden = false,
                Rodzaj = Kind.Start,
                MapyAdress = location.Address.FormattedAddress,
                Location = new Location()
                {
                    Latitude = location.Point.Coordinates[0],
                    Longitude = location.Point.Coordinates[1],
                },
                Adress =
                {
                    ConfidenceLevel = "Low",
                }
            };
            if (location.Address.FormattedAddress == null)
            {
                startPoint.MapyAdress = StartStopAdress;
            }
            return startPoint;
        }

        private static EmapyPoint CreateStartStop(StartStopData item)
        {
            EmapyPoint newItem = new EmapyPoint()
            {
                Adress =
                {
                    HouseNumber = item.HouseNumber,
                    //TypeOfAdress = item.TypeOfAdress,
                    Wojewodztwo = item.Wojewodztwo,
                    City = item.City,
                    FlatNumber = item.FlatNumber,
                    PostPlace = item.PostPlace,

                    //Ulica = item.Ulica,
                    PostCode = item.PostCode,
                    Kraj = item.Kraj,
                },
                MapyAdress = item.Adres,
                Rodzaj = item.Rodzaj,
                IsHidden = false,
                Id = item.UserId,
                Title = item.Title,
                Location = new Location()
                {
                    Longitude = (double)item.LocationLong,
                    Latitude = (double)item.LocationLat,
                }
            };
            if (newItem.Title == "Start")
            {
                newItem.Name = "Start";
                newItem.Rodzaj = Kind.Start;
            }
            if (newItem.Title == "Stop")
            {
                newItem.Name = "Stop";
                newItem.Rodzaj = Kind.Stop;
            }
            return newItem;
        }
    }
}