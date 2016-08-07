using System;
using System.IO;
using System.Linq;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test
{
    public class ExecutableUnitDataBaseBasicTest : IDisposable
    {
        private DisposableFileSystem _context;

        public void Dispose() => _context?.Dispose();

        [Fact]
        public void NotFoundOpenIndex()
        {
            _context = new DisposableFileSystem();

            var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
            var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};

            var db = new ExecutableUnitDataBase(dbFilePath);

            var r = db.OpenIndexAsync(targetPaths);

            Assert.Equal(IndexOpeningResults.NotFound, r.Result);
        }

        [Theory]
        [InlineData(new[] {"exe"}, new[] {"target1/aaa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public void OpenIndex(string[] executableFileExts, string[] targetFiles)
        {
            _context = new DisposableFileSystem();
            _context.CreateFiles(targetFiles);

            var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
            var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};

            var db = new ExecutableUnitDataBase(dbFilePath);
            var r = db.UpdateIndexAsync(targetPaths, executableFileExts);

            Assert.Equal(IndexOpeningResults.Ok, r.Result);
            Assert.Equal(3, db.ExecutableUnitCount);
        }

        [Theory]
        [InlineData(new[] {"exe"}, new[] {"target1/aaa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public void ReopenIndex(string[] executableFileExts, string[] targetFiles)
        {
            _context = new DisposableFileSystem();
            _context.CreateFiles(targetFiles);

            {
                var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
                var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};

                var db = new ExecutableUnitDataBase(dbFilePath);
                var r = db.UpdateIndexAsync(targetPaths, executableFileExts);

                Assert.Equal(IndexOpeningResults.Ok, r.Result);
                Assert.Equal(3, db.ExecutableUnitCount);
            }

            {
                var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
                var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};

                var db = new ExecutableUnitDataBase(dbFilePath);
                var r = db.OpenIndexAsync(targetPaths);

                Assert.Equal(IndexOpeningResults.Ok, r.Result);
            }
        }

        [Theory]
        [InlineData(new[] {"exe"}, new[] {"target1/aaa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public void ReopenIndexNotFound(string[] executableFileExts, string[] targetFiles)
        {
            _context = new DisposableFileSystem();
            _context.CreateFiles(targetFiles);

            {
                var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
                var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};

                var db = new ExecutableUnitDataBase(dbFilePath);
                var r = db.UpdateIndexAsync(targetPaths, executableFileExts);

                Assert.Equal(IndexOpeningResults.Ok, r.Result);
                Assert.Equal(3, db.ExecutableUnitCount);
            }

            foreach (var f in targetFiles)
                File.Delete(System.IO.Path.Combine(_context.RootPath, f));

            {
                var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
                var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};

                var db = new ExecutableUnitDataBase(dbFilePath);
                var r = db.OpenIndexAsync(targetPaths);

                Assert.Equal(IndexOpeningResults.Ok, r.Result);

                var f = db.Find("aaa", executableFileExts);
                Assert.Equal(0, f.Count());
            }
        }

        [Theory]
        [InlineData(new[] {"exe"}, new[] {"target1/aaa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        [InlineData(new[] {".exe"}, new[] {"target1/aaa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public void UpdateIndex(string[] executableFileExts, string[] targetFiles)
        {
            _context = new DisposableFileSystem();
            _context.CreateFiles(targetFiles);

            var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
            var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};

            var db = new ExecutableUnitDataBase(dbFilePath);
            var r = db.UpdateIndexAsync(targetPaths, executableFileExts);

            Assert.Equal(IndexOpeningResults.Ok, r.Result);
            Assert.Equal(3, db.ExecutableUnitCount);
        }

        [Theory]
        [InlineData(new[] {"exe"}, new[] {"target1/aAa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public void CrawingData1(string[] executableFileExts, string[] targetFiles)
        {
            _context = new DisposableFileSystem();
            _context.CreateFiles(targetFiles);

            var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
            var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};

            var db = new ExecutableUnitDataBase(dbFilePath);
            var r = db.UpdateIndexAsync(targetPaths, executableFileExts);

            Assert.Equal(IndexOpeningResults.Ok, r.Result);
            Assert.Equal(3, db.ExecutableUnitCount);

            var f = db.Find("aaa", executableFileExts).ToArray();

            Assert.Equal(1, f.Length);

            Assert.Equal(System.IO.Path.Combine(_context.RootPath, @"target1\aAa.exe"), f[0].Path);
            Assert.Equal(string.Empty, f[0].LowerDirectory);
            Assert.Equal("aaa", f[0].LowerFileName);
            Assert.Equal("aaa", f[0].LowerName);
            Assert.Equal("aAa", f[0].Name);
            Assert.Equal("aaa**aaa", f[0].SearchKey);
            Assert.Null(f[0].LowerDirectoryParts);
            Assert.Null(f[0].LowerFileNameParts);
            Assert.Null(f[0].LowerNameParts);
        }

        [Theory]
        [InlineData(new[] {"exe"}, new[] {"target1/xxx/YYY/zzz/aAa 123.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public void CrawingData2(string[] executableFileExts, string[] targetFiles)
        {
            _context = new DisposableFileSystem();
            _context.CreateFiles(targetFiles);

            var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
            var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};

            var db = new ExecutableUnitDataBase(dbFilePath);
            var r = db.UpdateIndexAsync(targetPaths, executableFileExts);

            Assert.Equal(IndexOpeningResults.Ok, r.Result);
            Assert.Equal(3, db.ExecutableUnitCount);

            var f = db.Find("aaa", executableFileExts).ToArray();

            Assert.Equal(1, f.Length);

            Assert.Equal(System.IO.Path.Combine(_context.RootPath, @"target1\xxx\YYY\zzz\aAa 123.exe"), f[0].Path);
            Assert.Equal(@"\xxx\yyy\zzz", f[0].LowerDirectory);
            Assert.Equal("aaa 123", f[0].LowerFileName);
            Assert.Equal("aaa 123", f[0].LowerName);
            Assert.Equal("aAa 123", f[0].Name);
            Assert.Equal(@"aaa 123*\xxx\yyy\zzz*aaa 123", f[0].SearchKey);
            Assert.Equal(new[] {"xxx", "yyy", "zzz"}, f[0].LowerDirectoryParts);
            Assert.Equal(new[] {"aaa", "123"}, f[0].LowerFileNameParts);
            Assert.Equal(new[] {"aaa", "123"}, f[0].LowerNameParts);
        }
    }
}