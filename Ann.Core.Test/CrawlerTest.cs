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
            Crawler.ExecuteAsync(
                "Test.db",
                new[]
                {
                    @"C:\Program Files",
                    @"C:\Program Files (x86)"
                }).Wait();

            Assert.IsTrue(File.Exists("Test.db"));

            using (var holder = new ExecutableUnitDataBase("Test.db"))
            {
                var v = holder.Find("vim");

                Assert.IsTrue(v.Any());
            }
        }
    }
}
