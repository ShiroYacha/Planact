using Planact.App.Converters;
using Planact.Common;
using Planact.DesignTime;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            HistoryTimelineHeader.ItemSwipe += HistoryTimelineHeader_ItemSwipe;
            Timeline.Start = DateTime.Now;
            Timeline.End = DateTime.Today.AddDays(1);
        }

        private TimeGrid historyGrid;

        private int state = 0;

        private void HistoryTimelineHeader_ItemSwipe(object sender, ItemSwipeEventArgs e)
        {
            if(e.Direction==SwipeListDirection.Right)
            {
                // update state
                state++;
                if(state==3)
                {
                    HistoryTimelineHeader.RightOrButtomBehavior = SwipeListBehavior.Disabled;
                }
                else
                {
                    HistoryTimelineHeader.RightOrButtomBehavior = SwipeListBehavior.Expand;
                }


                if (historyGrid==null)
                {
                    // create time grid
                    historyGrid = new TimeGrid();

                    // set as content
                    HistoryTimelineContent.Children.Clear();
                    HistoryTimelineContent.Children.Add(historyGrid);
                    historyGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                    historyGrid.VerticalAlignment = VerticalAlignment.Stretch;
                }

                // setup content
                SetupHistoryGrid(state);
                SetupHistoryBarContent(state);
            }
            else
            {
                // update state
                state--;
                if (state == 0)
                {
                    HistoryTimelineHeader.LeftOrTopBehavior = SwipeListBehavior.Disabled;
                }
                else
                {
                    HistoryTimelineHeader.LeftOrTopBehavior = SwipeListBehavior.Expand;
                }

                // setup content
                SetupHistoryGrid(state);
                SetupHistoryBarContent(state);
            }

            HistoryTimelineHeader.ResetSwipe();
        }

        private void SetupHistoryBarContent(int state)
        {
            if(state==0)
            {
                HistoryTodayTimeline.Visibility = Visibility.Visible;
                HistoryTodayBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                var codes = new List<string>();

                if(state==1)
                {
                    for(var i=0; i<7; ++i)
                    {
                        codes.Add(DateTime.Today.AddDays(-i).DayOfWeek.ToString().Substring(0, 3));
                    }
                }
                else if(state==2)
                {
                    DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                    DateTime date1 = new DateTime(2011, 1, 1);
                    Calendar cal = dfi.Calendar;
                    var currentWeekNum = cal.GetWeekOfYear(DateTime.Today, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                    for (var i = 0; i < 4; ++i)
                    {
                        codes.Add($"W{currentWeekNum-i}");
                    }
                }
                else if(state ==3)
                {
                    for (var i = 0; i < 12; ++i)
                    {
                        var currentMonth = DateTime.Today.Month - i <= 0?12+ DateTime.Today.Month - i: DateTime.Today.Month - i;
                        codes.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(currentMonth));
                    }
                }

                HistoryTodayBar.RowDefinitions.Clear();
                HistoryTodayBar.Children.Clear();

                // set history bar content
                for (var i = 0; i < codes.Count; ++i)
                {
                    HistoryTodayBar.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    TextBlock text = new TextBlock();
                    text.Text = codes[i];
                    text.FontSize = 20;
                    text.FontWeight = FontWeights.ExtraLight;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    Border background = new Border
                    {
                        Background = new SolidColorBrush(Color.FromArgb(255, 10, 10, 10)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                        BorderThickness = new Thickness(1)
                    };
                    HistoryTodayBar.Children.Add(background);
                    HistoryTodayBar.Children.Add(text);
                    Grid.SetRow(background, i);
                    Grid.SetRow(text, i);
                }


                HistoryTodayTimeline.Visibility = Visibility.Collapsed;
                HistoryTodayBar.Visibility = Visibility.Visible;
            }
        }



        private void SetupHistoryGrid(int state)
        {
            if(state==0)
            {
                historyGrid.Visibility = Visibility.Collapsed;
            }
            else 
            {
                historyGrid.Width = ActualWidth - CurrentTimeline.ActualWidth;
                historyGrid.Height = CurrentTimeline.ActualHeight;

                if (state == 1)
                {
                    historyGrid.Mode = TimeGridMode.Week;
                    historyGrid.Items = CreateTimelineItemsFor(DateTime.Today.AddDays(-6), DateTime.Today.AddDays(1));
                }
                else if(state==2)
                {
                    historyGrid.Mode = TimeGridMode.Month;
                    historyGrid.Items = CreateTimelineItemsFor(DateTime.Today.AddDays(-30), DateTime.Today.AddDays(1));
                }
                else if(state ==3)
                {
                    historyGrid.Mode = TimeGridMode.Year;
                    historyGrid.Items = CreateTimelineItemsFor(DateTime.Today.AddDays(-365), DateTime.Today.AddDays(1));
                }

                historyGrid.SetupItems();
            }

        }

        private void HistoryTimeline_ItemSwipe(object sender, UWPToolkit.Controls.ItemSwipeEventArgs e)
        {
            if(e.Direction == SwipeListDirection.Buttom)
            {
                // toggle visibility
                CurrentGrid.Visibility = Visibility.Visible;
                HistoryGrid.Visibility = Visibility.Collapsed;
            }
            HistoryTimeline.ResetSwipe();
        }

        private void CurrentTimeline_ItemSwipe(object sender, UWPToolkit.Controls.ItemSwipeEventArgs e)
        {
            if(e.Direction == SwipeListDirection.Top)
            {
                // toggle visibility
                HistoryGrid.Visibility = Visibility.Visible;
                CurrentGrid.Visibility = Visibility.Collapsed;

                // invalidate 
                HistoryTodayTimeline.Start = DateTime.Today.AddHours(7);
                HistoryTodayTimeline.End = DateTime.Today.AddDays(1);
                HistoryTodayTimeline.Items = CreateTimelineItemsFor(DateTime.Today.Date, DateTime.Today.AddDays(1));
                HistoryTodayTimeline.SetupItems(CurrentTimeline.ActualHeight);
            }
            CurrentTimeline.ResetSwipe();
        }

        public List<TimelineItem> CurrentTimelineItems
        {
            get
            {
                return CreateRandomTimelineItems(3);
            }
        }

        private List<TimelineItem> CreateTimelineItemsFor(DateTime from, DateTime to)
        {
            var converter = new StringToColorConverter();

            // get executions for today
            var executions = mpdsItems.SelectMany(i => i.Executions).Where(e => e.End <= to && e.Start >= from);
            var groups = executions.Select(i => i.Task.Group).Distinct().ToList();
            return executions.Select(e=>
                CreateTimelineItem(Factory.GetNonRepeatedImageNameFromName(e.Task.Group, groups), e.Start, e.End, (Color)converter.Convert(e.Task.Group, typeof(Color), null, null), true)).ToList();
        }

        private List<TimelineItem> CreateRandomTimelineItems(int count)
        {
            var random = new Random(count);
            var items = new List<TimelineItem>();
            var converter = new HexStringToColorConverter();

            TimeSpan residualDuration = Timeline.End - Timeline.Start;

            DateTime start = DateTime.Now;

            for (int i=0; i<count; ++i)
            {
                // generate end
                var end = start.AddMinutes(random.Next(15,30));

                // create item
                TimelineItem item = CreateTimelineItem(DesignTime.Factory.GetRandomImageName(random),
                    start, end, (Color)converter.Convert(DesignTime.Factory.CreateRandomColor(random), typeof(Color), null, null));

                // update start
                start = start.AddMinutes(random.Next(30, 60));

                // add to items
                items.Add(item);
            }

            // sort w.r.t. time
            items.Sort((a, b) => a.Start.CompareTo(b.Start));

            return items;
        }

        private static TimelineItem CreateTimelineItem(string imageName, DateTime start, DateTime end, Color color, bool compactMode = false)
        {
            // create container
            var container = new Grid();
            container.VerticalAlignment = VerticalAlignment.Stretch;
            container.Width = 50;
            container.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            if(!compactMode)
                container.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });

            // create image
            var imageContainer = new Grid();
            var imageBackground = new Border();
            imageBackground.Background = new SolidColorBrush(color);
            imageBackground.Opacity = 0.8;
            var iconImage = new Image();
            iconImage.MaxHeight = 30;
            if(!compactMode)
                iconImage.Source = new BitmapImage(new Uri($"ms-appx://Planact.App/Assets/{imageName}"));
            imageContainer.Children.Add(imageBackground);
            imageContainer.Children.Add(iconImage);
            container.Children.Add(imageContainer);
            Grid.SetRow(imageContainer, 0);

            // create time label
            if (!compactMode)
            {
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
            }

            // create item
            var item = new TimelineItem { Visual = container, Start = start, End = end, Tag = imageName };
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

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(historyGrid!=null)
            {
                historyGrid.Width = HistoryTimelineContent.ActualWidth;
                historyGrid.Height = HistoryTimelineContent.ActualHeight;
                historyGrid.SetupItems();
            }
        }
    }
}
