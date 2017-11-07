using BingMapsRESTToolkit;
using eMapy.BusinessLogic;
using eMapy.BusinessLogic.Services;
using eMapy.DataAccess.DataAccess.ProtoBuf;
using eMapy.Models;
using eMapy.Utils;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
//using DataAccess.DataAccess.AzureAccess;
using eMapy.DTOConverters;
using eMapy.Managers;
using eMapy.Models.Licencing;
using MessageBox = System.Windows.Forms.MessageBox;

namespace eMapy.ViewModels
{
    public partial class MainWindowViewModel : PointBase
    {
        #region fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ObservableCollection<EmapyPoint> EmapyPointsWithAdresses = new ObservableCollection<EmapyPoint>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsDragging;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public List<Section> ListOfSections = new List<Section>();

        public bool NextHelperWithSections;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private EmapyPoint _adresGridPoint = new EmapyPoint();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SolidColorBrush _backgroundBrush = new SolidColorBrush(Colors.White);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _cadre;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Microsoft.Maps.MapControl.WPF.Location _center = new Microsoft.Maps.MapControl.WPF.Location(Convert.ToDouble("50,0181515"), Convert.ToDouble("21,9732719"));

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _czasPoOptymalizacji;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _dystansPoOptymalizacji;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ObservableCollection<EmapyPoint> _emapypointsstartstop = new ObservableCollection<EmapyPoint>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ObservableCollection<EmapyPoint> _hiddenPoints = new ObservableCollection<EmapyPoint>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MapLayer _mapLayer = new MapLayer();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _mode = true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool _optamalizationFailed;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Options _options = new Options();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ObservableCollection<EmapyPoint> _pointsOnMap = new ObservableCollection<EmapyPoint>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<BingMapsRESTToolkit.Route> _routeResult = new List<BingMapsRESTToolkit.Route>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private EmapyPoint _selectedEmapyPoint = new EmapyPoint();

        private string _sessionKey;
        private ObservableCollection<EmapyPoint> _selectedPoints = new ObservableCollection<EmapyPoint>();

        #endregion fields
 
        public MainManager MainManager { get; set; }
        public LicenceManager LicenceManager { get; set; }

        //TODO: Mozliwosc zmiany adresu pkt start/stop jesli są na sobie
        public MainWindowViewModel()
        {

          


            MainManager = new MainManager(this);
            LicenceManager = new LicenceManager(this);
            MainManager.EnableDisableInterface(false);
            AsyncCommandMapLoaded = new AsyncCommand(x => Task.Run(MainManager.GetAllPointsAsync));
            AsyncCommandOptymalization = new AsyncCommand(x => Task.Run(MainManager.OptymalizationAsync), IsBasicVersion); // IsEnabled binding
            AsyncCommandDrawRoad = new AsyncCommand(x => Task.Run(MainManager.DrawRoadAsync), IsBasicVersion); // IsEnabled binding
            CommandSendToGoogle = new RelayCommand(param1 => MainManager.BtnSendToGoogle(), IsBasicVersion); // IsEnabled binding
            CommandContractorList = new RelayCommand(param15 => DataAccess.DataAccess.Xml.Contractors.SerializeIdToXML(PointsOnMap.ToList(), null, Options), IsBasicVersion); // IsEnabled Binding
            AsyncCommandDeleteItem = new AsyncCommand(MainManager.DeletePointAsync, IsParameterNotNull);
            AsyncCommandRestoreItem = new AsyncCommand(MainManager.RestoreItemAsync, IsParameterNotNull);
            AsyncCommandChangeAdressButton = new AsyncCommand(param10 => Task.Run(MainManager.ChangeAdressButtonAsync), CanUserUseApp);
            CommandSetPushpinLocation = new RelayCommand(param18 => MainManager.SetPushpinLocation(), CanUserChangeLocations);
            AsyncCommandRestoreLastItem = new AsyncCommand(x => MainManager.RestoreItemAsync(HiddenPoints.Last()), CanUserRestoreItem);
            AsyncCommandUpdateLocationInBufforAsync = new AsyncCommand(MainManager.UpdateLocationInBufforAsync);
            CommandMoveUp = new RelayCommand(MainManager.MoveUpInGrid, IsParameterNotNull);
            CommandMoveDown = new RelayCommand(MainManager.MoveDownInGrid, IsParameterNotNull);
            CommandZoomUp = new RelayCommand(param2 => MainManager.ZoomUp());
            CommandZoomDown = new RelayCommand(param3 => MainManager.ZoomDown());
            CommandShowAdressGrid = new RelayCommand(MainManager.ShowAdressGrid, CanUserUseApp);
            CommandCloseAdressGrid = new RelayCommand(param11 => MainManager.CloseAdressGrid(), CanUserUseApp);
            CommandClosingWindow = new RelayCommand(param14 => MainManager.ClosingWindow(), IsBasicVersion);
            CommandSelectedContractor = new RelayCommand(MainManager.ShowContractorInSymfonia, IsParameterNotNull);
            CommandSetSelectedItem = new RelayCommand(MainManager.SetSelecteditem, CanUserUseApp);
            CommandClearSelectedItem = new RelayCommand(MainManager.ClearSelectedItem, CanUserUseApp);
            CommandSetMouseEnterActionsOff = new RelayCommand(param16 => MainManager.SetMouseEnterActionsOff(), CanUserUseApp);
            CommandSetMouseEnterActionsOn = new RelayCommand(param17 => MainManager.SetMouseEnterActionsOn(), CanUserUseApp);
            CommandPreparePushpinToDrag = new RelayCommand(MainManager.PreparePushpinToDrag, CanUserChangeLocations);
            CommandCadre = new RelayCommand(x => MainManager.CadreMethod(), IsBasicVersion);
            CommandAllowToChangeLocation = new RelayCommand(x => MainManager.AllowToChangeLocation(), ChangeCoordsForBasic);
            CommandChangeMarkerAndNumberView = new AsyncCommand(x => Task.Run(MainManager.ChangeMarkerAndNumberViewAsync));
            CommandShowHiddenPointsOnMap = new RelayCommand(x => MainManager.ShowHiddenPoints(), IsBasicVersion);
            CommandChangeMapMode = new RelayCommand(x => MainManager.ChangeMapMode(), IsBasicVersion);


            try
            {
                User = LicenceManager.CheckLicence();
                if (User.Licence.KindOfLicence == "Basic")
                {
                    Licence = KindOfLicence.Basic;
                    Licence = KindOfLicence.Basic;
                    Licence = KindOfLicence.Basic;

                }
                if (User.Licence.KindOfLicence == "Demo")
                {
                    Licence = KindOfLicence.Demo;
                }
                if (User.Licence.KindOfLicence == "Full")
                {
                    Licence = KindOfLicence.Full;
                }

                CredentialKeyUsedInStart = User.Key.KeyValue;
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.ToString()}");
                throw;
            }
            
        }

