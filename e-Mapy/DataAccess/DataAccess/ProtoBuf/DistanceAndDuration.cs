using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace eMapy.DataAccess.DataAccess.ProtoBuf
{
    [ProtoContract]
    public class DistanceAndDuration
    {
        [ProtoMember(1)]
        public DateTime CreateDateTime { get; set; }

        [ProtoMember(2)]
        public double Distance { get; set; }

        [ProtoMember(3)]
        public double Duration { get; set; }

        [ProtoMember(4)]
        public double StartLong { get; set; }

        [ProtoMember(5)]
        public double StartLatt { get; set; }

        [ProtoMember(6)]
        public double StopLong { get; set; }

        [ProtoMember(7)]
        public double StopLatt { get; set; }
    }

    [XmlType]
    public class DistanceAndDurationArchiveList
    {
    }
}