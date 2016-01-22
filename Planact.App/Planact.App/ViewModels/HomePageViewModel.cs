using GalaSoft.MvvmLight.Command;
using Planact.App.Views;
using Planact.Models;
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

        public Action<object, int,int> ResizeAction
        {
            get;
            set;
        }
        private object selectedConfigurationTarget;


        public ObservableCollection<Objective> DesignTimeObjectives
        {
            get
            {
                return new ObservableCollection<Objective>(DesignTime.Factory.CreateRandomObjectives(6));
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

        public Action<object> EnterConfigurationModeAction
        {
            get
            {
                return new Action<object>(EnterConfigurationMode);
            }
        }

        public void EnterConfigurationMode(object element)
        {
            selectedConfigurationTarget = element;
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
            selectedConfigurationTarget = null;
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
                        ButtonVisual = CreateSymbolIcon(Symbol.FullScreen),
                        Command = new RelayCommand(()=>ResizeAction(selectedConfigurationTarget,2,1))
                        
                    },
                    new HierarchicalButtonConfiguration
                    {
                        ButtonVisual = CreateSymbolIcon(Symbol.BackToWindow),
                        Command = new RelayCommand(()=>ResizeAction(selectedConfigurationTarget,1,1))
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

