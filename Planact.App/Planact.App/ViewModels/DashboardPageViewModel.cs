using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Planact.App.ViewModels
{
    public class DashboardPageViewModel : Planact.App.Mvvm.NavigableViewModelBase
    {
        public DashboardPageViewModel()
        {
        }

        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {

        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (suspending)
            {

            }

            await Task.Yield();
        }
    }
}

