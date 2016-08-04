using System.Windows;
using Xunit;

namespace Ann.Foundation.Test
{
    public class WindowHelperTest
    {
        [StaFact]
        public void EnableBlur()
        {
            // 例外にならない
            var w = new Window();
            WindowHelper.EnableBlur(w);
        }
    }
}
