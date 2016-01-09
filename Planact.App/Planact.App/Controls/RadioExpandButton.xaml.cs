using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Planact.App.Common;
using System;
using System.Threading.Tasks;

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

        public void ToggleRootButtonStatus(object sender, PointerRoutedEventArgs e)
        {
            // run animation
            if (!innerRingExpanded)
            {
                ExpandRootButton(sender, e);
            }
            else
            {
                // collapse outer
                CollapseOuterRing(sender,e);

                // collapse root button
                CollapseRootButton(sender, e);
            }
        }

        public async void ExpandRootButton(object sender, PointerRoutedEventArgs e)
        {
            // handle button pressed event
            await RingButtonPressedHandler(sender, e);

            if (!innerRingExpanded)
            {
                // set flag 
                innerRingExpanded = true;

                // expand button
                Expand.Begin();
            }
        }

        public async void CollapseRootButton(object sender, PointerRoutedEventArgs e)
        {
            // handle button pressed event
            await RingButtonPressedHandler(sender, e);

            if (innerRingExpanded)
            {
                // set flag 
                innerRingExpanded = false;

                // expand button
                Collapse.Begin();
            }
        }
        
        public async void CollapseOuterRing(object sender, PointerRoutedEventArgs e)
        {
            // handle button pressed event
            await RingButtonPressedHandler(sender, e);

            if (outerRingExpanded)
            {
                // set flag
                outerRingExpanded = false;

                // start animation
                CollapseOuter.Begin();
            }
        }

        public async void ExpandOuterRing(object sender, PointerRoutedEventArgs e)
        {
            // handle button pressed event
            await RingButtonPressedHandler(sender, e);

            if (!outerRingExpanded)
            {
                // set flag 
                outerRingExpanded = true;

                // start animation
                ExpandOuter.Begin();
            }
        }

        private async Task RingButtonPressedHandler(object sender, PointerRoutedEventArgs e)
        {
            if (!e.Handled)
            {
                // get target name
                DependencyObject dpobj = sender as DependencyObject;
                string name = dpobj.GetValue(NameProperty) as string;

                // set target name
                IconBlink.Stop();
                Storyboard.SetTargetName(IconBlink, name);

                // animate icon blick effect
                await IconBlink.BeginAsync();

                // set flag
                e.Handled = true;
            }
        }
    }
}