        #region CanExecute

        public bool ChangeCoordsForBasic(object param)
        {
            if (Licence == KindOfLicence.Basic)
            {
                return true;
            }

            if (Options.ProgressBarAnimation)
            {
                return false;
            }
            return true;
        }

        private bool IsBasicVersion(object param)
        {
            if (Licence == KindOfLicence.Basic)
            {
                return false;
            }
            return true;
        }

        private bool AreTheyPointsOnMap(object param)
        {
            if (PointsOnMap.Count == 0 || Options.ProgressBarAnimation || Licence == KindOfLicence.Basic)
            {
                return false;
            }
            return true;
        }

        private bool CanUserChangeLocations(object param)
        {
            if (Licence == KindOfLicence.Basic && Options.ChangingCoordsPermitted)
            {
                return true;
            }

            if (Options.ProgressBarAnimation || !Options.ChangingCoordsPermitted)
            {
                return false;
            }
            return true;
        }

        private bool CanUserRestoreItem(object param)
        {
            if (HiddenPoints.Count == 0)
            {
                return false;
            }

            if (Options.AdressGridVisibility == true || Options.ProgressBarAnimation == true   || Licence == KindOfLicence.Basic )
            {
                return false;
            }
            return true;
        }

        private bool CanUserUseApp(object param)
        {
            if (Licence == KindOfLicence.Basic)
            {
                return false;
            }
            if (Options.ProgressBarAnimation)
            {
                return false;
            }
            return true;
        }

