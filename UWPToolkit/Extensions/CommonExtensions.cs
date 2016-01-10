using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPToolkit.Extensions
{
    public static class CommonExtensions
    {
        public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }

        public static void RemoveExceptTypes<T>(this ICollection<T> ic, params Type[] types)
        {
            // get a list of items to remove
            var removalList = new List<T>();
            foreach(var item in ic)
            {
                if (!types.Contains(item.GetType()))
                    removalList.Add(item);
            }

            // remove from collection
            foreach(var item in removalList)
            {
                ic.Remove(item);
            }
        }
    }
}
