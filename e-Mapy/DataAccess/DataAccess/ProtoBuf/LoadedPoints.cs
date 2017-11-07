using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace eMapy.DataAccess.DataAccess.ProtoBuf
{
    [ProtoContract]
    public class LoadedPoint
    {
        [ProtoMember(1)]
        public double LocationLong { get; set; }

        [ProtoMember(2)]
        public double LocationLati { get; set; }

        [ProtoMember(3)]
        public int Id { get; set; }

        [ProtoMember(4)]
        public DateTime CreateDate { get; set; }

        [ProtoMember(5)]
        public string AdressConfidence { get; set; }

        [ProtoMember(6)]
        public string MapyAdress { get; set; } = " ";

        [ProtoMember(7)]
        public string SymfoniaAdress { get; set; } = " ";

        [ProtoMember(8)]
        public string KodKontrahenta { get; set; }
    }
}