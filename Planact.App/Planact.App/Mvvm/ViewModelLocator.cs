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

            // set data providers
            if (GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            {

            }
            else
            {
 
            }

            // set view models
            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<DetailPageViewModel>();
            SimpleIoc.Default.Register<SettingsPageViewModel>();
        }

        public MainPageViewModel MainPageViewModel => SimpleIoc.Default.GetInstance<MainPageViewModel>();
        public DetailPageViewModel DetailPageViewModel => SimpleIoc.Default.GetInstance<DetailPageViewModel>();
        public SettingsPageViewModel SettingsPageViewModel => SimpleIoc.Default.GetInstance<SettingsPageViewModel>();
    }
}
