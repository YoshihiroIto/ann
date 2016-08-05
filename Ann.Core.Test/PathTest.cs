using System.IO;
using Xunit;
using YamlDotNet.Serialization;

namespace Ann.Core.Test
{
    public class PathTest
    {
        [Fact]
        public void Basic()
        {
            var path = new Path("AAA");

            Assert.Equal("AAA", path.Value);
        }

        [Fact]
        public void Serialize()
        {
            var path = new Path("AAA");

            using (var writer = new StringWriter())
            {
                new Serializer(SerializationOptions.EmitDefaults).Serialize(writer, path);

                var yaml = writer.ToString();

                Assert.Contains("Path: AAA", yaml);
            }
        }
    }
}