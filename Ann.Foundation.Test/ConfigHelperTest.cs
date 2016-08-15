using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Ann.Foundation.Test
{
    public class ConfigHelperTest : IDisposable
    {
        private readonly DisposableFileSystem _context = new DisposableFileSystem();

        public void Dispose()
        {
            _context.Dispose();
        }

        public class Data
        {
            public int Param0 { get; set; } = 999;
            public int Param1 { get; set; } = 888;
            public string Param2 { get; set; } = "XYZ";
        }

        [Theory]
        [MemberData(nameof(Source))]
        public void Basic(ConfigHelper.Category category)
        {
            var src = new Data {Param0 = 123, Param1 = 456, Param2 = category.ToString()};

            var filePath = Path.Combine(_context.RootPath, $"Ann.{category}.yaml");

            ConfigHelper.WriteConfig(category, _context.RootPath, src);
            Assert.True(File.Exists(filePath));

            var dst = ConfigHelper.ReadConfig<Data>(category, _context.RootPath);

            Assert.Equal(src.Param0, dst.Param0);
            Assert.Equal(src.Param1, dst.Param1);
            Assert.Equal(src.Param2, dst.Param2);
        }

        [Theory]
        [MemberData(nameof(Source))]
        public void DefaultOnNotFound(ConfigHelper.Category category)
        {
            var dst = ConfigHelper.ReadConfig<Data>(category, _context.RootPath);

            Assert.Equal(999, dst.Param0);
            Assert.Equal(888, dst.Param1);
            Assert.Equal("XYZ", dst.Param2);
        }

        [Theory]
        [MemberData(nameof(Source))]
        public void DefaultOnBrokenFile(ConfigHelper.Category category)
        {
            var filePath = Path.Combine(_context.RootPath, $"Ann.{category}.yaml");
            File.WriteAllText(filePath, "!!!!");

            var dst = ConfigHelper.ReadConfig<Data>(category, _context.RootPath);

            Assert.Equal(999, dst.Param0);
            Assert.Equal(888, dst.Param1);
            Assert.Equal("XYZ", dst.Param2);
        }

        [Theory]
        [MemberData(nameof(Source))]
        public void DefaultOnZeroFile(ConfigHelper.Category category)
        {
            var filePath = Path.Combine(_context.RootPath, $"Ann.{category}.yaml");
            File.WriteAllBytes(filePath, new byte[0]);

            var dst = ConfigHelper.ReadConfig<Data>(category, _context.RootPath);

            Assert.Equal(999, dst.Param0);
            Assert.Equal(888, dst.Param1);
            Assert.Equal("XYZ", dst.Param2);
        }

        public static object[][] Source
        {
            get
            {
                return Enum.GetValues(typeof(ConfigHelper.Category))
                    .Cast<object>()
                    .Select(x => new[] {x})
                    .ToArray();
            }
        }
    }
}