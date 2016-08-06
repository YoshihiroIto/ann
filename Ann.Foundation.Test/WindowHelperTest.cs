using System.Windows;
using Xunit;

namespace Ann.Foundation.Test
{
    public class WindowHelperTest
    {
        [WpfFact]
        public void EnableBlur()
        {
            // 例外にならない
            var w = new Window();
            WindowHelper.EnableBlur(w);
        }
    }
}
