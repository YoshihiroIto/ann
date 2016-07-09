using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ann.Core.Test
{
    [TestClass]
    public class CrawlerTest
    {
        [TestMethod]
        public void Simple()
        {
            Crawler.Execute(
                "Test.db",
                new[]
                {
                    @"C:\Program Files",
                    @"C:\Program Files (x86)"
                });

            Assert.IsTrue(File.Exists("Test.db"));

            using (var holder = new ExecutableUnitHolder("Test.db"))
            {
                var v = holder.Find("vim");

                Assert.IsTrue(v.Any());
            }
        }
    }
}
