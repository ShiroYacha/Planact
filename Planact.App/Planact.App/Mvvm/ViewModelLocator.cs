using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Planact.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planact.App.Mvvm
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            // setup server locator
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // register interfaces
            RegisterServices();
            RegisterDataServices();
            RegisterViewModels();
        }

        static private void RegisterServices()
        {
            
        }

        static private void RegisterDataServices()
        {
            // set data providers
            if (GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            {

            }
            else
            {

            }
        }


        static private void RegisterViewModels()
        {
            // set view models
            SimpleIoc.Default.Register<HomePageViewModel>();
            SimpleIoc.Default.Register<SettingsPageViewModel>();
        }

        public HomePageViewModel HomePageViewModel => SimpleIoc.Default.GetInstance<HomePageViewModel>();
        public SettingsPageViewModel SettingsPageViewModel => SimpleIoc.Default.GetInstance<SettingsPageViewModel>();
    }
}
