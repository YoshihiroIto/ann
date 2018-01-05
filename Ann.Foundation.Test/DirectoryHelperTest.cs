using System;
using System.Linq;
using Xunit;

namespace Ann.Foundation.Test
{
    public class DirectoryHelperTest : IDisposable
    {
        private readonly DisposableFileSystem _context = new DisposableFileSystem();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Basic()
        {
            _context.CreateFiles(
                "aaa/bbb/1.txt",
                "aaa/ccc/2.txt",
                "aaa/ddd/3.txt",
                "111/4.txt");

            var dirs = DirectoryHelper.EnumerateAllFiles(_context.RootPath).ToArray();

            Assert.Equal(4, dirs.Length);

            Assert.Contains(dirs, d => d.EndsWith(@"aaa\bbb\1.txt"));
            Assert.Contains(dirs, d => d.EndsWith(@"aaa\ccc\2.txt"));
            Assert.Contains(dirs, d => d.EndsWith(@"aaa\ddd\3.txt"));
            Assert.Contains(dirs, d => d.EndsWith(@"111\4.txt"));
        }

        [Fact]
        public void Permission()
        {
            // 例外にならない
            var dirs = DirectoryHelper.EnumerateAllFiles(@"C:\System Volume Information").ToArray();

            Assert.Empty(dirs);
        }
    }
}
