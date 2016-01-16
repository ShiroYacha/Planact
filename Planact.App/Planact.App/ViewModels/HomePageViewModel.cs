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
            Shell.Instance.SwitchToQuickButtonConfiguration("Resize");
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
            var configuration = new List<QuadrantExpandingButtonItem>
            {
                new QuadrantExpandingButtonItem()
                {
                    Item = CreateSymbolIcon(Symbol.Home),
                    SubItems = new List<SymbolIcon>
                    {
                        CreateSymbolIcon(Symbol.Help),
                        CreateSymbolIcon(Symbol.HangUp)
                    }
                },
                new QuadrantExpandingButtonItem()
                {
                    Item = CreateSymbolIcon(Symbol.Globe),
                    SubItems = new List<SymbolIcon>
                    {
                        CreateSymbolIcon(Symbol.AllApps),
                        CreateSymbolIcon(Symbol.Admin),
                        CreateSymbolIcon(Symbol.Calculator),
                    }
                },
                new QuadrantExpandingButtonItem()
                {
                    Item = CreateSymbolIcon(Symbol.FontSize),
                    SubItems = new List<SymbolIcon>
                    {
                        CreateSymbolIcon(Symbol.CalendarReply),
                        CreateSymbolIcon(Symbol.CalendarWeek),
                        CreateSymbolIcon(Symbol.Keyboard),
                        CreateSymbolIcon(Symbol.Import),
                        CreateSymbolIcon(Symbol.ImportAll),
                    }
                },
            };

            Shell.Instance.RegisterQuickButtonConfiguration("Default", configuration);
        }

        private void SetupResizeConfiguration()
        {
            var configuration = new List<QuadrantExpandingButtonItem>
            {              
                new QuadrantExpandingButtonItem()
                {
                    Item = CreateSymbolIcon(Symbol.FontSize),
                    SubItems = new List<SymbolIcon>
                    {
                        CreateSymbolIcon(Symbol.CalendarReply),
                        CreateSymbolIcon(Symbol.CalendarWeek),
                        CreateSymbolIcon(Symbol.Keyboard),
                        CreateSymbolIcon(Symbol.Import),
                        CreateSymbolIcon(Symbol.ImportAll),
                    }
                },
            };

            Shell.Instance.RegisterQuickButtonConfiguration("Resize", configuration);
        }

        private SymbolIcon CreateSymbolIcon(Symbol symbol)
        {
            return new SymbolIcon
            {
                Tag = symbol.ToString(),
                Symbol = symbol,
                Width = 20,
                Height = 20,
            };
        }
    }
}

