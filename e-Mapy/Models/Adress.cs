using eMapy.Utils;
using ProtoBuf;
using System;
using System.Diagnostics;
using BingMapsRESTToolkit;

namespace eMapy.Models
{
    [ProtoContract]

    public class Adress : NotifyPropertyChanged
    {

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly EmapyPoint _emapyPoint;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _city;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _houseNumber;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _postCode;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _postPlace;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _wojewodztwo;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _kraj;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _flatNumber;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _ulica;
        [ProtoMember(1)]
        public string FlatNumber
        {
            get { return _flatNumber; }
            set { _flatNumber = value; OnPropertyChanged("FlatNumber"); }
        } // pobieram

        public Adress(EmapyPoint emapyPoint)
        {
            _emapyPoint = emapyPoint;
        }
        public string City
        {
            get => _city;
            set
            {
                _city = value;
                _emapyPoint.OnPropertyChanged("City");
            }
        } //Pobierane

        public string HouseNumber
        {
            get => _houseNumber;
            set
            {
                _houseNumber = value;
                _emapyPoint.OnPropertyChanged("HouseNumber");
            }
        } // Pobieram ale jest flatNumber

        public string PostCode
        {
            get => _postCode;
            set
            {
                _postCode = value;
                _emapyPoint.OnPropertyChanged("PostCode");
            }
        } // pobieram

        public string PostPlace
        {
            get => _postPlace;
            set
            {
                _postPlace = value;
                _emapyPoint.OnPropertyChanged("PostPlace");
            }
        } // pobieram

        public string Wojewodztwo
        {
            get { return _wojewodztwo; }
            set
            {
                _wojewodztwo = value;
                OnPropertyChanged("Wojewodztwo");
            }
        } // pobieram

        public string Ulica
        {
            get { return _ulica; }
            set
            {
                _ulica = value;
                OnPropertyChanged("Ulica");
            }
        }
        public string Kraj
        {
            get { return _kraj; }
            set
            {
                _kraj = value;
                _emapyPoint.OnPropertyChanged("Kraj");
            }
        }

        private string confidenceLevel;
        public string ConfidenceLevel
        {
            get { return confidenceLevel; }
            set
            {
                confidenceLevel = value;
                OnPropertyChanged("ConfidenceLevel");
            }
        }


 


  
    }
}