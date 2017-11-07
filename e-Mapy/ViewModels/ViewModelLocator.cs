using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace eMapy.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainWindowViewModel>();
        }

        public MainWindowViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainWindowViewModel>(); }
        }
    }
}