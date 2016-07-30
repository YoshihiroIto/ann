using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Ann.Foundation.Control
{
    public static class WpfHelper
    {
        public static bool IsDesignMode { get; }
            = (bool) DependencyPropertyDescriptor.FromProperty(
                DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;

        public static void DoEvents()
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
    }
}