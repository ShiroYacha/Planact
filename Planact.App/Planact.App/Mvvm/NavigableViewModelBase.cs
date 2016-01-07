using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace Planact.App.Mvvm
{
    public abstract class NavigableViewModelBase : ViewModelBase, INavigable
    {
        #region Binding properties

        public string ViewName
        {
            get;
            set;
        } 

        #endregion

        #region INavigable implementations

        public virtual void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state) { /* nothing by default */ }
        public virtual async Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            await Task.CompletedTask;
        }
        public virtual void OnNavigatingFrom(NavigatingEventArgs args) { /* nothing by default */ }

        [JsonIgnore]
        public INavigationService NavigationService { get; set; }
        [JsonIgnore]
        public IDispatcherWrapper Dispatcher { get; set; }
        [JsonIgnore]
        public IStateItems SessionState { get; set; } 

        #endregion
    }
}
