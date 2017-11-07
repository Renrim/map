using eMapy.Models;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace eMapy.DataAccess.DataAccess.ProtoBuf
{
    [ProtoContract]
    public class StartStopData
    {
        [ProtoMember(1)]
        public double LocationLat { get; set; }

        [ProtoMember(2)]
        public double LocationLong { get; set; }

        [ProtoMember(3)]
        public string Adres { get; set; }

        [ProtoMember(4)]
        public string City { get; set; }

        [ProtoMember(5)]
        public string HouseNumber { get; set; }

        [ProtoMember(6)]
        public string PostCode { get; set; }

        [ProtoMember(7)]
        public string PostPlace { get; set; }

        [ProtoMember(8)]
        public string Wojewodztwo { get; set; }

        [ProtoMember(9)]
        public Int32 UserId { get; set; }

        [ProtoMember(10)]
        public string Kraj { get; set; }

        [ProtoMember(11)]
        public string FlatNumber { get; set; }

        [ProtoMember(12)]
        public string Ulica { get; set; }

        [ProtoMember(13)]
        public string Title { get; set; }

        [ProtoMember(14)]
        public Kind Rodzaj { get; set; }

        [ProtoMember(15)]
        public string TypeOfAdress { get; set; }
    }

   
}