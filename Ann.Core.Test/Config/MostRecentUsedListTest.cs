using System.Collections.ObjectModel;
using Ann.Core.Config;
using Xunit;

namespace Ann.Core.Test.Config
{
    public class MostRecentUsedListTest
    {
        [Fact]
        public void Basic()
        {
            var c = new MostRecentUsedList();

            Assert.NotNull(c.AppPath);
            Assert.Empty(c.AppPath);

            c.AppPath = new ObservableCollection<string>
            {
                "AAA"
            };
            Assert.Single(c.AppPath);
            Assert.Equal("AAA", c.AppPath[0]);
        }
    }
}