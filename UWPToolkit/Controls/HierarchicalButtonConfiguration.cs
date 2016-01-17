using Windows.UI.Xaml;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using System.Windows.Input;

namespace UWPToolkit.Controls
{
    public class HierarchicalButtonConfiguration
    {
        public FrameworkElement ButtonVisual
        {
            get;
            set;
        }

        public ICommand Command
        {
            get;
            set;
        }

        public List<HierarchicalButtonConfiguration> SubButtonConfigurations
        {
            get;
            set;
        } = new List<HierarchicalButtonConfiguration>();
    }
}
