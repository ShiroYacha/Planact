using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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
            // clear
            TimeGridContainer.RowDefinitions.Clear();
            foreach (var item in TimeGridContainer.Children)
            {
                // remove canvas children
                (item as Canvas)?.Children.Clear();

                // remove indirectly
                var subItems = (item as Grid)?.Children;
                if (subItems != null) {
                    foreach (var subitem in subItems)
                    {
                        (subitem as Canvas)?.Children.Clear();
                    }
                }
                subItems?.Clear();
            }
            TimeGridContainer.Children.Clear();

            // setup
            if (Mode == TimeGridMode.Week)
            {
                for (int day = 0; day < 7; ++day)
                {
                    TimeGridContainer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                }

                for (int day = 0; day < 7; ++day)
                {
                    // setups
                    var start = DateTime.Today.AddHours(7).AddDays(-day);
                    var end = DateTime.Today.AddHours(2).AddDays(-day + 1);
                    var totalDuration = end - start;

                    // setup border
                    Border background = new Border
                    {
                        Background = new SolidColorBrush(Color.FromArgb(255, 15, 15, 15)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                        BorderThickness = new Thickness(1),
                    };
                    TimeGridContainer.Children.Add(background);
                    Grid.SetRow(background, day);

                    // setup canvas
                    var canvas = new Canvas();
                    canvas.Width = Width;
                    canvas.Height = Height / 7;
                    TimeGridContainer.Children.Add(canvas);
                    Grid.SetRow(canvas, day);

                    // prepare components
                    var marginHeight = 8;
                    var height = (canvas.Height - marginHeight * 5) / 2;

                    // filter items
                    var filteredItems = Items.Where(i => i.Start > start && i.Start < end).OrderBy(i => i.Start).ToList();

                    // compute icons
                    var iconDict = new Dictionary<int, Image>();
                    var lastIcon = "";
                    for (var i = 0; i < filteredItems.Count; ++i)
                    {
                        var currentIcon = filteredItems[i].Tag;
                        if (lastIcon != currentIcon)
                        {
                            iconDict.Add(i, new Image() { Source = new BitmapImage(new Uri($"ms-appx://Planact.App/Assets/{currentIcon}")) });
                            lastIcon = currentIcon;
                        }
                    }

                    // render items
                    for (var i = 0; i < filteredItems.Count; ++i)
                    {
                        // get item
                        var item = filteredItems[i];
                        var left = (item.Start - start).TotalSeconds / totalDuration.TotalSeconds * canvas.Width;

                        // setup icon
                        if (iconDict.ContainsKey(i))
                        {
                            var icon = iconDict[i];
                            icon.Height = height / 1.5;
                            Canvas.SetLeft(icon, left - icon.ActualWidth / 3);
                            Canvas.SetTop(icon, marginHeight * 2 + height / 2 - icon.Height / 2);
                            canvas.Children.Add(icon);
                        }


                        // setup visual 
                        var visual = item.Visual;
                        visual.Height = height;
                        visual.Width = (item.End - item.Start).TotalSeconds / totalDuration.TotalSeconds * canvas.Width;

                        // add to canvas
                        Canvas.SetLeft(item.Visual, left);
                        Canvas.SetTop(item.Visual, marginHeight * 3 + height);
                        canvas.Children.Add(visual);
                    }
                }
            }
            else if (Mode == TimeGridMode.Month)
            {
                var firstStart = DateTime.Today.Date.AddDays(-(int)DateTime.Today.DayOfYear);

                for (int w = 0; w < 4; ++w)
                {
                    TimeGridContainer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                    // setup container visual 
                    Border background = new Border
                    {
                        Background = new SolidColorBrush(Color.FromArgb(255, 15, 15, 15)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                        BorderThickness = new Thickness(1),
                    };
                    TimeGridContainer.Children.Add(background);
                    Grid.SetRow(background, w);

                    // setup visual 
                    var grid = new Grid();
                    var startOfWeek = firstStart.AddDays(-7 * w);
                    for (int d = 0; d < 7; ++d)
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                        var start = startOfWeek.AddDays(d);
                        var end = start.AddDays(1);

                        var canvas = FillItemsVerticallyInContainer(Items, start, end, new Size(Width / 7, Height / 4));
                        Grid.SetColumn(canvas, d);
                        grid.Children.Add(canvas);
                    }
                    Grid.SetRow(grid, w);
                    TimeGridContainer.Children.Add(grid);
                }
            }
            else if (Mode == TimeGridMode.Year)
            {

                for (int m = 0; m < 12; ++m)
                {
                    TimeGridContainer.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                    var month = DateTime.Today.Month - m;
                    var year = month < 0 ? DateTime.Today.Year - 1 : DateTime.Today.Year;
                    month = month <= 0 ? month + 12 : month;
                    var start = new DateTime(year, month, 1);
                    var end = month < 12 ? new DateTime(year, month + 1, 1).AddDays(-1): new DateTime(year,12, 31);
                    var days = (end - start).TotalDays + 1;

                    // setup container visual 
                    Border background = new Border
                    {
                        Background = new SolidColorBrush(Color.FromArgb(255, 15, 15, 15)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                        BorderThickness = new Thickness(1),
                    };
                    TimeGridContainer.Children.Add(background);
                    Grid.SetRow(background, m);

                    // setup visual 
                    var grid = new Grid();
                    for (int d = 0; d< 31; ++d)
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    }
                    for (int d = 0; d < days; ++d)
                    {
                        var canvas = FillItemsVerticallyInContainer(Items, start.AddDays(d), start.AddDays(d+1), new Size(Width / 31, Height / 12));
                        Grid.SetColumn(canvas, d);
                        grid.Children.Add(canvas);
                    }
                    Grid.SetRow(grid, m);
                    TimeGridContainer.Children.Add(grid);
                }
            }
        }

        private Canvas FillItemsVerticallyInContainer(List<TimelineItem> items, DateTime start, DateTime end, Size size)
        {
            var canvas = new Canvas();
            canvas.Width = size.Width;
            canvas.Height = size.Height;
            var totalDuration = end - start;
            var margin = canvas.Width / 5;

            foreach (var item in items.Where(i => i.Start > start && i.End < end).OrderBy(i => i.Start))
            {
                var top = (item.Start - start).TotalSeconds / totalDuration.TotalSeconds * canvas.Height;
                var height = (item.End - item.Start).TotalSeconds / totalDuration.TotalSeconds * canvas.Height;
                var width = canvas.Width - 2 * margin;
                var left = margin;
                item.Visual.Height = height;
                item.Visual.Width = width;

                Canvas.SetLeft(item.Visual, left);
                Canvas.SetTop(item.Visual, top);
                canvas.Children.Add(item.Visual);
            }

            return canvas;
        }
    }
}
