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
    public sealed partial class TiledGridView : GridViewEx.GridViewEx
    {
        public TiledGridView()
        {
            this.InitializeComponent();
        }

        // creates new group in the data source, if end-user drags item to the new group placeholder
        private void gve_BeforeDrop(object sender, GridViewEx.BeforeDropItemsEventArgs e)
        {
           
        }

        // removes empty groups (except the last one)
        private void gve_Drop(object sender, DragEventArgs e)
        {
          
        }

        /// <summary>
        /// Set column spans depending on group id.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gve_PreparingContainerForItem(object sender, GridViewEx.PreparingContainerForItemEventArgs e)
        {
            var random = new Random();
            e.Element.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.ColumnSpanProperty, random.Next(2,4));
        }
    }
}
