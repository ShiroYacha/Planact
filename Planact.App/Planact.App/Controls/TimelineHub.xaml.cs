using Planact.App.Converters;
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
            CurrentTimeline.Visibility = Visibility.Visible;
            HistoryTimeline.Visibility = Visibility.Collapsed;
            HistoryTimeline.ResetSwipe();
        }

        private void CurrentTimeline_ItemSwipe(object sender, UWPToolkit.Controls.ItemSwipeEventArgs e)
        {
            CurrentTimeline.Visibility = Visibility.Collapsed;
            HistoryTimeline.Visibility = Visibility.Visible;
            CurrentTimeline.ResetSwipe();
        }

        public List<TimelineItem> DesignTimeTimelineItems
        {
            get
            {
                return CreateRandomTimelineItems(5);
            }
        }

        private List<TimelineItem> CreateRandomTimelineItems(int count)
        {
            var random = new Random(count);
            var items = new List<TimelineItem>();
            var converter = new HexStringToColorConverter();

            for (int i=0; i<count; ++i)
            {
                // create container
                var container = new Grid();
                container.Height = 40;
                container.Width = 50;
                container.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });
                container.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });

                // create random time
                var start = DateTime.Now.AddHours((0.9 * random.NextDouble() + 0.1) * 24);

                // create image
                var imageContainer = new Grid();
                var imageBackground = new Border();
                imageBackground.Background = new SolidColorBrush((Color)converter.Convert(DesignTime.Factory.CreateRandomColor(random),typeof(Color),null,null));
                imageBackground.Opacity = 0.8;
                var imageName = DesignTime.Factory.GetRandomImageName(random);
                var iconImage = new Image();
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

                // add to items
                items.Add(new TimelineItem { Visual = container, Start = start });
            }

            // sort w.r.t. time
            items.Sort((a, b) => a.Start.CompareTo(b.Start));

            return items;
        }
    }
}
