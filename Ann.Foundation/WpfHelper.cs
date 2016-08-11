using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ann.Foundation
{
    public static class WpfHelper
    {
        public static bool IsDesignMode { get; }
            = (bool) DependencyPropertyDescriptor.FromProperty(
                DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;

        public static async Task DoEventsAsync()
        {
            await Task.Run(() => DoEvents());
        }

        private static void DoEvents()
        {
            var frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames), frame);

            Dispatcher.PushFrame(frame);
        }

        private static object ExitFrames(object f)
        {
            ((DispatcherFrame) f).Continue = false;

            return null;
        }

        //http://stackoverflow.com/questions/19523139/find-control-in-the-visual-tree 
        public static DependencyObject FindChild(DependencyObject parent, string name)
        {
            if (parent == null || string.IsNullOrEmpty(name))
                return null;

            var element = parent as FrameworkElement;
            if (element != null && element.Name == name)
                return parent;

            DependencyObject result = null;

            (parent as FrameworkElement)?.ApplyTemplate();

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i != childrenCount; ++i)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                result = FindChild(child, name);

                if (result != null)
                    break;
            }

            return result;
        }

        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
                return null;

            var findChild = parent as T;
            if (findChild != null)
                return findChild;

            DependencyObject foundChild = null;

            (parent as FrameworkElement)?.ApplyTemplate();

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i != childrenCount; ++i)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                foundChild = FindChild<T>(child);

                if (foundChild != null)
                    break;
            }

            return (T) foundChild;
        }
    }
}