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

        public TiledGridView()
        {
            this.InitializeComponent();
        }

        #region Dependency binding

        public static readonly DependencyProperty ActivateConfigurationModeProperty =
            DependencyProperty.Register(nameof(ActivateConfigurationMode), typeof(Action<FrameworkElement>), typeof(TiledGridView),
                new PropertyMetadata(null,
                    (d, e) =>
                    {

                    }
                    ));

        public Action<FrameworkElement> ActivateConfigurationMode
        {
            get { return (Action<FrameworkElement>)GetValue(ActivateConfigurationModeProperty); }
            set { SetValue(ActivateConfigurationModeProperty, value); }
        }

        public static readonly DependencyProperty ExitConfigurationModeProperty =
    DependencyProperty.Register("ExitConfigurationMode", typeof(Action), typeof(TiledGridView),
        new PropertyMetadata(null,
            (d, e) =>
            {

            }
            ));

        public Action ExitConfigurationMode
        {
            get { return (Action)GetValue(ExitConfigurationModeProperty); }
            set { SetValue(ExitConfigurationModeProperty, value); }
        }

        #endregion

        ///// <summary>
        ///// Set column spans depending on group id.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        private void gve_PreparingContainerForItem(object sender, GridViewEx.PreparingContainerForItemEventArgs e)
        {
            var random = new Random();
            e.Element.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.ColumnSpanProperty, random.Next(2, 4));
        }

        #region Drag & drop and configuration mode

        private bool configurationMode = false;

        private void ChangeConfigurationModeStatus(FrameworkElement configurationTarget = null)
        {
            var newMode = configurationTarget != null;
            if (configurationMode != newMode)
            {
                // set flag
                configurationMode = newMode;

                // trigger callback if needed
                if (configurationMode)
                {
                    ActivateConfigurationMode?.Invoke(configurationTarget);
                }
                else
                {
                    ExitConfigurationMode?.Invoke();
                }
            }
        }

        private void ChangeItemHighlightingStatus(object highlightTarget = null)
        {
            // (un)highlight holding item
            foreach (var item in Items)
            {
                // get container
                var container = ItemContainerGenerator.ContainerFromItem(item) as UIElement;

                // if target exist, highlight the target
                if (highlightTarget != null && item != highlightTarget)
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
            if (configurationMode)
            {
                var configurationTarget = e.OriginalSource as FrameworkElement;

                // highlight new item
                ChangeItemHighlightingStatus(configurationTarget.DataContext);

                // stop propagation in drag mode
                e.Handled = true;

                // activate
                ActivateConfigurationMode?.Invoke(configurationTarget);
            }
        }

        private void WrapGrid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (!configurationMode)
            {
                var target = e.OriginalSource as FrameworkElement;

                // set flag
                ChangeConfigurationModeStatus(target);

                // highlight dragging item
                ChangeItemHighlightingStatus(target.DataContext);
            }
        }

        private void GridViewEx_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // make sure is dragging amd the tapped control is on blank space
            if (configurationMode && sender is TiledGridView)
            {
                // cancel drag model
                ChangeConfigurationModeStatus();

                // unhighlight
                ChangeItemHighlightingStatus();
            }
        }

        private void GridViewEx_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            // highlight dragging item
            ChangeItemHighlightingStatus(e.Items[0]);
        }

        private void GridViewEx_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            if (!configurationMode)
            {
                // set flag
                ChangeConfigurationModeStatus(args.Items[0] as FrameworkElement);
            }
        }

        #endregion

        #region Resize component

        public void ResizeComponent(FrameworkElement target, int columnSpan, int rowSpan)
        {
            // make sure in configuration mode
            if (configurationMode)
            {
                // get container
                var container = ContainerFromItem(target) as VariableSizedWrapGrid;

                if (container != null)
                {
                    VariableSizedWrapGrid.SetColumnSpan(container, columnSpan);
                    VariableSizedWrapGrid.SetRowSpan(container, rowSpan);
                }
            }
        }

        #endregion
    }
}
