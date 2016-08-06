using System;
using Ann.Foundation.Control.ValueConverter;
using Xunit;

namespace Ann.Foundation.Test.Control.ValueConverter
{
    public class TypeofConverterTest
    {
        [Fact]
        public void Convert()
        {
            var c = new TypeofConverter();

            var t = c.Convert("abc", null, null, null);

            Assert.Equal(typeof(string), t);
        }

        [Fact]
        public void ConvertBack()
        {
            var c = new TypeofConverter();

            Assert.Throws<NotImplementedException>(() =>
                c.ConvertBack("abc", null, null, null));
        }
    }
}