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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UWPToolkit.Controls
{
    public enum TimeGridMode
    {
        Week,
        Month,
        Year
    }

    public sealed partial class TimeGrid : UserControl
    {
        public List<TimelineItem> Items
        {
            get { return (List<TimelineItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(List<TimelineItem>), typeof(TimeGrid), new PropertyMetadata(null,
                    (d, e) =>
                    {
                    }
                    ));

        public TimeGridMode Mode
        {
            get { return (TimeGridMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(TimeGridMode), typeof(TimeGrid), new PropertyMetadata(TimeGridMode.Week,
                    (d, e) =>
                    {
                    }
                    ));

        public TimeGrid()
        {
            this.InitializeComponent();
        }

        public void SetupItems()
        {
            if(Mode == TimeGridMode.Week)
            {
                for (int day = 0; day < 7; ++day)
                {
                    TimeGridContainer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                }

                for (int day = 0; day < 7; ++day)
                {
                    // setups
                    var start = DateTime.Today.AddHours(7).AddDays(-day);
                    var end = DateTime.Today.AddHours(7).AddDays(-day+1);
                    var totalDuration = end - start;

                    // setup and add to container
                    var canvas = new Canvas();
                    canvas.Width = Width;
                    canvas.Height = Height / 7;
                    Grid.SetRow(canvas, day);
                    TimeGridContainer.Children.Add(canvas);

                    // render items
                    foreach(var item in Items.Where(i=>i.Start> start && i.Start< end))
                    {
                        // setup visual 
                        var visual = item.Visual;
                        visual.Height = canvas.Height;
                        visual.Width = (item.End - item.Start).TotalSeconds/ totalDuration.TotalSeconds * canvas.Width;

                        // add to canvas
                        Canvas.SetLeft(item.Visual, (item.Start - start).TotalSeconds / totalDuration.TotalSeconds * canvas.Width);
                        canvas.Children.Add(visual);
                    }
                }
            }
        }

    }
}
