using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace Planact.App.Converters
{
    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return ColorHelper.FromArgb(255, 63, 78, 149);
            }
            else
            {
                var hexString = GenerateSHA1HexStringFromString(value as string);
                return CreateColorFromHexString(hexString);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private string GenerateSHA1HexStringFromString(string rawString)
        {
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(rawString, BinaryStringEncoding.Utf8);
            HashAlgorithmProvider hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            IBuffer hashBuffer = hashAlgorithm.HashData(buffer);
            return CryptographicBuffer.EncodeToHexString(hashBuffer);
        }

        private Color CreateColorFromHexString(string hexString)
        {
            var hexColorString = hexString.Substring(0, 6);
            return ColorHelper.FromArgb(255,
                byte.Parse(hexColorString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hexColorString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hexColorString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
        }
    }
}
