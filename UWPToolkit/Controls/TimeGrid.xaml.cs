using System;
using System.Collections.Generic;
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
                (item as Canvas)?.Children.Clear();
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
                    var end = DateTime.Today.AddHours(2).AddDays(-day+1);
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
                    var filteredItems = Items.Where(i => i.Start > start && i.Start < end).OrderBy(i=>i.Start).ToList();

                    // compute icons
                    var iconDict = new Dictionary<int, Image>();
                    var lastIcon = "";
                    for (var i=0; i< filteredItems.Count; ++i)
                    {
                        var currentIcon = filteredItems[i].Tag;
                        if(lastIcon!=currentIcon)
                        {
                            iconDict.Add(i, new Image() { Source = new BitmapImage(new Uri($"ms-appx://Planact.App/Assets/{currentIcon}"))});
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
                            icon.Height = height/1.5;
                            Canvas.SetLeft(icon, left - icon.ActualWidth/3);
                            Canvas.SetTop(icon, marginHeight*2+height/2-icon.Height/2);
                            canvas.Children.Add(icon);
                        }


                        // setup visual 
                        var visual = item.Visual;
                        visual.Height = height;
                        visual.Width = (item.End - item.Start).TotalSeconds/ totalDuration.TotalSeconds * canvas.Width;

                        // add to canvas
                        Canvas.SetLeft(item.Visual, left);
                        Canvas.SetTop(item.Visual, marginHeight * 3 + height);
                        canvas.Children.Add(visual);
                    }
                }
            }
        }
    }
}
