using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ann.Foundation.Test
{
    [TestClass]
    public class WindowHelperTest
    {
        [TestMethod]
        public void Simple()
        {
            // 例外にならない
            var w = new Window();
            WindowHelper.EnableBlur(w);
        }
    }
}
