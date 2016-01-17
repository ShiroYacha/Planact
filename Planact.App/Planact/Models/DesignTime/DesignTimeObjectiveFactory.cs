﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planact.Common;

namespace Planact.Models.DesignTime
{
    public static class DesignTimeObjectiveFactory
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
            "Icon.metrop",
            "Leaf-03.png",
            "Muscle.png",
            "Palette-01.png",
            "Weight.png"
        };

        public static IEnumerable<Objective> CreateRandomObjectives(int count)
        {
            // initialize
            var objectives = new List<Objective>();

            // create objectives
            for(int i=0; i< count; i++)
            {
                objectives.Add(CreateRandomObjective(i+1));
            }

            return objectives;
        }

        public static Objective CreateRandomObjective(int index)
        {
            // generate name
            var name = $"Objective {index}";

            // generate contribution
            var random = new Random(index);
            var contributionCount = random.Next(10, 100);
            var contributions = new List<ObjectiveContribution>();
            var mean = 3;
            var std = 5;
            for (int i=0; i<contributionCount; i++)
            {
                contributions.Add(new ObjectiveContribution
                {
                    Timestamp = DateTime.Today.AddDays(i-contributionCount+1),
                    Count = (int)Math.Round(Math.Max(random.NextGaussian(mean, std),0))
                });
            }
            
            // generate icon string
            var iconName = iconList.ElementAt(random.Next(0, iconList.Count-1));

            // generate color string
            var colorString = string.Format("#{0:X6}", random.Next(0x1000000));

            // generate default span
            var defaultSpan = 1; 

            return new Objective {Name = name, Contributions = contributions, IconName = iconName, ColorString = colorString, RowSpan = defaultSpan , ColumnSpan = defaultSpan };
        }
    }
}
