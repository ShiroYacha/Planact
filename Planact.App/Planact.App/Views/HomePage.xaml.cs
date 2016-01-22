using Planact.App.ViewModels;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Planact.Common;
using UWPToolkit.Controls;

namespace Planact.App.Views
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
            ViewModel.ResizeAction = ObjectiveGridView.ResizeComponent;
        }

        // strongly-typed view models enable x:bind
        public HomePageViewModel ViewModel => this.DataContext as HomePageViewModel;


    }
}
