using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using UWPToolkit.Extensions;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UWPToolkit.Controls
{
    public sealed partial class QuadrantExpandingButton : UserControl
    {
        public QuadrantExpandingButton()
        {
            this.InitializeComponent();
        }

        #region Dependency binding

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(QuadrantExpandingButton), typeof(IEnumerable<QuadrantExpandingButtonItem>),
                new PropertyMetadata(new List<QuadrantExpandingButtonItem>(),
                    (d, e) =>
                    {
                        //BindableParameter param = (BindableParameter)d;
                        ////set the ConverterParameterValue before calling invalidate because the invalidate uses that value to sett the converter paramter
                        //param.ConverterParameterValue = e.NewValue;
                        ////update the converter parameter 
                        //InvalidateBinding(param);
                        (d as QuadrantExpandingButton).SetupItems();
                    }
                    ));

        public IEnumerable<QuadrantExpandingButtonItem> Items
        {
            get { return (IEnumerable<QuadrantExpandingButtonItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        #endregion

        #region Positioning

        public void SetupItems()
        {
            // get canvas configuration
            double innerRingHeight = innerRing.Height;
            double innerRingWidth = innerRing.Width;
            const double radius = 120; // depend on the Path

            // setup each items
            var count = Items.Count();
            Items.Each((item, index) => 
            {
                var innerRingItem = item.Item;
                double theta = Math.PI * (index + 1) / (2*(count+1));
                double x = innerRingWidth - radius * Math.Cos(theta);
                double y = innerRingHeight - radius * Math.Sin(theta);

                Canvas.SetLeft(innerRingItem, x - innerRingItem.ActualWidth/2);
                Canvas.SetTop(innerRingItem, y - innerRingItem.ActualHeight/2);

                innerRing.Children.Add(innerRingItem);
            });
        }  

        #endregion

        #region Animation

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
                CollapseOuterRing(sender, e);

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

        public void CollapseAll()
        {
            // collapse outer ring
            if (outerRingExpanded)
            {
                // set flag 
                outerRingExpanded = false;

                // start animation
                CollapseOuter.Begin();
            }

            // collapse root button
            if (innerRingExpanded)
            {
                // set flag 
                innerRingExpanded = false;

                // start animation
                Collapse.Begin();
            }
        }

        private async Task RingButtonPressedHandler(object sender, PointerRoutedEventArgs e)
        {
            if (!e.Handled && sender != null && e != null)
            {
                // get target name
                DependencyObject dpobj = sender as DependencyObject;
                string name = dpobj.GetValue(NameProperty) as string;

                // skip root icon blink
                if (name != nameof(rootIcon))
                {
                    // set target name
                    IconBlink.Stop();
                    Storyboard.SetTargetName(IconBlink, name);

                    // animate icon blick effect
                    await IconBlink.BeginAsync();
                }


                // set flag
                e.Handled = true;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // run initialize storyboard
            Initialize.Begin();
        }

        #endregion
    }
}
