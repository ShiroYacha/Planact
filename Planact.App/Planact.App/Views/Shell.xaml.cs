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
using System.Runtime.CompilerServices;

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Quick buttons

        private void Grid_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var dependencyObject = e.OriginalSource as DependencyObject;
            var target = dependencyObject.FindParent<QuadrantExpandingButton>();
            if (target == null)
            {
                QuickButton.CollapseAll();
            }
        }

        private Dictionary<string, IEnumerable<QuadrantExpandingButtonItem>> quickButtonConfigurations = new Dictionary<string, IEnumerable<QuadrantExpandingButtonItem>>();
        private IEnumerable<QuadrantExpandingButtonItem> activeQuickButtonItems = new List<QuadrantExpandingButtonItem>();
        public IEnumerable<QuadrantExpandingButtonItem> QuickButtonItems
        {
            get
            {
                return activeQuickButtonItems;
            }
        } 

        public void RegisterQuickButtonConfiguration(string key, IEnumerable<QuadrantExpandingButtonItem> buttons)
        {
            quickButtonConfigurations.Add(key, buttons);
        }

        public void SwitchToQuickButtonConfiguration(string key, bool expand = false)
        {
            if(quickButtonConfigurations.ContainsKey(key))
            {
                // set active configuration
                activeQuickButtonItems = quickButtonConfigurations[key];

                // invalidate binding
                OnPropertyChanged("QuickButtonItems");

                // expand if needed
                if(expand)
                {
                    QuickButton.ExpandRootButton();
                }
            }
        }

        #endregion
    }
}

