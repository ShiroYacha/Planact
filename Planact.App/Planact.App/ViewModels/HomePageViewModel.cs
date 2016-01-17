using GalaSoft.MvvmLight.Command;
using Planact.App.Views;
using Planact.Models;
using Planact.Models.DesignTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using UWPToolkit.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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

        public Shell Shell
        {
            get
            {
                return Shell.Instance;
            }
        }

        private ICommand loadedCommand;
        public ICommand LoadedCommand
        {
            get
            {
                return loadedCommand ?? (loadedCommand = new RelayCommand(Loaded));
            }
        }

        private void Loaded()
        {
            SetupDefaultConfiguration();
            SetupResizeConfiguration();
            Shell.Instance.SwitchToQuickButtonConfiguration("Default");
        }

        public Action<FrameworkElement> EnterConfigurationModeAction
        {
            get
            {
                return new Action<FrameworkElement>(EnterConfigurationMode);
            }
        }

        public void EnterConfigurationMode(FrameworkElement element)
        {
            Shell.Instance.SwitchToQuickButtonConfiguration("Resize",true);
        }

        public Action ExitConfigurationModeAction
        {
            get
            {
                return new Action(ExitConfigurationMode);
            }
        }

        public void ExitConfigurationMode()
        {
            Shell.Instance.SwitchToQuickButtonConfiguration("Default");
        }

        private void SetupDefaultConfiguration()
        {
            var configuration = new HierarchicalButtonConfiguration
            {
                ButtonVisual = CreateSymbolIcon(Symbol.Add, true),
                SubButtonConfigurations = new List<HierarchicalButtonConfiguration>
                {
                    new HierarchicalButtonConfiguration
                    {
                        ButtonVisual = CreateSymbolIcon(Symbol.Bookmarks)
                    },
                    new HierarchicalButtonConfiguration
                    {
                        ButtonVisual = CreateSymbolIcon(Symbol.Flag)
                    },
                    new HierarchicalButtonConfiguration
                    {
                        ButtonVisual = CreateSymbolIcon(Symbol.Edit)
                    }
                }
            };

            Shell.Instance.RegisterQuickButtonConfiguration("Default", configuration);
        }

        private void SetupResizeConfiguration()
        {
            var configuration = new HierarchicalButtonConfiguration
            {
                ButtonVisual = CreateSymbolIcon(Symbol.Setting, true),
                SubButtonConfigurations = new List<HierarchicalButtonConfiguration>
                {
                    new HierarchicalButtonConfiguration
                    {
                        ButtonVisual = CreateSymbolIcon(Symbol.FullScreen)
                    },
                    new HierarchicalButtonConfiguration
                    {
                        ButtonVisual = CreateSymbolIcon(Symbol.BackToWindow)
                    }
                }
            };

            Shell.Instance.RegisterQuickButtonConfiguration("Resize", configuration);
        }

        private SymbolIcon CreateSymbolIcon(Symbol symbol, bool isRoot = false)
        {
            var icon = new SymbolIcon
            {
                Symbol = symbol,
                Width = 20,
                Height = 20,
            };

            if(isRoot)
            {
                icon.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
                icon.RenderTransform = new CompositeTransform { ScaleX = 2, ScaleY =2 };
            }

            return icon;
        }
    }
}

