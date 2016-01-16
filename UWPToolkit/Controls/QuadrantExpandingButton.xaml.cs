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
            const double innerRingRadius = 120; // depend on the Path
            double outerRingHeight = outerRing.Height;
            double outerRingWidth = outerRing.Width;
            const double outerRingRadius = 210; // depend on the Path

            // setup each items
            var count = Items.Count();
            innerRing.Children.RemoveExceptTypes(typeof(Windows.UI.Xaml.Shapes.Path));
            Items.Each((item, index) => 
            {
                // compute coordinates
                var innerRingItem = item.Item;
                double innerRingTheta = Math.PI * (index + 1) / (2*(count+1));
                double innerRingX = innerRingWidth - innerRingRadius * Math.Cos(innerRingTheta);
                double innerRingY = innerRingHeight - innerRingRadius * Math.Sin(innerRingTheta);

                // add to inner ring canvas
                Canvas.SetLeft(innerRingItem, innerRingX - innerRingItem.Width/2);
                Canvas.SetTop(innerRingItem, innerRingY - innerRingItem.Height/2);
                innerRing.Children.Add(innerRingItem);

                // setup outer ring
                PointerEventHandler blinkAndCollapse = async (s, e) =>
                {
                    // play blink animation
                    await RingButtonPressedHandler(s, e);

                    // collapse
                    CollapseAll();
                };

                var subCount = item.SubItems.Count();
                if (subCount > 0)
                {
                    // create link on inner ring item
                    innerRingItem.PointerPressed += async (s, e) =>
                    {
                        // play blink animation
                        await RingButtonPressedHandler(s, e);

                        // clear outer ring items except the path
                        outerRing.Children.RemoveExceptTypes(typeof(Windows.UI.Xaml.Shapes.Path));

                        // setup outer ring items
                        item.SubItems.Each((subItem, subIndex) =>
                        {
                            // compute coordinates
                            var outerRingItem = subItem;
                            double outerRingTheta = Math.PI * (subIndex + 1) / (2 * (subCount + 1));
                            double outerRingX = outerRingWidth - outerRingRadius * Math.Cos(outerRingTheta);
                            double outerRingY = outerRingHeight - outerRingRadius * Math.Sin(outerRingTheta);

                            // add to inner ring canvas
                            Canvas.SetLeft(outerRingItem, outerRingX - outerRingItem.Width / 2);
                            Canvas.SetTop(outerRingItem, outerRingY - outerRingItem.Height / 2);
                            outerRing.Children.Add(outerRingItem);

                            // play blink animation
                            subItem.PointerPressed += blinkAndCollapse;
                        });

                        // play animation
                        ExpandOuterRing();
                    };
                }
                else
                {
                    innerRingItem.PointerPressed += blinkAndCollapse;
                }
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
                ExpandRootButton();
            }
            else
            {
                // collapse outer
                CollapseOuterRing();

                // collapse root button
                CollapseRootButton();
            }
        }

        public void ExpandRootButton()
        {
            if (!innerRingExpanded)
            {
                // set flag 
                innerRingExpanded = true;

                // expand button
                Expand.Begin();
            }
        }

        public void CollapseRootButton()
        {
            if (innerRingExpanded)
            {
                // set flag 
                innerRingExpanded = false;

                // expand button
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
