using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UWPToolkit.Controls
{
    public sealed partial class Timeline : UserControl
    {
        public Timeline()
        {
            InitializeComponent();
        }

        #region Dependency binding
        public List<TimelineItem> Items
        {
            get { return (List<TimelineItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(List<TimelineItem>), typeof(Timeline), new PropertyMetadata(null,
                    (d, e) =>
                    {
                    }
                    ));

        public DateTime Start
        {
            get { return (DateTime)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register(nameof(Start), typeof(DateTime), typeof(Timeline), new PropertyMetadata(DateTime.Today.Date,
                    (d, e) =>
                    {
                        //(d as Timeline).SetupItems();
                    }
                    ));

        public DateTime End
        {
            get { return (DateTime)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register(nameof(End), typeof(DateTime), typeof(Timeline), new PropertyMetadata(DateTime.Today.Date.AddDays(1),
                    (d, e) =>
                    {
                        //(d as Timeline).SetupItems();
                    }
                    ));


        #endregion

        public void SetupItems(double height = 0)
        {
            // initialize
            TimelineGrid.RowDefinitions.Clear();
            TimelineGrid.Children.Clear();

            // prepare the items
            var validItems = FilterItems();
            var totalHeight = height==0?ActualHeight: height;
            var totalDuration = End - Start;
            DateTime start = Start;
            DateTime end;
            for (var i = 0; i < validItems.Count; i++)
            {
                // compute end of the place holder
                end = validItems[i].Start;

                // computer the height of the placeholder
                var placeholderDuration = end - start;
                var placeholderHeight = placeholderDuration.TotalSeconds / totalDuration.TotalSeconds * totalHeight;

                // pass if reverted
                if (placeholderHeight < 0)
                    continue;

                // computer the height of the item
                var itemDuration = validItems[i].End - validItems[i].Start;
                var itemHeight = itemDuration.TotalSeconds / totalDuration.TotalSeconds * totalHeight;


                // setup grid row definition according to the result
                TimelineGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(placeholderHeight) });
                TimelineGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(itemHeight) });

                // put the item in place
                var visual = validItems[i].Visual;
                TimelineGrid.Children.Add(visual);
                Grid.SetRow(visual, i * 2 + 1);

                // set new start as last end
                start = end;
            }

            // add the last placeholder
            TimelineGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100, GridUnitType.Star) });
        }

        private List<TimelineItem> FilterItems()
        {
            return Items.Where(i => i.Start>=Start && i.Start <= End).ToList();
        }

        private void TimelineGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Items.Any() && ActualHeight > 0)
            {
                SetupItems();
            }
        }

        private void TimelineGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if(Items.Any() && ActualHeight>0)
            {
                SetupItems();
            }
        }
    }
}
