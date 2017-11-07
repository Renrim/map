using eMapy.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace eMapy.DataAccess.DataAccess.Xml
{
    public class Contractors
    {
        public static readonly string ContractorsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Sage\\ListaKontrahentow.tmp";



        public static void SerializeIdToXML(List<EmapyPoint> parameters, EmapyPoint param, Options options)
        {
           
            if (options != null )
            {
                if (options.ProgressBarAnimation)
                {
                    return;
                }
                options.ProgressBarAnimation = true;
            }

            List<int> idList = new List<int>();
            if (param == null)
            {
                foreach (var item in parameters)
                {
                    idList.Add(item.Id);
                };
            }
            else
            {
                idList.Add(param.Id);
            }

            var ser = new XmlSerializer(typeof(MyRoot));
            var obj = new MyRoot

            {
                Items = new MyListWrapper
                {
                    Items = idList
                }
            };
            //var writer = new System.IO.StreamWriter(ContractorsPath);
            File.Create(ContractorsPath).Close();
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Sage\\ListaKontrahentow.xml");
            using (var sw = new StreamWriter(ContractorsPath, true))
            {
                ser.Serialize(sw, obj);
            }
            System.IO.File.Move(ContractorsPath, (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Sage\\ListaKontrahentow.xml"));

            if (options != null)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(3500);
                    options.ProgressBarAnimation = false;
                    options.WindowStateBool = true;
                });
            }
        }
    }

    [XmlRoot("myroot")]
    public class MyRoot
    {
        [XmlElement("items")]
        public MyListWrapper Items { get; set; }
    }

    public class MyListWrapper
    {
        [XmlElement("item")]
        public List<int> Items { get; set; }
    }
}