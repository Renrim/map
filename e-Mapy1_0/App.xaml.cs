using System;
using System.Windows;

namespace eMapy1
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
                if (e.Args.Length > 0)
                {
                    //GlobalMethods.FilePathString = e.Args[0];
                }
            }
            catch (Exception)
            {
            }
        }
    }
}