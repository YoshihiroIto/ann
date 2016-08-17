using Ann.Foundation.Mvvm.Message;
using Xunit;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Ann.Foundation.Test.Mvvm.Message
{
    public class FileOrFolderSelectMessageTest
    {
        [Fact]
        public void Basic()
        {
            var filters = new[]
                {
                    new CommonFileDialogFilter("111", "222"),
                    new CommonFileDialogFilter("333", "444")
                };


            // 例外にならない
            var m = new FileOrFolderSelectMessage
            {
                IsFolderPicker = true,
                InitialDirectory = "AAA",
                Filters = filters,
                Response = "BBB"
            };

            Assert.True(m.IsFolderPicker);
            Assert.Equal("AAA", m.InitialDirectory);
            Assert.Same(m.Filters, filters);
            Assert.Equal("BBB", m.Response);
        }
    }
}