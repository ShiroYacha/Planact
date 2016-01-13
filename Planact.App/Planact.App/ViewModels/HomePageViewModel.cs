using Planact.Models;
using Planact.Models.DesignTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Planact.App.ViewModels
{
    public class HomePageViewModel : Planact.App.Mvvm.NavigableViewModelBase
    {
        public HomePageViewModel()
        {
        }

        public ObservableCollection<Objective> DesignTimeObjectives
        {
            get
            {
                return new ObservableCollection<Objective>(DesignTimeObjectiveFactory.CreateRandomObjectives(5));
            }
        }
    }
}

