using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planact.DesignTime
{
    public class ObjectiveContribution
    {
        public DateTime Timestamp
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }
    }

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

        public int RowSpan
        {
            get;
            set;
        }

        public int ColumnSpan
        {
            get;
            set;
        }
    }
}
