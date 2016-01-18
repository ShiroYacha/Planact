﻿using System;
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

namespace Planact.App.Controls
{
    public sealed partial class TimelineHub : UserControl
    {
        public TimelineHub()
        {
            this.InitializeComponent();
            CurrentTimeline.ItemSwipe += CurrentTimeline_ItemSwipe;
            HistoryTimeline.ItemSwipe += HistoryTimeline_ItemSwipe; ;
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
    }
}