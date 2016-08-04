using System;
using System.IO;
using Xunit;

namespace Ann.Foundation.Test
{
    public class ConfigHelperTest : IDisposable
    {
        private readonly DisposableFileSystem _context = new DisposableFileSystem(); 
        public void Dispose() => _context.Dispose();

        public class Data
        {
            public int Param0 { get; set; }
            public int Param1 { get; set; }
            public string Param2 { get; set; }
        }

        [Theory]
        [InlineData(ConfigHelper.Category.App)]
        [InlineData(ConfigHelper.Category.MainWindow)]
        [InlineData(ConfigHelper.Category.MostRecentUsedList)]
        public void Simple(ConfigHelper.Category category)
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
    }
}