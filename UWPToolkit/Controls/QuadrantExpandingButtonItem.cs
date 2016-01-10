using Windows.UI.Xaml;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace UWPToolkit.Controls
{
    public class QuadrantExpandingButtonItem:DependencyObject
    {
        #region Dependency binding

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(QuadrantExpandingButtonItem), typeof(FrameworkElement),
                new PropertyMetadata(null,
                    (d, e) =>
                    {
                        //BindableParameter param = (BindableParameter)d;
                        ////set the ConverterParameterValue before calling invalidate because the invalidate uses that value to sett the converter paramter
                        //param.ConverterParameterValue = e.NewValue;
                        ////update the converter parameter 
                        //InvalidateBinding(param);
                    }
                    ));

        public static readonly DependencyProperty SubItemsProperty =
            DependencyProperty.Register("SubItems", typeof(QuadrantExpandingButtonItem), typeof(IEnumerable<FrameworkElement>),
                new PropertyMetadata(new List<FrameworkElement>(),
                    (d, e) =>
                    {
                        //BindableParameter param = (BindableParameter)d;
                        ////set the ConverterParameterValue before calling invalidate because the invalidate uses that value to sett the converter paramter
                        //param.ConverterParameterValue = e.NewValue;
                        ////update the converter parameter 
                        //InvalidateBinding(param);
                    }
                    ));

        public FrameworkElement Item
        {
            get { return (FrameworkElement)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public IEnumerable<FrameworkElement> SubItems
        {
            get { return (IEnumerable<FrameworkElement>)GetValue(SubItemsProperty); }
            set { SetValue(SubItemsProperty, value); }
        }

        #endregion
    }
}
