using System;
using System.Linq;
using Xunit;

namespace Ann.Core.Test
{
    public class LanguagesTest
    {
        [Fact]
        public void Basic()
        {
            foreach (var s in Enum.GetValues(typeof(StringTags)).Cast<StringTags>())
                foreach (var l in Enum.GetValues(typeof(Languages)).Cast<Languages>())
                    Assert.NotNull(Localization.GetString(l, s));
        }
    }
}