using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
//using DataAccess.DataAccess.AzureAccess;
//using DataModel;
using eMapy.BusinessLogic.Services;
using eMapy.DataAccess.DataAccess.ProtoBuf;
using eMapy.DTOConverters;
using eMapy.Managers;
using eMapy.Models;
using eMapy.Models.Licencing;
using eMapy.Utils;
using WymianaFTP;

namespace eMapy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                string[] args = Environment.GetCommandLineArgs(); // Pierwszy argument to odwołanie do exe





                KillAnotherProccess();
                //dbAccess.CreateLicenceANdKey();
         
               


                //EncryptAppFile();

                LoadDataFromArgs(args);



            }
            catch (Exception es)
            {
                es.ToString();
            }
        }


        private static void EncryptAppFile()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionSection = configFile.GetSection("connectionStrings");

            if (connectionSection != null)
            {
                connectionSection.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                connectionSection.SectionInformation.ForceSave = true;
                configFile.Save(ConfigurationSaveMode.Modified);
            }
        }

      

        private static void KillAnotherProccess()
        {
            Process[] Proccesses = Process.GetProcessesByName("eMapy");
            if (Proccesses.Length > 1)
            {
                var xs = Proccesses.Min(x => x.StartTime);
                var xss = (from item in Proccesses.Where(x => x.StartTime == xs)
                           select item).First();
                xss.Kill();
            }
        }

        private static void LoadDataFromArgs(string[] args)
        {

            ProtoMethods.DistanceAndDurationPath = args[4] + "DurationAndDistance.prof";
            //ProtoMethods.LoadedPointsPath = args[4] + "LoadedPoint.prof";
            PointsServices.UserLogin = args[5];
            LicenceManager.UserLogin = args[5];
            LicenceManager.Nip = args[6];
            LicenceManager.SymfoniaSerial = args[7];
            ProtoMethods.OpenAppDate = DateTime.Now;
            string adress = string.Empty;
            for (int i = 8; i < args.Length; i++)
            {
                adress += args[i] + " ";
            }
            PointsServices.StartStopAdress = $"{adress}";
            PointsServices.UserId = Convert.ToInt32(args[3]);
            eMapy.BusinessLogic.GlobalMethods.FilePathString = args[1];
        }
    }
}