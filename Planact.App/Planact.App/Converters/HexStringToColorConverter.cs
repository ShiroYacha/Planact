using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace Planact.App.Converters
{
    public sealed class HexStringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, string culture)
        {
            var colorString = value as string;
            if (colorString == null)
                return Colors.SteelBlue;
            if (colorString.StartsWith("#"))
                colorString = colorString.Substring(1);
            if (colorString.Length != 6)
                throw new Exception("Color not valid");
            return Color.FromArgb(255, 
                byte.Parse(colorString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), 
                byte.Parse(colorString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), 
                byte.Parse(colorString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
