using System.IO;
using Xunit;
using YamlDotNet.Serialization;

namespace Ann.Core.Test
{
    public class PathTest
    {
        public PathTest()
        {
            TestHelper.CleanTestEnv();
        }

        [Fact]
        public void Basic()
        {
            var path = new Path("AAA");

            Assert.Equal("AAA", path.Value);
        }

        [Fact]
        public void DefaultCtor()
        {
            var path = new Path();

            Assert.Equal(string.Empty, path.Value);
        }

        [Fact]
        public void Serialize()
        {
            var path = new Path("AAA");

            using (var writer = new StringWriter())
            {
                new SerializerBuilder().EmitDefaults().Build()
                    .Serialize(writer, path);

                var yaml = writer.ToString();

                Assert.Contains("Path: AAA", yaml);
            }
        }
    }
}