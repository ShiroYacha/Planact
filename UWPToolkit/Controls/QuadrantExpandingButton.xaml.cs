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
            // initialize
            this.InitializeComponent();

            // setup fields
            innerRingHeight = innerRing.Height;
            innerRingWidth = innerRing.Width;
            outerRingHeight = outerRing.Height;
            outerRingWidth = outerRing.Width;
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

        // get canvas configuration
        double innerRingHeight;
        double innerRingWidth; 
        double outerRingHeight;
        double outerRingWidth;

        Dictionary<FrameworkElement, Grid> itemContainerDict = new Dictionary<FrameworkElement, Grid>();

        public void SetupItems()
        {
            // initialize 
            ClearPreviousContainers();

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
                if (item.SubItems.Any())
                {
                    // wrap sub items
                    var wrapedSubItems = (from subItem in item.SubItems select WrapItemWithContainer(subItem, outerRingRibbonRadius)).ToList();

                    // create link on inner ring item
                    innerRingItemContainer.PointerPressed += async (s, e) =>
                    {
                        // play blink animation
                        await RingButtonPressedHandler(s, e);

                        // setup sub items
                        SetupSubItems(wrapedSubItems);
                    };
                }
                else
                {
                    innerRingItem.PointerPressed += BlinkAndCollapse;
                }
            });
        }

        private void ClearPreviousContainers()
        {
            // clear childs
            foreach(var key in itemContainerDict.Keys)
            {
                itemContainerDict[key].Children.Clear();
            }

            // clear container dictionary
            itemContainerDict.Clear();
        }

        private void SetupSubItems(IEnumerable<Grid> wrapedSubItems)
        {
            // clear outer ring items except the path
            outerRing.Children.RemoveExceptTypes(typeof(Path));

            // setup outer ring items
            wrapedSubItems.Each((wrapedSubItem, subIndex) =>
            {
                // compute coordinates
                double outerRingTheta = ComputeThetaOfQuadrantPart(subIndex, wrapedSubItems.Count());
                double outerRingX = outerRingWidth - outerRingRadius * Math.Cos(outerRingTheta);
                double outerRingY = outerRingHeight - outerRingRadius * Math.Sin(outerRingTheta);

                // add to inner ring canvas
                Canvas.SetLeft(wrapedSubItem, outerRingX - outerRingRibbonRadius);
                Canvas.SetTop(wrapedSubItem, outerRingY - outerRingRibbonRadius);
                outerRing.Children.Add(wrapedSubItem);

                // play blink animation
                wrapedSubItem.PointerPressed += BlinkAndCollapse;
            });

            // play animation
            ExpandOuterRing();
        }

        private static double ComputeThetaOfQuadrantPart(int index, int count)
        {
            return Math.PI * 0.6 * (index + 1) / (count + 1) - Math.PI * 0.05;
        }

        private Grid WrapItemWithContainer(FrameworkElement item, double radius)
        {
            // create container
            var container = new Grid()
            {
                Name = item.Tag as string,
                RenderTransform = new CompositeTransform(),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            // create hitbox
            var hitbox = new Ellipse()
            {
                Width = radius * 2,
                Height = radius * 2,
                Fill = new SolidColorBrush(Colors.Transparent),
            };

            // add to container
            container.Children.Add(item);
            container.Children.Add(hitbox);

            // add to dictionary
            itemContainerDict.Add(item, container);

            // return container
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
                    // setup sub items
                    SetupSubItems(Items.ElementAtOrDefault(0).SubItems.Select(subItem => itemContainerDict[subItem]));
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

        private async void BlinkAndCollapse(object sender, PointerRoutedEventArgs e)
        {
            // play blink animation
            await RingButtonPressedHandler(sender, e);

            // collapse
            CollapseAll();
        }

        #endregion
    }
}
