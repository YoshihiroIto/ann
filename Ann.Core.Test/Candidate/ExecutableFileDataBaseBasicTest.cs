using System;
using System.IO;
using System.Linq;
using Ann.Core.Candidate;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test.Candidate
{
    public class ExecutableFileDataBaseBasicTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();
        private readonly DisposableFileSystem _dataFs = new DisposableFileSystem();

        public void Dispose()
        {
            _context.Dispose();
            _dataFs.Dispose();
        }

        [Fact]
        public async void NotFoundOpenIndex()
        {
            var app = _context.GetInstance<App>();
            {
                var dbFilePath = System.IO.Path.Combine(_dataFs.RootPath, "index.dat");
                var targetPaths = new[] {System.IO.Path.Combine(_dataFs.RootPath, "target1")};

                var db = new ExecutableFileDataBase(app, dbFilePath);

                var r = await db.OpenIndexAsync(targetPaths);

                Assert.Equal(IndexOpeningResults.NotFound, r);
            }
        }

        [Theory]
        [InlineData(new[] {"exe"}, new[] {"target1/aaa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public async void OpenIndex(string[] executableFileExts, string[] targetFiles)
        {
            var app = _context.GetInstance<App>();
            {
                _dataFs.CreateFiles(targetFiles);

                var dbFilePath = System.IO.Path.Combine(_dataFs.RootPath, "index.dat");
                var targetPaths = new[] {System.IO.Path.Combine(_dataFs.RootPath, "target1")};

                var db = new ExecutableFileDataBase(app, dbFilePath);
                var r = await db.UpdateIndexAsync(targetPaths, executableFileExts);

                Assert.Equal(IndexOpeningResults.Ok, r);
                Assert.Equal(3, db.ExecutableFileCount);
            }
        }

        [Theory]
        [InlineData(new[] {"exe"}, new[] {"target1/aaa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public async void ReopenIndex(string[] executableFileExts, string[] targetFiles)
        {
            _dataFs.CreateFiles(targetFiles);

            {
                var app = _context.GetInstance<App>();
                {
                    var dbFilePath = System.IO.Path.Combine(_dataFs.RootPath, "index.dat");
                    var targetPaths = new[] {System.IO.Path.Combine(_dataFs.RootPath, "target1")};

                    var db = new ExecutableFileDataBase(app, dbFilePath);
                    var r = await db.UpdateIndexAsync(targetPaths, executableFileExts);

                    Assert.Equal(IndexOpeningResults.Ok, r);
                    Assert.Equal(3, db.ExecutableFileCount);
                }
            }

            {
                var app = _context.GetInstance<App>();
                {
                    var dbFilePath = System.IO.Path.Combine(_dataFs.RootPath, "index.dat");
                    var targetPaths = new[] {System.IO.Path.Combine(_dataFs.RootPath, "target1")};

                    var db = new ExecutableFileDataBase(app, dbFilePath);
                    var r = await db.OpenIndexAsync(targetPaths);

                    Assert.Equal(IndexOpeningResults.Ok, r);
                }
            }
        }

        [Theory]
        [InlineData(new[] {"exe"}, new[] {"target1/aaa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public async void ReopenIndexNotFound(string[] executableFileExts, string[] targetFiles)
        {
            _dataFs.CreateFiles(targetFiles);

            {
                var app = _context.GetInstance<App>();
                {
                    var dbFilePath = System.IO.Path.Combine(_dataFs.RootPath, "index.dat");
                    var targetPaths = new[] {System.IO.Path.Combine(_dataFs.RootPath, "target1")};

                    var db = new ExecutableFileDataBase(app, dbFilePath);
                    var r = await db.UpdateIndexAsync(targetPaths, executableFileExts);

                    Assert.Equal(IndexOpeningResults.Ok, r);
                    Assert.Equal(3, db.ExecutableFileCount);
                }
            }

            foreach (var f in targetFiles)
                File.Delete(System.IO.Path.Combine(_dataFs.RootPath, f));

            {
                var app = _context.GetInstance<App>();
                {
                    var dbFilePath = System.IO.Path.Combine(_dataFs.RootPath, "index.dat");
                    var targetPaths = new[] {System.IO.Path.Combine(_dataFs.RootPath, "target1")};

                    var db = new ExecutableFileDataBase(app, dbFilePath);
                    var r = await db.OpenIndexAsync(targetPaths);

                    Assert.Equal(IndexOpeningResults.Ok, r);

                    var f = db.Find("aaa", executableFileExts);
                    Assert.Equal(0, f.Count());
                }
            }
        }

        [Theory]
        [InlineData(new[] {"exe"}, new[] {"target1/aaa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        [InlineData(new[] {".exe"}, new[] {"target1/aaa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})
        ]
        public async void UpdateIndex(string[] executableFileExts, string[] targetFiles)
        {
            _dataFs.CreateFiles(targetFiles);

            var app = _context.GetInstance<App>();
            {
                var dbFilePath = System.IO.Path.Combine(_dataFs.RootPath, "index.dat");
                var targetPaths = new[] {System.IO.Path.Combine(_dataFs.RootPath, "target1")};

                var db = new ExecutableFileDataBase(app, dbFilePath);
                var r = await db.UpdateIndexAsync(targetPaths, executableFileExts);

                Assert.Equal(IndexOpeningResults.Ok, r);
                Assert.Equal(3, db.ExecutableFileCount);
            }
        }

        [Theory]
        [InlineData(new[] {"exe"}, new[] {"target1/aAa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public async void CrawingData1(string[] executableFileExts, string[] targetFiles)
        {
            _dataFs.CreateFiles(targetFiles);

            var app = _context.GetInstance<App>();
            {
                var dbFilePath = System.IO.Path.Combine(_dataFs.RootPath, "index.dat");
                var targetPaths = new[] {System.IO.Path.Combine(_dataFs.RootPath, "target1")};

                var db = new ExecutableFileDataBase(app, dbFilePath);
                var r = await db.UpdateIndexAsync(targetPaths, executableFileExts);

                Assert.Equal(IndexOpeningResults.Ok, r);
                Assert.Equal(3, db.ExecutableFileCount);

                var f = db.Find("aaa", executableFileExts).ToArray();

                Assert.Equal(1, f.Length);

                Assert.Equal(System.IO.Path.Combine(_dataFs.RootPath, @"target1\aAa.exe"), f[0].Path);
                Assert.Equal(string.Empty, f[0].Directory);
                Assert.Equal("aAa", f[0].FileName);
                Assert.Equal("aAa", f[0].Name);
                Assert.Equal("aAa**aAa", f[0].SearchKey);
                Assert.Null(f[0].DirectoryParts);
                Assert.Null(f[0].FileNameParts);
                Assert.Null(f[0].NameParts);
            }
        }

        [Theory]
        [InlineData(new[] {"exe"},
             new[] {"target1/xxx/YYY/zzz/aAa 123.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public async void CrawingData2(string[] executableFileExts, string[] targetFiles)
        {
            _dataFs.CreateFiles(targetFiles);

            var app = _context.GetInstance<App>();
            {
                var dbFilePath = System.IO.Path.Combine(_dataFs.RootPath, "index.dat");
                var targetPaths = new[] {System.IO.Path.Combine(_dataFs.RootPath, "target1")};

                var db = new ExecutableFileDataBase(app, dbFilePath);
                var r = await db.UpdateIndexAsync(targetPaths, executableFileExts);

                Assert.Equal(IndexOpeningResults.Ok, r);
                Assert.Equal(3, db.ExecutableFileCount);

                var f = db.Find("aaa", executableFileExts).ToArray();

                Assert.Equal(1, f.Length);

                Assert.Equal(System.IO.Path.Combine(_dataFs.RootPath, @"target1\xxx\YYY\zzz\aAa 123.exe"), f[0].Path);
                Assert.Equal(@"\xxx\YYY\zzz", f[0].Directory);
                Assert.Equal("aAa 123", f[0].FileName);
                Assert.Equal("aAa 123", f[0].Name);
                Assert.Equal(@"aAa 123*\xxx\YYY\zzz*aAa 123", f[0].SearchKey);
                Assert.Equal(new[] {"xxx", "YYY", "zzz"}, f[0].DirectoryParts);
                Assert.Equal(new[] {"aAa", "123"}, f[0].FileNameParts);
                Assert.Equal(new[] {"aAa", "123"}, f[0].NameParts);
            }
        }
    }
}