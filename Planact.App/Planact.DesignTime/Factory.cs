using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planact.Common;
using Windows.Storage.Streams;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;

namespace Planact.DesignTime
{
    public static class Factory
    {
        private static List<string> iconList = new List<string>
        {
            "Alarm-01.png",
            "Beverage-Coffee-01.png",
            "Book-02.png",
            "Book-04.png",
            "Brush-01.png",
            "Caduceus.png",
            "Camera-05.png",
            "Cloud-Overcast.png",
            "Cloud-Rain.png",
            "Cloud-Snow.png",
            "Cloud-Sun.png",
            "Cloud-Thunder.png",
            "Code-02.png",
            "Exercise-01.png",
            "Finance.png",
            "Heart-02.png",
            "Leaf-03.png",
            "Muscle.png",
            "Palette-01.png",
            "Weight.png"
        };

        public static string GetRandomImageName(Random random)
        {
            return iconList.ElementAt(random.Next(0, iconList.Count - 1));
        }

        public static IEnumerable<Objective> CreateRandomObjectives(int count)
        {
            // initialize
            var objectives = new List<Objective>();

            // create objectives
            for(int i=0; i< count; i++)
            {
                objectives.Add(CreateRandomObjective(i));
            }

            return objectives;
        }

        public static Objective CreateRandomObjective(int index)
        {
            // generate name
            var name = $"Objective {index + 1}";

            // generate contribution
            var random = new Random(index);
            var contributionCount = random.Next(10, 100);
            var contributions = new List<ObjectiveContribution>();
            var mean = 3;
            var std = 5;
            for (int i = 0; i < contributionCount; i++)
            {
                contributions.Add(new ObjectiveContribution
                {
                    Timestamp = DateTime.Today.AddDays(i - contributionCount + 1),
                    Count = (int)Math.Round(Math.Max(random.NextGaussian(mean, std), 0))
                });
            }

            // generate icon string
            var iconName = GetRandomImageName(random);

            // generate color string
            string colorString = CreateRandomColor(random);

            // generate default span
            var defaultRowSpan = 1;
            var defaultColumnSpan = (new int[] { 1, 2, 5 }).Contains(index) ? 2 : 1;

            return new Objective { Name = name, Contributions = contributions, IconName = iconName, ColorString = colorString, RowSpan = defaultRowSpan, ColumnSpan = defaultColumnSpan };
        }

        public static string GetImageNameFromName(string name)
        {
            // get count from string
            var hexString = ConvertStringToHexString(name).Substring(0,3);
            var count = 0;
            foreach(var hexChar in hexString)
            {
                count += Convert.ToInt32(hexChar);
            }

            // get index
            count = count % iconList.Count;

            // return icon name
            return iconList[count];
        }

        public static string CreateRandomColor(Random random)
        {
            return string.Format("#{0:X6}", random.Next(0x1000000));
        }

        private static string ConvertStringToHexString(string target)
        {
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(target, BinaryStringEncoding.Utf8);
            HashAlgorithmProvider hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            IBuffer hashBuffer = hashAlgorithm.HashData(buffer);
            return CryptographicBuffer.EncodeToHexString(hashBuffer);
        }
    }
}
