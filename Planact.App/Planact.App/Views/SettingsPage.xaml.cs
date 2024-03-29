using Planact.App.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Planact.App.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
        }

        // strongly-typed view models enable x:bind
        public SettingsPageViewModel ViewModel => this.DataContext as SettingsPageViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int index;
            if (int.TryParse(e.Parameter?.ToString(), out index))
                MyPivot.SelectedIndex = index;
        }
    }
}

