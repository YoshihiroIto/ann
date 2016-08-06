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
        [InlineData(new[] {".exe"}, new[] {"target1/aaa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})
        ]
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
    }
}