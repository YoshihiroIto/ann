using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ann.Foundation.Test
{
    [TestClass]
    public class TimeMeasureTest
    {
        [TestMethod]
        public void Simple()
        {
            using (new TimeMeasure())
            {
            }
        }

        [TestMethod]
        public void Title()
        {
            using (new TimeMeasure("TimeMeasureTest"))
            {
            }
        }
    }
}
