using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Ann.Foundation.Control.Behavior;
using Reactive.Bindings;
using Xunit;

namespace Ann.Foundation.Test.Control.Behavior
{
    public class WindowTaskTrayIconBehaviorTest
    {
        [WpfFact]
        public void Basic()
        {
            var w = new Window();

            var b = new WindowTaskTrayIconBehavior();

            b.Attach(w);

            Assert.Null(b.ToolTipText);
            b.ToolTipText = "ABC";
            Assert.Equal("ABC", b.ToolTipText);

            Assert.Null(b.IconSource);
            b.IconSource = new BitmapImage();
            Assert.NotNull(b.IconSource);

            Assert.Null(b.ContextMenu);
            b.ContextMenu = new ContextMenu();
            Assert.NotNull(b.ContextMenu);

            Assert.Null(b.LeftClickedCommand);
            b.LeftClickedCommand = new ReactiveCommand();
            Assert.NotNull(b.LeftClickedCommand);
            ((IDisposable)b.LeftClickedCommand).Dispose();

            w.Close();

            b.Detach();
        }
    }
}