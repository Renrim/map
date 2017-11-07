using System.Diagnostics;
using eMapy.Utils;

namespace eMapy.Models
{
    public class Options : PointBase
    {
        private bool _isMouseEnterEventActive = true;

        public bool IsMouseEnterEventActive
        {
            get { return _isMouseEnterEventActive; }
            set
            {
                _isMouseEnterEventActive = value;
                OnPropertyChanged("IsMouseEnterEventActive");
            }
        }


        private bool _adressGridVisibility;

        public bool AdressGridVisibility
        {
            get
            {
                return _adressGridVisibility;
            }
            set
            {
                _adressGridVisibility = value;
                OnPropertyChanged("AdressGridVisibility");
            }
        }

        private bool _isToggleButtonEnabled = true;

        public bool IsToggleButtonEnabled
        {
            get { return _isToggleButtonEnabled; }
            set
            {
                _isToggleButtonEnabled = value;
                OnPropertyChanged("IsToggleButtonEnabled");
            }
        }

        private bool _changingCoordsPerrmited;

        public bool ChangingCoordsPermitted
        {
            get { return _changingCoordsPerrmited; }
            set
            {
                this.AdressGridVisibility = false;
                _changingCoordsPerrmited = value;
                OnPropertyChanged("ChangingCoordsPermitted");
            }
        }

        private int _zoomlevel = 7;

        public int Zoomlevel
        {
            get { return _zoomlevel; }
            set
            {
                _zoomlevel = value;
                OnPropertyChanged("Zoomlevel");
            }
        }

        private bool _progressBarAnimation = false;

        public bool ProgressBarAnimation
        {
            get { return _progressBarAnimation; }
            set
            {
                _progressBarAnimation = value;
                OnPropertyChanged("ProgressBarAnimation");
            }
        }

        private bool _windowStateBool;

        public bool WindowStateBool
        {
            get { return _windowStateBool; }
            set
            {
                _windowStateBool = value;
                OnPropertyChanged("WindowStateBool");
            }
        }

        private bool showMarkers = false;
        public bool ShowMarkers
        {
            get { return showMarkers; }
            set
            {
                showMarkers = value;
                OnPropertyChanged("ShowMarkers");
            }
        }

        private bool _showHiddenPointsOnMap = true;
        public bool ShowHiddenPointsOnMap
        {
            get { return _showHiddenPointsOnMap; }
            set
            {
                _showHiddenPointsOnMap = value;
                OnPropertyChanged("ShowHiddenPointsOnMap");
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool checkBoxesEnabled = true;

        public bool CheckBoxesEnabled
        {
            get { return checkBoxesEnabled; }
            set
            {
                checkBoxesEnabled = value;
                OnPropertyChanged("CheckBoxesEnabled");
            }
        }

    }
}