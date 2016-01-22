using Planact.App.Converters;
using Planact.DesignTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static Planact.Common.MathExtension;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Planact.App.Controls
{
    public sealed partial class ObjectiveTile : UserControl
    {
        #region Bindings

        public static readonly DependencyProperty ObjectiveNameProperty =
    DependencyProperty.Register("ObjectiveName", typeof(string), typeof(ObjectiveTile),
        new PropertyMetadata(null,
            (d, e) =>
            {
                (d as ObjectiveTile).ObjectiveNameTextBlock.Text = e.NewValue as string;
            }
            ));


        public string ObjectiveName
        {
            get { return (string)GetValue(ObjectiveNameProperty); }
            set { SetValue(ObjectiveNameProperty, value); }
        }

        public static readonly DependencyProperty ObjectiveContributionsProperty =
DependencyProperty.Register("ObjectiveContributions", typeof(List<ObjectiveContribution>), typeof(ObjectiveTile),
new PropertyMetadata(new List<ObjectiveContribution>(),
(d, e) =>
{
    var _this = (d as ObjectiveTile);
    _this.StepAreaSeries.ItemsSource = e.NewValue as List<ObjectiveContribution>;
    _this.UpdateTrendAnimation();
    _this.LevelTextBlock.Text = _this.Level.ToString();
    _this.CurrentStreakTextBlock.Text = $"{_this.CurrentStreak} days streak!";
}
));

        public List<ObjectiveContribution> ObjectiveContributions
        {
            get { return (List<ObjectiveContribution>)GetValue(ObjectiveContributionsProperty); }
            set { SetValue(ObjectiveContributionsProperty, value); }
        }

        public static readonly DependencyProperty IconNameProperty =
DependencyProperty.Register("IconName", typeof(string), typeof(ObjectiveTile),
new PropertyMetadata(null,
    (d, e) =>
    {
        (d as ObjectiveTile).IconImage.Source = new BitmapImage(new Uri($"ms-appx://Planact.App/Assets/{e.NewValue as string}")); 
    }
    ));

        public string IconName
        {
            get { return (string)GetValue(IconNameProperty); }
            set { SetValue(IconNameProperty, value); }
        }

        public static readonly DependencyProperty ColorStringProperty =
DependencyProperty.Register("ColorString", typeof(string), typeof(ObjectiveTile),
new PropertyMetadata(null,
(d, e) =>
{
    var converter = new HexStringToColorConverter();
    (d as ObjectiveTile).BackgroundGrid.Background =new SolidColorBrush((Color)converter.Convert(e.NewValue, typeof(Color), null, null));
}
));

        public string ColorString
        {
            get { return (string)GetValue(ColorStringProperty); }
            set { SetValue(ColorStringProperty, value); }
        }

        public int Level => (int)Math.Round(Math.Sqrt(ObjectiveContributions.Sum(c => c.Count)));

        public int CurrentStreak
        {
            get
            {
                // initialize
                int streak = 0;
                DateTime currentTimepoint = DateTime.Today.AddDays(-1); // start from yesterday

                // find the first breaking point
                foreach (var contribution in ObjectiveContributions.OrderByDescending(c => c.Timestamp))
                {
                    // get distance to current time point
                    var delta = (contribution.Timestamp.Date - currentTimepoint).TotalDays;
                    if (delta >= 0)
                    {
                        streak = 1;
                    }
                    else if (delta == -1)
                    {
                        // increase streak
                        streak++;

                        // decrease current time point
                        currentTimepoint = currentTimepoint.AddDays(-1);
                    }
                    else
                    {
                        break;
                    }
                }

                return streak;
            }
        }

        #endregion

        public ObjectiveTile()
        {
            this.InitializeComponent();
        }

        private double ComputeTrend()
        {
            // get latest data
            var latestData = (from d in ObjectiveContributions orderby d.Timestamp descending select d).Take(5);
            var xdata = (from d in latestData select (DateTime.Today.Date - d.Timestamp.Date).TotalDays).ToArray();
            var ydata = (from d in latestData select (double)d.Count).ToArray();

            // compute slope of latest values
            var slope = LeastSquareLinearRegression(xdata,ydata).Item2;

            // return slope's sign
            return Math.Sign(slope);
        }


        private void UpdateTrendAnimation()
        {
            // compute trend of the last
            var trend = ComputeTrend();

            // set storyboard and image source
            Storyboard targetStoryboard;
            string targetSourcePath;
            if (trend >= 0)
            {
                targetStoryboard = Rising;
                targetSourcePath = @"ms-appx://Planact.App/Assets/Arrowhead-Up.png";
            }
            else
            {
                targetStoryboard = Falling;
                targetSourcePath = @"ms-appx://Planact.App/Assets/Arrowhead-Down.png";
            }

            // setup image source
            trendImage.Source = new BitmapImage(new Uri(targetSourcePath));

            // setup storyboard and play
            Storyboard.SetTargetName(targetStoryboard, nameof(trendImage));
            targetStoryboard.RepeatBehavior = new RepeatBehavior(double.MaxValue);
            targetStoryboard.Begin();
        }
    }
}
