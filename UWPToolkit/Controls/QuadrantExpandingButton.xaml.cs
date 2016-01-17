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

        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register(nameof(Configuration), typeof(HierarchicalButtonConfiguration), typeof(QuadrantExpandingButton),
                new PropertyMetadata(null,
                    (d, e) =>
                    {
                        (d as QuadrantExpandingButton).SetupItems();
                    }
                    ));

        public HierarchicalButtonConfiguration Configuration
        {
            get { return (HierarchicalButtonConfiguration)GetValue(ConfigurationProperty); }
            set { SetValue(ConfigurationProperty, value); }
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

        Dictionary<HierarchicalButtonConfiguration, Grid> itemContainerDict = new Dictionary<HierarchicalButtonConfiguration, Grid>();
        PointerEventHandler rootButtonCommand;

        public void SetupItems()
        {
            // initialize 
            ClearPreviousRecords();

            // setup root button 
            rootButtonVisualContainer.Children.Clear();
            rootButtonVisualContainer.Children.Add(Configuration.ButtonVisual);

            // setup command 
            if (Configuration.Command != null)
            {
                rootButtonCommand = (s, e) => Configuration.Command.Execute(null);
                rootButton.PointerPressed += rootButtonCommand;
            }
            else
            {
                rootButton.PointerPressed += ToggleRootButtonStatus;
            }

            // setup each items
            var count = Configuration.SubButtonConfigurations.Count();
            innerRing.Children.RemoveExceptTypes(typeof(Path));
            Configuration.SubButtonConfigurations.Each((item, index) =>
            {
                // compute coordinates
                var innerRingItem = item.ButtonVisual;
                double innerRingTheta = ComputeThetaOfQuadrantPart(index, count);
                double innerRingX = innerRingWidth - innerRingRadius * Math.Cos(innerRingTheta);
                double innerRingY = innerRingHeight - innerRingRadius * Math.Sin(innerRingTheta);

                // wrap with container to expand hitbox
                var innerRingItemContainer = WrapItemWithContainer(item, innerRingRibbonRadius);

                // add to inner ring canvas
                Canvas.SetLeft(innerRingItemContainer, innerRingX - innerRingRibbonRadius);
                Canvas.SetTop(innerRingItemContainer, innerRingY - innerRingRibbonRadius);
                innerRing.Children.Add(innerRingItemContainer);

                // setup outer ring
                if (item.SubButtonConfigurations.Any())
                {
                    // wrap sub items
                    var wrapedSubItems = (from subItem in item.SubButtonConfigurations select WrapItemWithContainer(subItem, outerRingRibbonRadius)).ToList();

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
                    // setup animation
                    innerRingItemContainer.PointerPressed += BlinkAndCollapse;

                    // setup command 
                    if(item.Command!=null)
                    {
                        innerRingItemContainer.PointerPressed += (s,e)=> item.Command.Execute(null);
                    }
                }
            });
        }

        private void ClearPreviousRecords()
        {
            // clear previous results
            foreach(var key in itemContainerDict.Keys)
            {
                // clear children
                itemContainerDict[key].Children.Clear();
            }

            // clear root button 
            rootButton.PointerPressed -= ToggleRootButtonStatus;
            rootButton.PointerPressed -= rootButtonCommand;

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

                // setup command 
                var subItem = itemContainerDict.Single(kvp => kvp.Value == wrapedSubItem).Key;
                if (subItem.Command != null)
                {
                    wrapedSubItem.PointerPressed += (s, e) => subItem.Command.Execute(null);
                }
            });

            // play animation
            ExpandOuterRing();
        }

        private static double ComputeThetaOfQuadrantPart(int index, int count)
        {
            return Math.PI * 0.6 * (index + 1) / (count + 1) - Math.PI * 0.05;
        }

        private Grid WrapItemWithContainer(HierarchicalButtonConfiguration item, double radius)
        {
            // create container
            var container = new Grid()
            {
                RenderTransform = new CompositeTransform(),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            // setup name
            container.GenerateRandomName();

            // create hitbox
            var hitbox = new Ellipse()
            {
                Width = radius * 2,
                Height = radius * 2,
                Fill = new SolidColorBrush(Colors.Transparent),
            };

            // add to container
            container.Children.Add(item.ButtonVisual);
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
                if(Configuration?.SubButtonConfigurations?.Count()==1 && Configuration.SubButtonConfigurations.ElementAtOrDefault(0)?.SubButtonConfigurations?.Count()>0)
                {
                    // setup sub items
                    SetupSubItems(Configuration.SubButtonConfigurations.ElementAtOrDefault(0).SubButtonConfigurations.Select(subItem => itemContainerDict[subItem]));
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
                if (name != nameof(rootButton))
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
