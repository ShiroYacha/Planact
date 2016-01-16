using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
        private bool dragMode = false;

        public TiledGridView()
        {
            this.InitializeComponent();
        }

        ///// <summary>
        ///// Set column spans depending on group id.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void gve_PreparingContainerForItem(object sender, GridViewEx.PreparingContainerForItemEventArgs e)
        //{
        //    var random = new Random();
        //    e.Element.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.ColumnSpanProperty, random.Next(2,4));
        //}

        private void ChangeItemHighlightingStatus(object target = null)
        {
            // (un)highlight holding item
            foreach (var item in Items)
            {
                // get container
                var container = ItemContainerGenerator.ContainerFromItem(item) as UIElement;

                // if target exist, highlight the target
                if (target != null && item != target)
                {
                    container.Opacity = 0.5;
                }
                // otherwise, set the same opacity
                else
                {
                    container.Opacity = 1;
                }
            }
        }

        private void WrapGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // make sure is dragging
            if(dragMode)
            {
                // highlight new item
                ChangeItemHighlightingStatus((e.OriginalSource as FrameworkElement).DataContext);

                // stop propagation in drag mode
                e.Handled = true;
            }
        }

        private void GridViewEx_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // make sure is dragging amd the tapped control is on blank space
            if (dragMode && sender is TiledGridView)
            {
                // cancel drag model
                dragMode = false;

                // unhighlight
                ChangeItemHighlightingStatus();
            }
        }

        private void GridViewEx_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            // set flag
            dragMode = true;

            // highlight dragging item
            ChangeItemHighlightingStatus(e.Items[0]);
        }
    }
}
