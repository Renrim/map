using eMapy.Utils;
using Microsoft.Maps.MapControl.WPF;
using ProtoBuf;
using System.Diagnostics;
using System.Xml.Serialization;

namespace eMapy.Models
{
    [DebuggerDisplay("Lp: {Lp}, SymfoniaAdress: {SymfoniaAdress}, Marker:{Marker} Nazwa:{Name}")]
    [ProtoContract]
    public class EmapyPoint : PointBase
    {

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isFixed; 
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _lp;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isSelected;
        public EmapyPoint()
        {
            Adress = new Adress(this);
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _isHidden = false;
        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                _isHidden = value;
                OnPropertyChanged("IsHidden");
            }
        }
        public Adress Adress { get; }
        public string Catalog { get; set; }
        public string Email { get; set; }
        public bool IsFixed
        {
            get
            {
                return _isFixed;
            }
            set
            {
                _isFixed = value;
                OnPropertyChanged("IsFixed");
            }
        }

        // nie pobieram
        public int Lp // nie pobieram
        {
            get
            {
                return _lp;
            }
            set
            {
                _lp = value;
                OnPropertyChanged("Lp");
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }

        public string SymfoniaKod { get; set; }
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public string Marker { get; set; }
    }
}