using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BingMapsRESTToolkit;
using eMapy.BusinessLogic;
using eMapy.BusinessLogic.Services;
using eMapy.DataAccess.DataAccess.ProtoBuf;
using eMapy.Models;
using eMapy.Utils;
using eMapy.ViewModels;
using Microsoft.Maps.MapControl.WPF;
using Location = BingMapsRESTToolkit.Location;

namespace eMapy.Managers
{
    public class MainManager : ManagerBase<MainWindowViewModel>
    {
        public MainManager(MainWindowViewModel param) : base(param)
        {
        }

        public void AllowToChangeLocation()
        {
            ViewModel.Options.ChangingCoordsPermitted = !ViewModel.Options.ChangingCoordsPermitted;
            //if (ViewModel.Licence == KindOfLicence.Basic)
            //{
            //    ViewModel.Licence = KindOfLicence.Full;
            //    return;
            //}
            //if (ViewModel.Licence == KindOfLicence.Full)
            //{
            //    ViewModel.Licence = KindOfLicence.Basic;
            //    return;
            //}

        }

        public void ChangeMapMode()
        {
            ViewModel.Mode = !ViewModel.Mode;
        }

        public void ClosingWindow()
        {
            var result = ViewModel.StartAndStopPoints;
            ObservableCollection<StartStopData> serializeDataList = ProtoMethods.Deserialize<ObservableCollection<StartStopData>>(ProtoMethods.StartStopDataListPath);
            foreach (var item in result)
            {
                if (serializeDataList == null)
                {
                    serializeDataList = new ObservableCollection<StartStopData>();
                }

                var protoItem = (from data in serializeDataList.Where
                                    (x => x.UserId == item.Id && x.Title == item.Title)
                                 select data).FirstOrDefault();

                if (protoItem == null)
                {
                    protoItem = new StartStopData
                    {
                        Adres = item.MapyAdress,

                        City = item.Adress.City,
                        FlatNumber = item.Adress.FlatNumber,
                        HouseNumber = item.Adress.HouseNumber,
                        Kraj = item.Adress.Kraj,
                        Wojewodztwo = item.Adress.Wojewodztwo,
                        LocationLat = item.Location.Latitude,
                        LocationLong = item.Location.Longitude,
                        //TypeOfAdress = item.Adress.TypeOfAdress,
                        UserId = item.Id,
                        PostPlace = item.Adress.PostPlace,
                        PostCode = item.Adress.PostCode,
                        Title = item.Title
                    };
                    //protoItem.Ulica = item.Adress.Ulica;
                    if (item.Title == "Start")
                    {
                        protoItem.Rodzaj = Kind.Start;
                    }
                    if (item.Title == "Stop")
                    {
                        protoItem.Rodzaj = Kind.Stop;
                    }
                    serializeDataList.Add(protoItem);
                }
                else
                {
                    protoItem.Adres = item.MapyAdress;
                    protoItem.City = item.Adress.City;
                    protoItem.FlatNumber = item.Adress.FlatNumber;
                    protoItem.HouseNumber = item.Adress.HouseNumber;
                    protoItem.Kraj = item.Adress.Kraj;
                    protoItem.Wojewodztwo = item.Adress.Wojewodztwo;
                    //protoItem.Ulica = item.Adress.Ulica;
                    protoItem.LocationLat = item.Location.Latitude;
                    protoItem.LocationLong = item.Location.Longitude;
                    //protoItem.TypeOfAdress = item.Adress.TypeOfAdress;
                    protoItem.UserId = item.Id;
                    protoItem.PostPlace = item.Adress.PostPlace;
                    protoItem.PostCode = item.Adress.PostCode;
                    protoItem.Title = item.Title;
                }
            }
            try
            {
                ProtoMethods.Serialize(ViewModel.DeserializedList, ProtoMethods.DistanceAndDurationPath);
                ProtoMethods.Serialize(serializeDataList, ProtoMethods.StartStopDataListPath);

                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Sage\\ListaKontrahentow.xml");
            }
            catch
            {
                // ignored
            }
        }
       private List<EmapyPoint> twoPoints = new List<EmapyPoint>();
        public List<SimpleWaypoint> ConvertedList;
        public void Optymalize(Section section)
        {
            ViewModel.DeserializedList =  ProtoMethods.Deserialize<List<DistanceAndDuration>>(ProtoMethods.DistanceAndDurationPath);
            ObservableCollection<EmapyPoint> points = new ObservableCollection<EmapyPoint> { section.StartPoint };
            foreach (var item in section.SectionOfAllPoints)
            {
                points.Add(item);
            }
            points.Add(section.StopPoint);
            try
            {
                int[][] dur = new int[points.Count][];
                int i = 0;
                foreach (EmapyPoint firstPoint in points)
                {
                    dur[i] = new int[points.Count];
                    int j = 0;
                    foreach (EmapyPoint secondPoint in points)
                    {
                        if (Equals(firstPoint, secondPoint))
                        {
                            dur[i][j] = 0;
                        }
                        else
                        {
                            Task.WaitAll();
                            var durationPoint = ProtoMethods.SelectDurationPoint(ViewModel.DeserializedList,
                                firstPoint.Location, secondPoint.Location);
                            //var distancePoint = ProtoMethods.SelectDistancePoint(_deserializedList, firstPoint.MapyAdress, secondPoint.MapyAdress);
                            if (durationPoint != null)
                            {
                                dur[i][j] = Convert.ToInt32(durationPoint);
                            }
                            else
                            {

                                try
                                {
                                    twoPoints = new List<EmapyPoint>();
                                    twoPoints.Add(firstPoint);
                                    twoPoints.Add(secondPoint);

                                    ConvertedList = GlobalMethods.ConvertEMapyPointToSimpleWaypoints(twoPoints);
                                    var route = GlobalMethods.GetRoute(ConvertedList);
                                    twoPoints.Clear();
                                    DistanceAndDuration distanceAndDuration = new DistanceAndDuration()
                                    {
                                        CreateDateTime = DateTime.Now,
                                        Distance = route.TravelDistance,
                                        Duration = route.TravelDuration,
                                        StartLatt = firstPoint.Location.Latitude,
                                        StartLong = firstPoint.Location.Longitude,
                                        StopLatt = secondPoint.Location.Latitude,
                                        StopLong = secondPoint.Location.Longitude,
                                    };

                                    ViewModel.DeserializedList.Add(distanceAndDuration);
                                    dur[i][j] = Convert.ToInt32(route.TravelDurationTraffic);
                                }
                                catch (Exception)
                                {
                                    ProtoMethods.Serialize(ViewModel.DeserializedList, ProtoMethods.DistanceAndDurationPath);
                                    ViewModel._optamalizationFailed = true;
                                    MessageBox.Show($"Problem ze znalezieniem trasy między:{Environment.NewLine}{twoPoints.First().Name}, a {twoPoints.Last().Name} {Environment.NewLine}Sprawdź połączenie sieciowe oraz lokalizacje kontrahentów na mapie ");
                                    return;
                                    
                                }                           
                            }
                        }
                        j += 1;
                    }

                    i += 1;
                }

                //int[] order; //global
                    ProtoMethods.Serialize(ViewModel.DeserializedList, ProtoMethods.DistanceAndDurationPath);
                    int indexer = 1;
                    foreach (var item in section.SectionOfAllPoints)
                    {
                        item.Index = indexer;
                        indexer += 1;
                    }
                    var order = new int[dur[0].Length];
                    if (GlobalMethods.AntColonyOpt(dur, 1, ref order) == false)
                    {
                        ViewModel._optamalizationFailed = true;
                        return;
                    }
                    var indexorder = 0;
                    foreach (int index in order)
                    {
                        foreach (var itemO in section.SectionOfAllPoints)
                        {
                            if (itemO.Index == index)
                            {
                                itemO.Lp = indexorder + section.StartPoint.Lp;
                            }
                        }
                        indexorder += 1;
                    }
                ViewModel._optamalizationFailed = false;

            }
            catch (Exception e)
            {
                ProtoMethods.Serialize(ViewModel.DeserializedList, ProtoMethods.DistanceAndDurationPath);
                ViewModel._optamalizationFailed = true;
            }
        }

