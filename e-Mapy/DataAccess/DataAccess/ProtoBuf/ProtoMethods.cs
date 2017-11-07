using Microsoft.Maps.MapControl.WPF;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace eMapy.DataAccess.DataAccess.ProtoBuf
{
    [XmlType]
    public class ProtoMethods
    {
        public static readonly string StartStopDataListPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\StartStopDataList.prof";

        public static readonly string LoadedPointsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LoadedPoint.prof";
        //public static string LoadedPointsPath;

        public static string DistanceAndDurationPath;

        public static DateTime DurationWriteTime;
        public static DateTime LoadedPointsWriteTime;

        public static DateTime OpenAppDate = DateTime.Now;

        public static void Serialize(object data, string path)
        {
            try
            {
                DateTime lastWriteTime = File.GetLastWriteTime(path); //TODO: OBSŁUGA ZDARZENIA GDY JAKIS UZYTKOWNIK ZAPISAŁ DANE W MOEMENCIE GDY KORZYSTAMY Z APLIKACJI
                if (data == null)
                {
                    return;
                }

                using (var ms = File.Create(path))
                {
                    Serializer.Serialize(ms, data);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static T Deserialize<T>(string path) where T : new()
        {
            try
            {
                using (var ms = File.OpenRead(path))
                {
                    var x = Serializer.Deserialize<T>(ms);
                    return x;
                }
            }
            catch (Exception e)
            {
                var x = File.Exists(path);
                if (!x)
                {
                    File.Create(path);
                }
                return new T();
            }
        }

        public static double? SelectDurationPoint(List<DistanceAndDuration> list, Location startLocation, Location stopLocation)  // TODO: Porównywać po hashu/jednej liczbie
        {
            try
            {
                DistanceAndDuration fineDistanceAndDuration = list.FirstOrDefault(s => s.StartLatt == startLocation.Latitude &&
                    s.StartLong == startLocation.Longitude && s.StopLatt == stopLocation.Latitude && s.StopLong == stopLocation.Longitude);

                return fineDistanceAndDuration?.Duration;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //public static double? SelectDistancePoint(List<DistanceAndDuration> list, string startAdress,
        //    string stopAdress)
        //{
        //    try
        //    {
        //        DistanceAndDuration fineDistanceAndDurationArchive = list.Single(s =>
        //            s.StartAdress == startAdress &&
        //            s.StopAdress == stopAdress);

        //        return fineDistanceAndDurationArchive?.Distance;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}
    }
}