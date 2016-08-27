using Xunit;

namespace Ann.Core.Test
{
    public class MenuCommandTest
    {
        [Fact]
        public void DefaultCtor()
        {
            var m = new MenuCommand();

            Assert.Equal((StringTags)0, m.Caption);
            Assert.Null(m.Command);
        }

        [Fact]
        public void Basic()
        {
            // ReSharper disable once UnusedVariable
            var m = new MenuCommand
            {
                Command = null,
                Caption = StringTags.File
            };

        }
    }
}