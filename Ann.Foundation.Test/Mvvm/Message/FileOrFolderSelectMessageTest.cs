using Ann.Foundation.Mvvm.Message;
using Xunit;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Ann.Foundation.Test.Mvvm.Message
{
    public class FileOrFolderSelectMessageTest
    {
        [Fact]
        public void Simple()
        {
            var filters = new[]
                {
                    new CommonFileDialogFilter("111", "222"),
                    new CommonFileDialogFilter("333", "444")
                };


            // 例外にならな：い
            var m = new FileOrFolderSelectMessage
            {
                IsFolderPicker = true,
                InitialDirectory = "AAA",
                Filters = filters,
                Response = "BBB"
            };

            Assert.True(m.IsFolderPicker);
            Assert.Equal(m.InitialDirectory, "AAA");
            Assert.Same(m.Filters, filters);
            Assert.Equal(m.Response, "BBB");
        }
    }
}