using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ann.Foundation.Test
{
    [TestClass]
    public class TimeMeasureTest
    {
        [TestMethod]
        public void Simple()
        {
            // 例外にならない
            using (new TimeMeasure())
            {
            }
        }

        [TestMethod]
        public void Title()
        {
            // 例外にならない
            using (new TimeMeasure("TimeMeasureTest"))
            {
            }
        }
    }
}
