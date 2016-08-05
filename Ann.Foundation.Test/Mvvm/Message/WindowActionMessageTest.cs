using Ann.Foundation.Mvvm.Message;
using Xunit;

namespace Ann.Foundation.Test.Mvvm.Message
{
    public class WindowActionMessageTest
    {
        [Fact]
        public void Simple()
        {
            var m = new WindowActionMessage(WindowAction.Active);

            Assert.Equal(WindowAction.Active, m.Action);

            Assert.False(m.IsOk);
            m.IsOk = true;
            Assert.True(m.IsOk);
        }
    }
}
