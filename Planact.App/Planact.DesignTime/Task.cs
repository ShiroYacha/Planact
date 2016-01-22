using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planact.DesignTime
{
    public class Execution
    {
        public DateTime Start
        {
            get;
            set;
        }

        public TimeSpan Duration
        {
            get;
            set;
        }

        public DateTime End
        {
            get
            {
                return Start + Duration;
            }
        }
    }

    public class Task
    {
        public string Name
        {
            get;
            set;
        }

        public DateTime Start
        {
            get;
            set;
        }

        public DateTime End
        {
            get;
            set;
        }

        public string Group
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public IEnumerable<Execution> Executions
        {
            get;
            set;
        }  
    }
}
