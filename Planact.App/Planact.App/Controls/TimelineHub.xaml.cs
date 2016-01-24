using Planact.App.Converters;
using Planact.Common;
using Planact.DesignTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPToolkit.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Planact.App.Controls
{
    public sealed partial class TimelineHub : UserControl
    {
        private IEnumerable<Task> mpdsItems;

        public TimelineHub()
        {
            this.InitializeComponent();
            CurrentTimeline.ItemSwipe += CurrentTimeline_ItemSwipe;
            HistoryTimeline.ItemSwipe += HistoryTimeline_ItemSwipe;
            Timeline.Start = DateTime.Now;
            Timeline.End = DateTime.Now.AddHours(24);
        }

        private void HistoryTimeline_ItemSwipe(object sender, UWPToolkit.Controls.ItemSwipeEventArgs e)
        {
            CurrentGrid.Visibility = Visibility.Visible;
            HistoryGrid.Visibility = Visibility.Collapsed;
            HistoryTimeline.ResetSwipe();
        }

        private void CurrentTimeline_ItemSwipe(object sender, UWPToolkit.Controls.ItemSwipeEventArgs e)
        {
            double height = CurrentTimeline.ActualHeight;
            HistoryGrid.Visibility = Visibility.Visible;
            CurrentGrid.Visibility = Visibility.Collapsed;
            CurrentTimeline.ResetSwipe();

            // invalidate 
            HistoryTodayTimeline.Start = DateTime.Today;
            HistoryTodayTimeline.End = DateTime.Today.AddDays(1);
            HistoryTodayTimeline.Items = CreateTimelineItemsFor(DateTime.Today.Date, DateTime.Today.AddDays(1));
            HistoryTodayTimeline.SetupItems(height);
        }

        public List<TimelineItem> CurrentTimelineItems
        {
            get
            {
                return CreateRandomTimelineItems(5);
            }
        }

        private List<TimelineItem> CreateTimelineItemsFor(DateTime from, DateTime to)
        {
            var converter = new StringToColorConverter();

            // get executions for today
            var executions = mpdsItems.SelectMany(i => i.Executions).Where(e => e.End <= to && e.Start >= from);
            return executions.Select(e=>
                CreateTimelineItem(Factory.GetImageNameFromName(e.Task.Name), e.Start, e.End, (Color)converter.Convert(e.Task.Group, typeof(Color), null, null))).ToList();
        }

        private List<TimelineItem> CreateRandomTimelineItems(int count)
        {
            var random = new Random(count);
            var items = new List<TimelineItem>();
            var converter = new HexStringToColorConverter();

            for (int i=0; i<count; ++i)
            {
                // generate random start/end
                var start = DateTime.Now.AddHours((0.9 * random.NextDouble() + 0.1) * 24);
                var end = start.AddMinutes((0.5 * random.NextGaussian(2, 1) + 0.5) * 240);

                // create item
                TimelineItem item = CreateTimelineItem(DesignTime.Factory.GetRandomImageName(random),
                    start, end, (Color)converter.Convert(DesignTime.Factory.CreateRandomColor(random), typeof(Color), null, null));

                // add to items
                items.Add(item);
            }

            // sort w.r.t. time
            items.Sort((a, b) => a.Start.CompareTo(b.Start));

            return items;
        }

        private static TimelineItem CreateTimelineItem(string imageName, DateTime start, DateTime end, Color color)
        {
            // create container
            var container = new Grid();
            container.VerticalAlignment = VerticalAlignment.Stretch;
            container.Width = 50;
            container.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            container.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });

            // create image
            var imageContainer = new Grid();
            var imageBackground = new Border();
            imageBackground.Background = new SolidColorBrush(color);
            imageBackground.Opacity = 0.8;
            var iconImage = new Image();
            iconImage.MaxHeight = 30;
            iconImage.Source = new BitmapImage(new Uri($"ms-appx://Planact.App/Assets/{imageName}"));
            imageContainer.Children.Add(imageBackground);
            imageContainer.Children.Add(iconImage);
            container.Children.Add(imageContainer);
            Grid.SetRow(imageContainer, 0);

            // create time label
            var labelContainer = new Grid();
            var labelBackground = new Border();
            labelBackground.Background = imageBackground.Background;
            labelBackground.Opacity = 0.4;
            var timeLabel = new TextBlock();
            timeLabel.FontFamily = new FontFamily("Segoe UI Light");
            timeLabel.FontWeight = FontWeights.Light;
            timeLabel.FontSize = 12;
            timeLabel.Text = start.ToString("HH:mm");
            timeLabel.TextAlignment = TextAlignment.Center;
            labelContainer.Children.Add(labelBackground);
            labelContainer.Children.Add(timeLabel);
            container.Children.Add(labelContainer);
            Grid.SetRow(labelContainer, 1);

            // create item
            var item = new TimelineItem { Visual = container, Start = start, End = end };
            return item;
        }

        private bool loaded = false;

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!loaded)
            {
                // load data
                MpdsAdapter adapter = new MpdsAdapter();
                mpdsItems = await adapter.ImportData();

                // set flag
                loaded = true;
            }
        }
    }
}
