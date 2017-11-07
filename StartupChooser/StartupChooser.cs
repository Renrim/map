using eMapy.BusinessLogic.Services;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using eMapy.DataAccess.DataAccess.ProtoBuf;
using WymianaFTP;

namespace StartupChooser
{
    internal class StartupChooser
    {
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                //var mbs = new ManagementObjectSearcher("Select ProcessorID From Win32_processor");
                //var mbsList = mbs.Get();

                //foreach (ManagementObject mo in mbsList)
                //{
                //    var cpuid = mo["ProcessorID"].ToString();
                //    return;
                //}
                var cpuId = WMI.GetCPUId();

                CheckIfAnotherProccessIsOpen();
                EncryptAppConfig();
                var adress = WriteDataFromArgs(args);
                RunApp(args, adress);
            }
            catch (Exception s)
            {
                MessageBox.Show($"{s.ToString()}");
            }
        }

        private static void CheckIfAnotherProccessIsOpen()
        {
            Process[] Proccesses = Process.GetProcessesByName("StartupChooser");
            if (Proccesses.Length > 1)
            {
                var xs = Proccesses.Min(x => x.StartTime);
                var xss = (from item in Proccesses.Where(x => x.StartTime == xs)
                    select item).First();
                xss.Kill();
            }
        }

        private static string WriteDataFromArgs(string[] args)
        {
            ProtoMethods.DistanceAndDurationPath = args[3] + "DurationAndDistance.prof";
            ProtoMethods.LoadedPointsPath = args[3] + "LoadedPoint.prof";
            PointsServices.UserLogin = args[4];
            ProtoMethods.OpenAppDate = DateTime.Now;
            string adress = string.Empty;
            for (int i = 5; i < args.Length; i++)
            {
                adress += args[i] + " ";
            }
            return adress;
        }

        private static void RunApp(string[] args, string adress)
        {
            eMapy.BusinessLogic.GlobalMethods.FilePathString = args[0];
            eMapy.App app = new eMapy.App();
            PointsServices.StartStopAdress = $"{adress}";
            PointsServices.UserId = Convert.ToInt32(args[2]);
            app.StartupUri = new System.Uri("MainWindow.xaml", System.UriKind.Relative);
            app.InitializeComponent();
            app.Run();
        }

        private static void EncryptAppConfig()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionSection = configFile.GetSection("connectionStrings");

            if (connectionSection != null)
            {
                connectionSection.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                connectionSection.SectionInformation.ForceSave = true;
                configFile.Save(ConfigurationSaveMode.Full);
            }
        }
    }
}