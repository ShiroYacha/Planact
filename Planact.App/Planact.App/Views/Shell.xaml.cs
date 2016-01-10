using System.ComponentModel;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UWPToolkit.Extensions;
using UWPToolkit.Controls;
using System.Collections.Generic;

namespace Planact.App.Views
{
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SplitView
    public sealed partial class Shell : Page, INotifyPropertyChanged
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu { get { return Instance.MyHamburgerMenu; } }

        public Shell(NavigationService navigationService)
        {
            Instance = this;
            InitializeComponent();
            MyHamburgerMenu.NavigationService = navigationService;
        }

        public bool IsBusy { get; set; } = false;
        public string BusyText { get; set; } = "Please wait...";
        public event PropertyChangedEventHandler PropertyChanged;

        public static void SetBusy(bool busy, string text = null)
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                if (busy)
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                else
                    BootStrapper.Current.UpdateShellBackButton();

                Instance.IsBusy = busy;
                Instance.BusyText = text;

                Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(IsBusy)));
                Instance.PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(nameof(BusyText)));
            });
        }

        private void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var dependencyObject = e.OriginalSource as DependencyObject;
            var target = dependencyObject.FindParent<QuadrantExpandingButton>();
            if (target == null)
            {
                QuickButton.CollapseAll();
            }
        }

        public IEnumerable<QuadrantExpandingButtonItem> QuickButtonItems
        {
            get
            {
                return new List<QuadrantExpandingButtonItem>
                        {
                            new QuadrantExpandingButtonItem()
                            {
                                Item = new SymbolIcon {Symbol=Symbol.Home},
                                SubItems = new List<SymbolIcon>
                                {
                                    new SymbolIcon {Symbol=Symbol.Help },
                                    new SymbolIcon {Symbol=Symbol.GoToStart }
                                }
                            },
                            new QuadrantExpandingButtonItem()
                            {
                                Item = new SymbolIcon {Symbol=Symbol.Account},
                            }
                        };
            }
        } 
    }
}

