using Xunit;

namespace Ann.Foundation.Test
{
    public class TimeMeasureTest
    {
        [Fact]
        public void Basic()
        {
            // 例外にならない
            using (new TimeMeasure())
            {
            }
        }

        [Fact]
        public void Title()
        {
            // 例外にならない
            using (new TimeMeasure("TimeMeasureTest"))
            {
            }
        }
    }
}