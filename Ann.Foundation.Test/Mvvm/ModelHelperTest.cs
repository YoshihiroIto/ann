using System.Collections.ObjectModel;
using Ann.Foundation.Mvvm;
using Xunit;

namespace Ann.Foundation.Test.Mvvm
{
    public class ModelHelperTest
    {
        [Fact]
        public void Basic()
        {
            var a = "a";
            var b = "b";
            var c = "c";
            var d = "d";

            {
                var collection = new ObservableCollection<string> {a, b, c, d};
                ModelHelper.MovoTo(collection, a, 2);

                Assert.Equal(new[] {b, a, c, d}, collection);
            }

            {
                var collection = new ObservableCollection<string> {a, b, c, d};
                ModelHelper.MovoTo(collection, d, 2);

                Assert.Equal(new[] {a, b, d, c}, collection);
            }
        }
    }
}