        private bool IsParameterNotNull(object param)
        {
            try
            {
                if (Options.ProgressBarAnimation || param == null || Licence == KindOfLicence.Basic)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion CanExecute

        #region Props

        private KindOfLicence _licence;

        public KindOfLicence Licence
        {
            get { return _licence; }
            set
            { 
                if (value != _licence)
                {
                    _licence = value;
                    OnPropertyChanged("Licence");
                }
            }
        }

        public List<DistanceAndDuration> DeserializedList { get; set; } = new List<DistanceAndDuration>();

        public static string CredentialKey { get; set; } 
          

        public string CredentialKeyUsedInStart { get; set; } 
           
        public EmapyPoint AdresGridPoint
        {
            get => _adresGridPoint;
            set
            {
                _adresGridPoint = value;
                OnPropertyChanged("AdresGridPoint");
            }
        }

        public SolidColorBrush BackgroundBrush
        {
            get => _backgroundBrush;
            set
            {
                _backgroundBrush = value;
                OnPropertyChanged("BackgroundBrush");
            }
        }

        public List<LoadedPoint> BufforedList { get; set; }

        public bool Cadre
        {
            get { return _cadre; }
            set
            {
                _cadre = value;
                OnPropertyChanged("Cadre");
            }
        }

        public Microsoft.Maps.MapControl.WPF.Location Center
        {
            get => _center;
            set
            {
                if (IsDragging)
                {
                    return;
                }
                _center = value;
                OnPropertyChanged("Center");
            }
        }

        public string CzasPoOptymalizacji // godziny + minuty
        {
            get => _czasPoOptymalizacji;
            set
            {
                _czasPoOptymalizacji = value;
                OnPropertyChanged("CzasPoOptymalizacji");
            }
        }

        public EmapyPoint DraggingPoint { get; set; } = new EmapyPoint();

        public Pushpin DraggingPushpin { get; set; } = new Pushpin();

        public string DystansPoOptymalizacji
        {
            get => _dystansPoOptymalizacji;
            set
            {
                _dystansPoOptymalizacji = value;
                OnPropertyChanged("DystansPoOptymalizacji");
            }
        }

        public User User { get; set; }
        public ObservableCollection<EmapyPoint> HiddenPoints
        {
            get => _hiddenPoints;
            set
            {
                _hiddenPoints = value;
                OnPropertyChanged("HiddenPoints");
            }
        }

        public MapLayer MapLayer
        {
            get => _mapLayer;
            set
            {
                _mapLayer = value;
                OnPropertyChanged("MapLayer");
            }
        }

        public bool Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                OnPropertyChanged("Mode");
            }
        }

        public Options Options
        {
            get => _options;
            set
            {
                _options = value;
                OnPropertyChanged("Options");
            }
        }

        public ObservableCollection<EmapyPoint> PointsOnMap
        {
            get { return _pointsOnMap; }
            set
            {
                _pointsOnMap = value;
                OnPropertyChanged("PointsOnMap");
            }
        }

        public ObservableCollection<EmapyPoint> SelectedPoints
        {
            get { return _selectedPoints; }
            set
            {
                _selectedPoints = value;
                OnPropertyChanged("SelectedPoints");
            }
        }

        public List<BingMapsRESTToolkit.Route> RouteResult
        {
            get => _routeResult;
            set
            {
                _routeResult = value;
                OnPropertyChanged("RouteResult");
            }
        }

        public EmapyPoint SelectedEmapyPoint
        {
            get { return _selectedEmapyPoint; }
            set
            {
                _selectedEmapyPoint = value;
                OnPropertyChanged("SelectedEmapyPoint");
            }
        }

        public string SessionKey
        {
            get
            {
                return this._sessionKey;
            }
            set
            {
                CredentialKey = value;
                this._sessionKey = value;
            }
        }

        public ObservableCollection<EmapyPoint> StartAndStopPoints
        {
            get => _emapypointsstartstop;

            set
            {
                _emapypointsstartstop = value;
                OnPropertyChanged("StartAndStopPoints");
            }
        }

        #endregion Props

        #region RelayCommands

        public RelayCommand CommandCadre { get; }
        public AsyncCommand AsyncCommandChangeAdressButton { get; }
        public AsyncCommand AsyncCommandMapLoaded { get; }
        public AsyncCommand CommandChangeMarkerAndNumberView { get; }
        public RelayCommand CommandClearSelectedItem { get; }
        public RelayCommand CommandCloseAdressGrid { get; }
        public RelayCommand CommandClosingWindow { get; }
        public RelayCommand CommandContractorList { get; }
        public AsyncCommand AsyncCommandDeleteItem { get; }
        public AsyncCommand AsyncCommandDrawRoad { get; set; }
        public RelayCommand CommandMoveDown { get; }
        public RelayCommand CommandMoveUp { get; }
        public AsyncCommand AsyncCommandOptymalization { get; }
        public RelayCommand CommandPreparePushpinToDrag { get; }
        public AsyncCommand AsyncCommandRestoreItem { get; }
        public AsyncCommand AsyncCommandRestoreLastItem { get; }
        public RelayCommand CommandSelectedContractor { get; }
        public RelayCommand CommandSendToGoogle { get; }
        public RelayCommand CommandSetMouseEnterActionsOff { get; }
        public RelayCommand CommandSetMouseEnterActionsOn { get; }
        public RelayCommand CommandSetPushpinLocation { get; }
        public RelayCommand CommandSetSelectedItem { get; }
        public RelayCommand CommandShowAdressGrid { get; }
        public AsyncCommand AsyncCommandUpdateLocationInBufforAsync { get; }
        public RelayCommand CommandZoomDown { get; }
        public RelayCommand CommandZoomUp { get; }
        public RelayCommand CommandAllowToChangeLocation { get; }
        public RelayCommand CommandShowHiddenPointsOnMap { get; }
        public RelayCommand CommandChangeMapMode { get; }

        #endregion RelayCommands
    }
}