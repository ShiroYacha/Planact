using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planact.Models
{
    public class Objective
    {
        public string Name
        {
            get;
            set;
        }

        public List<ObjectiveContribution> Contributions
        {
            get;
            set;
        } = new List<ObjectiveContribution>();

        public string IconName
        {
            get;
            set;
        }

        public string ColorString
        {
            get;
            set;
        }
    }
}
