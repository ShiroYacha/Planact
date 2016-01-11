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

namespace Planact.App.Controls
{
    public sealed partial class StepAreaChartTile : UserControl
    {
        public class SampleData
        {
            public DateTime Date { get; set; }
            public int Count { get; set; }
        }

        public ObservableCollection<SampleData> Data
        {
            get;
            set;
        }


        public StepAreaChartTile()
        {
            this.InitializeComponent();

            DesignTimeData();
        }

        private void DesignTimeData()
        {
            Data = new ObservableCollection<SampleData>();
            var random = new Random(615);
            for (int i = 0; i < 100; ++i)
            {
                Data.Add(new SampleData { Date = DateTime.Today.AddDays(-i), Count = random.Next(0,15) });
            }

        }
    }
}
