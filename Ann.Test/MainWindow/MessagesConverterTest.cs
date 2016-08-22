using System;
using System.Windows.Threading;
using Ann.Core;
using Ann.Foundation;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class MessagesConverterTest : IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public MessagesConverterTest()
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
        public void Convert_WithOpt()
        {
            var configHolder = new ConfigHolder(_config.RootPath) {Config = {Culture = "en"}};

            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                var c = new MessagesConverter();

                var messages = new[]
                {
                    new StatusBarItemViewModel.Message
                    {
                        String = StringTags.AutoUpdateStates_CloseAfterNSec,
                        Options = new object[] {123}
                    },
                    new StatusBarItemViewModel.Message
                    {
                        String = StringTags.AutoUpdateStates_CloseAfterNSec,
                        Options = new object[] {456}
                    }
                };

                var values = new object[]
                {
                    messages,
                    app
                };

                var expected =
                    string.Format(
                        Localization.GetString(Languages.en, StringTags.AutoUpdateStates_CloseAfterNSec), 123) +
                    string.Format(
                        Localization.GetString(Languages.en, StringTags.AutoUpdateStates_CloseAfterNSec), 456);

                var actual = c.Convert(values, null, null, null);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Convert_WithoutOpt()
        {
            var configHolder = new ConfigHolder(_config.RootPath) {Config = {Culture = "en"}};

            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                var c = new MessagesConverter();

                var messages = new[]
                {
                    new StatusBarItemViewModel.Message
                    {
                        String = StringTags.Settings_General,
                    },
                    new StatusBarItemViewModel.Message
                    {
                        String = StringTags.MenuItem_Exit
                    }
                };

                var values = new object[]
                {
                    messages,
                    app
                };

                var expected =
                        Localization.GetString(Languages.en, StringTags.Settings_General) +
                        Localization.GetString(Languages.en, StringTags.MenuItem_Exit);

                var actual = c.Convert(values, null, null, null);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void ConvertBack()
        {
            var c = new MessagesConverter();

            Assert.Throws<NotImplementedException>(() =>
                c.ConvertBack("abc", null, null, null));
        }
    }
}