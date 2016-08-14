using System.Windows;
using Ann.Foundation.Control.Behavior;
using Xunit;

namespace Ann.Foundation.Test.Control.Behavior
{
    public class WindowDisableMinMaxBoxBehaviorTest
    {
        [WpfFact]
        public void Basic()
        {
            var w = new Window();

            var b = new WindowDisableMinMaxBoxBehavior();

            b.Attach(w);

            w.Close();

            b.Detach();
        }
    }
}