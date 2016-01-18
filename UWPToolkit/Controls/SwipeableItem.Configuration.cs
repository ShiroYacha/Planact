using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPToolkit.Controls
{
    public enum SwipeListDirection
    {
        Left, Right, Top, Buttom, None
    }

    public enum SwipeListBehavior
    {
        Expand, Collapse, Disabled
    }

    public class ItemSwipeEventArgs : EventArgs
    {
        public object SwipedItem { get; private set; }

        public SwipeListDirection Direction { get; private set; }

        public ItemSwipeEventArgs(object item, SwipeListDirection direction)
        {
            SwipedItem = item;
            Direction = direction;
        }
    }

    public delegate void ItemSwipeEventHandler(object sender, ItemSwipeEventArgs e);
}
