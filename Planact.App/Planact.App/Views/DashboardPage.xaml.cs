using Planact.App.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Planact.App.Views
{
    public sealed partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
        }

        // strongly-typed view models enable x:bind
        public DashboardPageViewModel ViewModel => this.DataContext as DashboardPageViewModel;
    }
}