        public void SetPushpinLocation()
        {
            try
            {
                Coordinate coords = new Coordinate
                {
                    Latitude = ViewModel.DraggingPoint.Location.Latitude,
                    Longitude = ViewModel.DraggingPoint.Location.Longitude
                };
                if (ViewModel.Licence == KindOfLicence.Basic)
                {
                    ViewModel.DraggingPushpin = new Pushpin();
                    ViewModel.IsDragging = false;
                    ViewModel.DraggingPoint = null;
                    return;
                }
                var result = GlobalMethods.GetAdressByCoordinatesAsync(coords).Result;
                PointLocationComparator(ViewModel.DraggingPoint, result);
                ViewModel.DraggingPushpin = new Pushpin();
                DeleteRoad();
                ViewModel.IsDragging = false;
                ViewModel.DraggingPoint = null;
            }
            catch (Exception)
            {
                ViewModel.DraggingPushpin = new Pushpin();
                ViewModel.DraggingPoint = new EmapyPoint();
            }
        }

        public void ZoomUp()
        {
            ViewModel.Options.Zoomlevel--;
        }

        public void ZoomDown()
        {
            ViewModel.Options.Zoomlevel++;
        }

        public void MoveDownInGrid(object parameter)
        {
            try
            {
                EmapyPoint selectedItem = parameter as EmapyPoint;
                if (selectedItem != null && selectedItem.IsFixed) return;
                EmapyPoint nextItem = (from item in ViewModel.PointsOnMap
                                       where selectedItem != null && (item.IsFixed == false && item.Lp > selectedItem.Lp)
                                       select item).FirstOrDefault();
                if (nextItem != null &&
                    (nextItem.Lp != ViewModel.PointsOnMap.Max(x => x.Lp) || nextItem.IsFixed == false))
                {
                    if (selectedItem != null)
                    {
                        int selectedItemLp = selectedItem.Lp;
                        int nextItemLp = nextItem.Lp;
                        selectedItem.Lp = nextItemLp;
                        nextItem.Lp = selectedItemLp;
                    }
                    ViewModel.PointsOnMap = new ObservableCollection<EmapyPoint>(ViewModel.PointsOnMap.OrderBy(d => d.Lp));
                    DeleteRoad();
                    ViewModel.Options.IsMouseEnterEventActive = true;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public async Task GetAllPointsAsync()
        {
            try
            {
                if (ViewModel.Licence == KindOfLicence.Basic)
                {

                    ViewModel.PointsOnMap = PointsServices.GetPointsAsync().Result;
                    Task.WaitAll();
                    EnableDisableInterface(true);
                    CadreMethod();

                    return;
                }
                

                ViewModel.PointsOnMap = Task.Run(PointsServices.GetPointsAsync).Result;
                ViewModel.StartAndStopPoints = Task.Run(PointsServices.GetStartStopAsync).Result;
                

                Task.WaitAll();
                foreach (var emapyPoint in ViewModel.PointsOnMap)
                {
                    ViewModel.EmapyPointsWithAdresses.Add(emapyPoint);
                }
                EnableDisableInterface(true);
                CadreMethod();
            }
            catch
            {
                EnableDisableInterface(true);
               
            }
        }

        public List<Section> MakeSections() // Wszystkie przedziały
        {
            try
            {
                List<Section> listOfSections = new List<Section>(); //result
                Section section = new Section(); //sekcja
                List<EmapyPoint> emapyPointsWithStartStop = new List<EmapyPoint> { ViewModel.StartAndStopPoints.First() };

                foreach (var item in ViewModel.PointsOnMap)
                {
                    emapyPointsWithStartStop.Add(item);
                }
                emapyPointsWithStartStop.Add(ViewModel.StartAndStopPoints.Last());

                section.StartPoint = ViewModel.StartAndStopPoints.First(); // Pierwsza sekcja, pierwszy punkt to zawsze punktStart
                foreach (var item in emapyPointsWithStartStop) //Dla punktu start nie robi już nic bo był dodany wcześniej
                {
                    //punkt: Nie zafiksowany, helperWithSection oraz nextHelperWithSection domyślnie false.
                    if (item.IsFixed == false && item.HelperWithSection == false && ViewModel.NextHelperWithSections == false &&
                        item.Rodzaj != Kind.Stop && item.Rodzaj != Kind.Start) //
                    {
                        item.HelperWithSection = true; // ustawiam item.helperWithSection = true
                        section.SectionOfAllPoints.Add(item);
                    }
                    else if (item.IsFixed) // Zafiksowany item.
                    {
                        section.StopPoint = item;
                        listOfSections.Add(section);
                        section = new Section
                        {
                            //Tworzymy nową sekcje.
                            StartPoint = item, // stopPoint poprzedniej sekcji jest StartPointem nowej.
                            StopPoint = ViewModel.StartAndStopPoints.Last()
                        }; //Dla nowej sekcji stopPoint domyślnie będzie ostatniPunkt - stop. Jesli po drodze będzie zafiksowany
                        ViewModel.NextHelperWithSections = true; //item to ten item nadpisuje stopPoint 5 linijek wyżej.
                    }
                    else if (item.IsFixed == false && ViewModel.NextHelperWithSections
                    ) // nextHelperWithSection wykorzystywany aby nie puścić punktu startu.
                    {
                        section.SectionOfAllPoints.Add(item);
                        ViewModel.NextHelperWithSections = false;
                    }
                    else if (item.Rodzaj == Kind.Stop)
                    {
                        section.StopPoint = ViewModel.StartAndStopPoints.Last();
                        listOfSections.Add(section);
                    }
                }

                foreach (var item in emapyPointsWithStartStop) // zmiana na domyslne wartości
                {
                    item.HelperWithSection = false;
                }

                ViewModel.NextHelperWithSections = false;
                return listOfSections;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task CalculateRouteAsync(ObservableCollection<EmapyPoint> mapPoints)
        {
            try
            {
                List<BingMapsRESTToolkit.Route> routeList = new List<BingMapsRESTToolkit.Route>();
                List<EmapyPoint> helperList = new List<EmapyPoint> { ViewModel.StartAndStopPoints.First() };

                foreach (var item in mapPoints)
                {
                    helperList.Add(item);
                }
                helperList.Add(ViewModel.StartAndStopPoints.Last());
                var waypointsResult = GlobalMethods.ConvertEMapyPointToSimpleWaypoints(helperList);
                var chunkedList = GlobalMethods.ChunkBy(waypointsResult, 25);
                if (waypointsResult.Count >= 25)
                {
                    foreach (var list in chunkedList)
                    {
                        var route = GlobalMethods.GetRoute(list);

                        if (chunkedList[0] == list)
                        {
                            routeList.Add(route);
                        }
                        else
                        {
                            routeList.Add(route);
                        }
                    }
                }
                else
                {
                    var route = GlobalMethods.GetRoute(waypointsResult);
                    routeList.Add(route);
                }
                if (chunkedList.Count != 1 || routeList.Count != 0)
                {
                    for (int i = 0; i < chunkedList.Count - 1; i++)
                    {
                        List<SimpleWaypoint> pointsOfBreak =
                            new List<SimpleWaypoint> { chunkedList[i].Last(), chunkedList[i + 1].First() };
                        var route = GlobalMethods.GetRoute(pointsOfBreak);
                        routeList.Add(route);
                    }
                }

                ViewModel.RouteResult = routeList;
                TimeSpan fromSeconds = new TimeSpan();
                foreach (var item in routeList)
                {
                    fromSeconds += TimeSpan.FromSeconds(item.TravelDuration);
                }

                var hours = fromSeconds.Hours;
                if (fromSeconds.Days > 0)
                {
                    var hoursFromDays = fromSeconds.Days * 24;
                    hours = fromSeconds.Hours + hoursFromDays;
                }

                string duration = $"{hours}h:{fromSeconds.Minutes}m";
                ViewModel.DystansPoOptymalizacji = $"{ViewModel.RouteResult[0].TravelDistance} km";
                ViewModel.CzasPoOptymalizacji = $"{duration}";
                ViewModel.Cadre = !ViewModel.Cadre;
                Application.Current.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            }
            catch
            {
                MessageBox.Show("Problem z wyświetleniem trasy, sprawdź połączenie sieciowe");
            }
        }

        public void EnableDisableInterface(bool param)
        {
            if (param)
            {
                ViewModel.Options.ProgressBarAnimation = false;
                ViewModel.Options.IsToggleButtonEnabled = true;
                ViewModel.Options.IsMouseEnterEventActive = true;
                ViewModel.Options.CheckBoxesEnabled = true;
            }
            else
            {
                ViewModel.Options.AdressGridVisibility = false;
                ViewModel.Options.IsToggleButtonEnabled = false;
                ViewModel.Options.CheckBoxesEnabled = false;
                ViewModel.Options.ProgressBarAnimation = true;
            }
        }

        public async Task OptymalizationAsync()
        {
            try
            {
                if (ViewModel.PointsOnMap.Count > 15)
                {
                    MessageBox.Show("Wybrano więcej niż 15 punktów");
                    return;
                }
                EnableDisableInterface(false);

                DeleteRoad();

                List<Section> sectionToOpT = MakeSections();
                foreach (var item in sectionToOpT)
                {
                    Optymalize(item);
                }

                if (ViewModel._optamalizationFailed)
                {
                    EnableDisableInterface(true);
                    return;
                }
                ViewModel.PointsOnMap = new ObservableCollection<EmapyPoint>(ViewModel.PointsOnMap.OrderBy(d => d.Lp));

                CalculateRouteAsync(this.ViewModel.PointsOnMap).Wait();
                ViewModel.ListOfSections.Clear();

                EnableDisableInterface(true);
            }
            catch (Exception)
            {
                EnableDisableInterface(true);
            }
        }

        public async Task SortItemsAsync()
        {
            int itemLp = 1;
            foreach (var item in ViewModel.PointsOnMap)
            {
                item.Lp = itemLp++;
            }
            //for (int i = 0; i < PointsOnMap.Count; i++)
            //{
            //    PointsOnMap[i].Lp = i + 1;
            //}
        }

        public async Task RestoreItemAsync(object parameter)
        {
            EmapyPoint selectedItem = parameter as EmapyPoint;
            foreach (var iPoint in ViewModel.PointsOnMap.Where(x => x.IsSelected && x.Id != selectedItem.Id))
            {
                iPoint.IsSelected = false;
            }
            var lastNonFixedIndex = (this.ViewModel.PointsOnMap.Where(x => x.IsFixed == false).Select(item => item)).LastOrDefault();

            if (lastNonFixedIndex != null)
            {
                ViewModel.PointsOnMap.Insert(lastNonFixedIndex.Lp, selectedItem);
                ViewModel.EmapyPointsWithAdresses.Add(selectedItem);
                ViewModel.HiddenPoints.Remove(selectedItem);
                Task.Run(this.SortItemsAsync);
            }
            else
            {
                EmapyPoint point = ViewModel.PointsOnMap.LastOrDefault();
                if (point != null)
                {
                    if (selectedItem != null)
                    {
                        selectedItem.Lp = point.Lp + 1;
                        ViewModel.PointsOnMap.Add(selectedItem);
                    }
                }
                else
                {
                    if (selectedItem != null)
                    {
                        selectedItem.Lp = 1;
                        ViewModel.PointsOnMap.Add(selectedItem);
                    }
                }

                ViewModel.EmapyPointsWithAdresses.Add(selectedItem);
                ViewModel.HiddenPoints.Remove(selectedItem);
            }

            if (selectedItem != null) selectedItem.IsHidden = false;
            DeleteRoad();
            ViewModel.Options.AdressGridVisibility = false;
        }

        public async Task ChangeAdressButtonAsync()
        {
            try
            {
                if (ViewModel.Options.AdressGridVisibility == false)return;
             
                EmapyPoint currentItem = new EmapyPoint();
                if (ViewModel.AdresGridPoint.Rodzaj == Kind.Start || this.ViewModel.AdresGridPoint.Rodzaj == Kind.Stop)
                {
                    currentItem = (ViewModel.StartAndStopPoints.Where(item => item.Rodzaj == ViewModel.AdresGridPoint.Rodzaj)).FirstOrDefault();
                }
                else if (ViewModel.AdresGridPoint.Rodzaj == Kind.Zwykly)
                {
                    currentItem = (ViewModel.PointsOnMap.Where(item => item.Lp == ViewModel.AdresGridPoint.Lp)).FirstOrDefault();
                }

                if (string.IsNullOrEmpty(ViewModel.AdresGridPoint.MapyAdress))
                {
                    MessageBox.Show("SymfoniaAdress nie może być pusty");
                    return;
                }

                var location = await GlobalMethods.GetLocationAsync(ViewModel.AdresGridPoint.MapyAdress);
                if (currentItem != null)
                {
                    PointLocationComparator(currentItem, location);
                    ViewModel.Center.Latitude = currentItem.Location.Latitude;
                    ViewModel.Center.Longitude = currentItem.Location.Longitude;
                }

                ViewModel.Options.AdressGridVisibility = false;
                ViewModel.Options.IsMouseEnterEventActive = true;
                DeleteRoad();
                ViewModel.Cadre = !ViewModel.Cadre;
                Application.Current.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Podaj dokladniejszy adres");
            }
        }

        public void ShowAdressGrid(object parameter)
        {
            try
            {
                if (parameter == null) return;
                ViewModel.Options.AdressGridVisibility = true;
                ViewModel.AdresGridPoint = parameter as EmapyPoint;
                if (ViewModel.AdresGridPoint != null)
                {
                    ViewModel.AdresGridPoint.IsSelected = true;
                }

                ViewModel.Options.IsMouseEnterEventActive = true;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void ShowHiddenPoints()
        {
            ViewModel.Options.ShowHiddenPointsOnMap = !ViewModel.Options.ShowHiddenPointsOnMap;
        }

        public async Task ChangeMarkerAndNumberViewAsync()
        {
            //System.Windows.Forms.MessageBox.Show($"{GlobalMethods.counter}");
            ViewModel.Options.ShowMarkers = !ViewModel.Options.ShowMarkers;
        }

        public void DeleteRoad()
        {
            try
            {
                if (ViewModel.MapLayer.Children.Count != 0)
                {
                    ViewModel.MapLayer.Children.Clear();
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task DeletePointAsync(object parameter)
        {
            if (parameter == null)
            {
                return;
            }

            //var  type = parameter.GetType();

            DeleteRoad();
            ViewModel.Options.AdressGridVisibility = false;
            ViewModel.Options.IsMouseEnterEventActive = true;
            EmapyPoint selectedItem = parameter as EmapyPoint;
            if (selectedItem.Lp == 0 && selectedItem.SymfoniaAdress == null && selectedItem.Location == null)
            {
                return;
            }

            selectedItem.IsFixed = false;
            selectedItem.IsHidden = true;
            ViewModel.HiddenPoints.Add(selectedItem);
            ViewModel.EmapyPointsWithAdresses.Remove(selectedItem);
            ViewModel.PointsOnMap.Remove(selectedItem);
            Task.Run(SortItemsAsync);

            ViewModel.Options.IsMouseEnterEventActive = true;
            ViewModel.SelectedEmapyPoint = new EmapyPoint();
        }

        public async Task DrawRoadAsync()
        {
            EnableDisableInterface(false);
            Task task = Task.Run(async
                () =>
            {
                await CalculateRouteAsync(ViewModel.PointsOnMap);
                EnableDisableInterface(true);
            });
        }

        public void MoveUpInGrid(object parameter)
        {
            try
            {
                EmapyPoint selectedItem = parameter as EmapyPoint;
                if (selectedItem != null && selectedItem.IsFixed)
                {
                    return;
                }
                EmapyPoint previousItem = (ViewModel.PointsOnMap.Where(item =>
                    selectedItem != null && (item.IsFixed == false && item.Lp < selectedItem.Lp))).Last();
                if (ViewModel.PointsOnMap.Any() && previousItem.IsFixed != true)
                {
                    if (selectedItem != null)
                    {
                        int selectedItemLp = selectedItem.Lp;
                        int previousItemLp = previousItem.Lp;
                        selectedItem.Lp = previousItemLp;
                        previousItem.Lp = selectedItemLp;
                    }
                    ViewModel.PointsOnMap = new ObservableCollection<EmapyPoint>(ViewModel.PointsOnMap.OrderBy(d => d.Lp));
                    DeleteRoad();
                    ViewModel.Options.IsMouseEnterEventActive = true;
                }
            }
            catch
            {
                // ignored
            }
        }

        public void BtnSendToGoogle() //TODO: Jesli jest więcej nż 25 punktów..
        {
            try
            {
                ViewModel.Options.AdressGridVisibility = false;
                ViewModel.Options.IsMouseEnterEventActive = true;
                //Location nic = GlobalMethods.GetLokalizacjaBING("Oleszyce" );
                EmapyPoint startPoint = ViewModel.StartAndStopPoints.First();
                EmapyPoint stopPoint = ViewModel.StartAndStopPoints.Last();

                string adres = "https://www.google.com/maps/dir/";
                //adres += string.Format(
                //    $"'{startPoint.Location.Latitude.ToString().Replace(",", ".")},{startPoint.Location.Longitude.ToString().Replace(",", ".")}'/");

                foreach (EmapyPoint item in ViewModel.PointsOnMap)
                {
                    adres += string.Format(
                        $"'{item.Location.Latitude.ToString().Replace(",", ".")},{item.Location.Longitude.ToString().Replace(",", ".")}'/");
                }

                adres += string.Format(
                    $"'{stopPoint.Location.Latitude.ToString().Replace(",", ".")},{stopPoint.Location.Longitude.ToString().Replace(",", ".")}'/");
                Process.Start(adres);
            }
            catch (Exception)
            {
            }
        }

        public void ShowContractorInSymfonia(object parameter)
        {
            var point = parameter as EmapyPoint;
            DataAccess.DataAccess.Xml.Contractors.SerializeIdToXML(null, point, ViewModel.Options);
        }

        public void ClearSelectedItem(object item)
        {
            foreach (var iPoint in ViewModel.PointsOnMap.Where(x => x.IsSelected == true))
            {
                iPoint.IsSelected = false;
            }
            ViewModel.SelectedEmapyPoint = new EmapyPoint();
            ViewModel.SelectedPoints.Clear();
        }

        public void CloseAdressGrid()
        {
            ViewModel.Options.IsMouseEnterEventActive = true;
            ViewModel.Options.AdressGridVisibility = false;
        }

        public void SetSelecteditem(object item)
        {
            ViewModel.SelectedEmapyPoint = item as EmapyPoint;
            ViewModel.SelectedEmapyPoint.IsSelected = true;
        }

        public void SetMouseEnterActionsOff()
        {
            ViewModel.Options.IsMouseEnterEventActive = false;
        }

        public void SetMouseEnterActionsOn()
        {
            ViewModel.Options.IsMouseEnterEventActive = true;
        }

        public void CadreToOne(object param)
        {
            ViewModel.Options.IsMouseEnterEventActive = false;
            EmapyPoint point = param as EmapyPoint;
            ViewModel.SelectedPoints.Clear();
            ViewModel.SelectedPoints.Add(point);
            CadreMethod();
            ViewModel.OnPropertyChanged("Cadre");
            ViewModel.SelectedPoints.Clear();
            ViewModel.Options.IsMouseEnterEventActive = true;
        }

        public void CadreMethod()
        {
            if (ViewModel.Options.AdressGridVisibility)
            {
                return;
            }
            ViewModel.Cadre = !ViewModel.Cadre;
            Application.Current.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);

            return;
        }

        public void PreparePushpinToDrag(object sender)
        {
            ViewModel.DraggingPushpin = new Pushpin();
            ViewModel.DraggingPushpin = (Pushpin)sender;
            ViewModel.DraggingPoint = (EmapyPoint)ViewModel.DraggingPushpin.Tag;

            // Enable Dragging
            ViewModel.IsDragging = true;
            //MapLayerChanged();
        }

        public async Task UpdateLocationInBufforAsync(object param)
        {
            ViewModel.BufforedList = ProtoMethods.Deserialize<List<LoadedPoint>>(ProtoMethods.LoadedPointsPath);
            EmapyPoint point = param as EmapyPoint;
            LoadedPoint foundedPoint = PointsServices.FindLoadedDataDataByAdress(point.SymfoniaAdress, point.SymfoniaKod, ViewModel.BufforedList);
            if (foundedPoint == null) return;
            foundedPoint.AdressConfidence = point.Adress.ConfidenceLevel;
            foundedPoint.MapyAdress = point.MapyAdress;
            foundedPoint.LocationLati = point.Location.Latitude;
            foundedPoint.LocationLong = point.Location.Longitude;
            ProtoMethods.Serialize(ViewModel.BufforedList, ProtoMethods.LoadedPointsPath);
        }

        public static void PointLocationComparator(EmapyPoint point, Location location)
        {
            point.Location = new Microsoft.Maps.MapControl.WPF.Location();
            point.Location.Longitude = location.Point.Coordinates[1];
            point.Location.Latitude = location.Point.Coordinates[0];
            point.MapyAdress = location.Address.FormattedAddress;
            point.Adress.ConfidenceLevel = location.Confidence;
        }
    }
}