using System;
using System.Windows.Threading;
using Ann.Core;
using Ann.MainWindow;
using Xunit;

namespace Ann.Test.MainWindow
{
    public class MessagesConverterTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();

            // for appveyor 
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Fact]
        public void Convert_WithOpt()
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
                _context.GetInstance<App>()
            };

            _context.GetInstance<Core.Config.App>().Culture = "en";

            var expected =
                string.Format(
                    Localization.GetString(Languages.en, StringTags.AutoUpdateStates_CloseAfterNSec), 123) +
                string.Format(
                    Localization.GetString(Languages.en, StringTags.AutoUpdateStates_CloseAfterNSec), 456);

            var actual = c.Convert(values, null, null, null);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Convert_WithoutOpt()
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
                _context.GetInstance<App>()
            };

            _context.GetInstance<Core.Config.App>().Culture = "en";

            var expected =
                Localization.GetString(Languages.en, StringTags.Settings_General) +
                Localization.GetString(Languages.en, StringTags.MenuItem_Exit);

            var actual = c.Convert(values, null, null, null);

            Assert.Equal(expected, actual);
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