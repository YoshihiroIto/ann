using Xunit;

namespace Ann.Foundation.Test
{
    public class WpfHelperTest
    {
        [WpfFact]
        public void IsDesignMode()
        {
            Assert.False(WpfHelper.IsDesignMode);
        }

        [WpfFact]
        public void DoEventsAsync()
        {
            // 例外にならない
            WpfHelper.DoEventsAsync().Wait();
        }

    }
}
