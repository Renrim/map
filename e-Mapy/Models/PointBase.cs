using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using eMapy.Utils;
using Microsoft.Maps.MapControl.WPF;

namespace eMapy.Models
{
    public class PointBase : NotifyPropertyChanged

    {
        public string SymfoniaAdress { get; set; }

        private string _mapyAdress;
        public string MapyAdress
        {
            get { return _mapyAdress; }
            set
            {
                _mapyAdress = value;
                OnPropertyChanged("MapyAdress");
            }
        }

        public int Index { get; set; }
        [XmlElement("Id")]
        public int Id { get; set; }
        public bool HelperWithSection { get; set; } = false;
        public string Name { get; set; }//pobieram

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Location _location;
        public Location Location //Do zrobienia
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
                OnPropertyChanged("Location");
            }
        }
        public Kind Rodzaj { get; set; }
        public string Title { get; set; }
    }
}