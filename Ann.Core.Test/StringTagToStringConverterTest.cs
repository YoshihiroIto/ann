using System;
using System.Windows.Threading;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test
{
    public class StringTagToStringConverterTest : IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public StringTagToStringConverterTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _config.Dispose();

            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Fact]
        public void Convert()
        {
            var configHolder = new ConfigHolder(_config.RootPath) {Config = {Culture = "en"}};

            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
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