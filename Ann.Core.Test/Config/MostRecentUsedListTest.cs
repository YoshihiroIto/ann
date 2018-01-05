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
            Assert.Equal(0, c.AppPath.Count);

            c.AppPath = new ObservableCollection<string>
            {
                "AAA"
            };
            Assert.Equal(1, c.AppPath.Count);
            Assert.Equal("AAA", c.AppPath[0]);
        }
    }
}