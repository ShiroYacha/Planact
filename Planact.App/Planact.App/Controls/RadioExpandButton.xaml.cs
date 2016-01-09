using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Planact.App.Controls
{
    public sealed partial class RadioExpandButton : UserControl
    {
        public RadioExpandButton()
        {
            this.InitializeComponent();
        }

        private bool innerRingExpanded = false;
        private bool outerRingExpanded = false;

        public void ToggleRootButtonStatus()
        {
            // toggle button status
            innerRingExpanded = !innerRingExpanded;

            // run animation
            if (innerRingExpanded)
            {
                // expand button
                Expand.Begin();
            }
            else
            {
                // collapse outer
                CollapseOuterRing();

                // collapse button
                Collapse.Begin();
            }
        }

        public void CollapseOuterRing()
        {
            if (outerRingExpanded)
            {
                // set flag
                outerRingExpanded = false;

                // start animation
                CollapseOuter.Begin();
            }
        }

        public void ExpandOuterRing()
        {
            if (!outerRingExpanded)
            {
                // set flag 
                outerRingExpanded = true;

                // start animation
                ExpandOuter.Begin();
            }
        }
    }
}
