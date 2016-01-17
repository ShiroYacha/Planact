using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace UWPToolkit.Extensions
{
    public static class UIExtension
    {
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            // return null if nothing
            if (child == null)
                return null;

            // get parent of target
            var parent = VisualTreeHelper.GetParent(child);

            // recursive search
            if (parent is T)
                return parent as T;
            else
                return parent.FindParent<T>();
        }

        public static T FindChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            // return null if nothing
            if (parent == null)
                return null;

            // get child count of parent
            var childCount = VisualTreeHelper.GetChildrenCount(parent);

            // go through each child and search recursively
            for (var i = 0; i < childCount; i++)
            {
                var elt = VisualTreeHelper.GetChild(parent, i);
                if (elt is T) return (T)elt;
                var result = FindChild<T>(elt);
                if (result != null) return result;
            }

            return null;
        }

        public static Task BeginAsync(this Storyboard storyboard)
        {
            System.Threading.Tasks.TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            if (storyboard == null)
                tcs.SetException(new ArgumentNullException());
            else {
                EventHandler<object> onComplete = null;
                onComplete = (s, e) => {
                    storyboard.Completed -= onComplete;
                    tcs.SetResult(true);
                };
                storyboard.Completed += onComplete;
                storyboard.Begin();
            }
            return tcs.Task;
        }

        public static string GenerateRandomName(this FrameworkElement target)
        {
            // create random name with guid
            var name = "_" + Guid.NewGuid().ToString("N");

            // set target name
            target.Name = name;

            // return name
            return name;
        }

    }
}
