using System;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test
{
    public class StringTagToStringConverterTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public void Dispose()
        {
            _context.Dispose();
            _config.Dispose();
        }

        [Fact]
        public void Convert()
        {
            var app = _context.GetInstance<App>();
            {
                _context.GetInstance<Core.Config.App>().Culture = "en";

                var c = new StringTagToStringConverter();

                var expected = Localization.GetString(Languages.en, StringTags.AllFiles);
                var actual = c.Convert(new object[] {StringTags.AllFiles, app}, null, null, null);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void ConvertBack()
        {
            var c = new StringTagToStringConverter();

            Assert.Throws<NotImplementedException>(() =>
                c.ConvertBack("abc", null, null, null));
        }
    }
}