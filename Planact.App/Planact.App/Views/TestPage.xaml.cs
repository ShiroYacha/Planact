using Planact.App.ViewModels;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Planact.Common;
using Planact.Models;
using Planact.Models.DesignTime;
using UWPToolkit.Controls;

namespace Planact.App.Views
{
    public sealed partial class TestPage : Page
    {
        public TestPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
        }

        // strongly-typed view models enable x:bind
        public TestPageViewModel ViewModel => this.DataContext as TestPageViewModel;
    }
}
