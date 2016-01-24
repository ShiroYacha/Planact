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
            TimelineCanvas.Children.Clear();

            // prepare the items
            var validItems = FilterItems();
            var totalHeight = height==0?ActualHeight: height;
            var totalDuration = End - Start;
            DateTime start = Start;
            for (var i = 0; i < validItems.Count; i++)
            {
                // compute position of the item
                var itemBeforeDuration = validItems[i].Start - start;
                var itemBeforeHeight = itemBeforeDuration.TotalSeconds / totalDuration.TotalSeconds * totalHeight;

                // compute the height of the item
                var itemDuration = validItems[i].End - validItems[i].Start;
                var itemHeight = itemDuration.TotalSeconds / totalDuration.TotalSeconds * totalHeight;

                // compute visual 
                var visual = validItems[i].Visual;
                visual.Height = itemHeight;

                // set position
                Canvas.SetLeft(visual, 0);
                Canvas.SetTop(visual, itemBeforeHeight);

                // add to children
                TimelineCanvas.Children.Add(visual);
            }
        }

        private List<TimelineItem> FilterItems()
        {
            return Items.Where(i => i.Start>=Start && i.Start <= End).ToList();
        }

        private void TimelineCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Items.Any() && ActualHeight > 0)
            {
                SetupItems();
            }
        }

        private void TimelineCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            if(Items.Any() && ActualHeight>0)
            {
                SetupItems();
            }
        }
    }
}
