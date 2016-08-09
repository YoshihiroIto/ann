using System;
using System.Reflection;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test
{
    public class IconDecoderTest : IDisposable
    {
        private readonly DisposableFileSystem _context = new DisposableFileSystem();

        public void Dispose() => _context?.Dispose();

        [WpfFact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            var decoder = new IconDecoder();
            var ann = Assembly.GetEntryAssembly();

            var icon1 = decoder.GetIcon(ann.Location);
            Assert.NotNull(icon1);

            var icon2 = decoder.GetIcon(ann.Location);
            Assert.NotNull(icon2);

            Assert.NotSame(icon1, icon2);
        }

        [WpfFact]
        public void Cache()
        {
            TestHelper.CleanTestEnv();

            var decoder = new IconDecoder {IconCacheSize = 10};

            var ann = Assembly.GetEntryAssembly();

            var icon1 = decoder.GetIcon(ann.Location);
            Assert.NotNull(icon1);

            var icon2 = decoder.GetIcon(ann.Location);
            Assert.NotNull(icon2);

            Assert.Same(icon1, icon2);
        }

        [WpfFact]
        public void NotFound()
        {
            TestHelper.CleanTestEnv();

            var decoder = new IconDecoder();

            var icon = decoder.GetIcon("XXXXX");

            Assert.Null(icon);
        }

        [WpfFact]
        public void IconCacheSize()
        {
            TestHelper.CleanTestEnv();

            var decoder = new IconDecoder();

            Assert.Equal(0, decoder.IconCacheSize);
            decoder.IconCacheSize = 10;
            Assert.Equal(10, decoder.IconCacheSize);
        }

        [WpfFact]
        public void IconShareFileExt()
        {
            TestHelper.CleanTestEnv();

            var decoder = new IconDecoder();

            _context.CreateFile("test1.js");
            _context.CreateFile("test2.js");

            var icon = decoder.GetIcon(System.IO.Path.Combine(_context.RootPath, "test1.js"));
            Assert.NotNull(icon);

            var icon2 = decoder.GetIcon(System.IO.Path.Combine(_context.RootPath, "test2.js"));
            Assert.NotNull(icon2);

            _context.Dispose();
        }

        [WpfFact]
        public void NoExt()
        {
            TestHelper.CleanTestEnv();

            var decoder = new IconDecoder();
            _context.CreateFile("AAAA");

            var icon = decoder.GetIcon(System.IO.Path.Combine(_context.RootPath, "AAAA"));
            Assert.Null(icon);

            _context.Dispose();
        }
    }
}