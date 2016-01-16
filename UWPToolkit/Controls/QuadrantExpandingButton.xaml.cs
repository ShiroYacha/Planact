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
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI;

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
            DependencyProperty.Register("Items", typeof(IEnumerable<QuadrantExpandingButtonItem>), typeof(QuadrantExpandingButton),
                new PropertyMetadata(new List<QuadrantExpandingButtonItem>(),
                    (d, e) =>
                    {
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

        const double innerRingRadius = 120; // depend on the Path
        const double innerRingRibbonRadius = 23;
        const double outerRingRadius = 210; // depend on the Path
        const double outerRingRibbonRadius = 35;

        public void SetupItems()
        {
            // prepare handler resource
            PointerEventHandler blinkAndCollapse = async (s, e) =>
            {
                // play blink animation
                await RingButtonPressedHandler(s, e);

                // collapse
                CollapseAll();
            };

            // get canvas configuration
            double innerRingHeight = innerRing.Height;
            double innerRingWidth = innerRing.Width;
            double outerRingHeight = outerRing.Height;
            double outerRingWidth = outerRing.Width;

            // setup each items
            var count = Items.Count();
            innerRing.Children.RemoveExceptTypes(typeof(Path));
            Items.Each((item, index) =>
            {
                // compute coordinates
                var innerRingItem = item.Item;
                double innerRingTheta = ComputeThetaOfQuadrantPart(index, count);
                double innerRingX = innerRingWidth - innerRingRadius * Math.Cos(innerRingTheta);
                double innerRingY = innerRingHeight - innerRingRadius * Math.Sin(innerRingTheta);

                // wrap with container to expand hitbox
                var innerRingItemContainer = WrapItemWithContainer(innerRingItem, innerRingRibbonRadius);

                // add to inner ring canvas
                Canvas.SetLeft(innerRingItemContainer, innerRingX - innerRingRibbonRadius);
                Canvas.SetTop(innerRingItemContainer, innerRingY - innerRingRibbonRadius);
                innerRing.Children.Add(innerRingItemContainer);

                // setup outer ring
                var subCount = item.SubItems.Count();
                if (subCount > 0)
                {
                    // wrap sub items
                    var wrapedSubItems = (from subItem in item.SubItems select WrapItemWithContainer(subItem, outerRingRibbonRadius)).ToList();

                    // create link on inner ring item
                    innerRingItemContainer.PointerPressed += async (s, e) =>
                    {
                        // play blink animation
                        await RingButtonPressedHandler(s, e);

                        // clear outer ring items except the path
                        outerRing.Children.RemoveExceptTypes(typeof(Path));

                        // setup outer ring items
                        wrapedSubItems.Each((wrapedSubItem, subIndex) =>
                        {
                            // compute coordinates
                            double outerRingTheta = ComputeThetaOfQuadrantPart(subIndex, subCount);
                            double outerRingX = outerRingWidth - outerRingRadius * Math.Cos(outerRingTheta);
                            double outerRingY = outerRingHeight - outerRingRadius * Math.Sin(outerRingTheta);

                            // add to inner ring canvas
                            Canvas.SetLeft(wrapedSubItem, outerRingX - outerRingRibbonRadius);
                            Canvas.SetTop(wrapedSubItem, outerRingY - outerRingRibbonRadius);
                            outerRing.Children.Add(wrapedSubItem);

                            // play blink animation
                            wrapedSubItem.PointerPressed += blinkAndCollapse;
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

        private static double ComputeThetaOfQuadrantPart(int index, int count)
        {
            return Math.PI * 0.6 * (index + 1) / (count + 1) - Math.PI * 0.05;
        }

        private Grid WrapItemWithContainer(FrameworkElement item, double radius)
        {
            var container = new Grid()
            {
                Name = item.Tag as string,
                RenderTransform = new CompositeTransform(),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };
            var hitbox = new Ellipse()
            {
                Width = radius * 2,
                Height = radius * 2,
                Fill = new SolidColorBrush(Colors.Tomato),
            };
            container.Children.Add(item);
            container.Children.Add(hitbox);
            return container;
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

                // if there is only one button in the inner ring, expand the outer ring automatically if any items are in there
                if(Items?.Count()==1 && Items.ElementAtOrDefault(0)?.SubItems?.Count()>0)
                {

                }
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
