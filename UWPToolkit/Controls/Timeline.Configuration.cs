using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace UWPToolkit.Controls
{
    public class TimelineItem
    {
        public string Name
        {
            get;
            set;
        }

        public DateTime Start
        {
            get;
            set;
        }

        public DateTime End
        {
            get;
            set;
        }

        public FrameworkElement Visual
        {
            get;
            set;
        }
    }
}
