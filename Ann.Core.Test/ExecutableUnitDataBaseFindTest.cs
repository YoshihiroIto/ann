using System;
using System.Linq;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test
{
    public class ExecutableUnitDataBaseFindTest : IDisposable
    {
        private readonly DisposableFileSystem _context = new DisposableFileSystem();

        public ExecutableUnitDataBaseFindTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        [Theory]
        [InlineData("bbb", new[] {"target1/aaa.exe", @"target1\bbb.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        [InlineData("BBB", new[] {"target1/aaa.exe", @"target1\BBB.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public async void Basic(string name, string[] targetFiles)
        {
            _context.CreateFiles(targetFiles);

            var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
            var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};
            var executableFileExts = new[] {"exe"};

            var db = new ExecutableUnitDataBase(dbFilePath);
            var r = await db.UpdateIndexAsync(targetPaths, executableFileExts);

            Assert.Equal(IndexOpeningResults.Ok, r);
            Assert.Equal(3, db.ExecutableUnitCount);

            var f = db.Find("bb", executableFileExts).ToArray();

            Assert.Equal(1, f.Length);
            Assert.Equal(name, f[0].Name);
            Assert.Equal(
                System.IO.Path.Combine(_context.RootPath, @"target1\bbb.exe").ToLower(),
                f[0].Path.ToLower());
        }

        [Theory]
        [InlineData("", new[] {"target1/aaa.exe", @"target1\BBB.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        [InlineData(" ", new[] {"target1/aaa.exe", @"target1\BBB.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        [InlineData(null, new[] {"target1/aaa.exe", @"target1\BBB.exe", "target1/ccc.exe", "target1/ddd.bin"})]
        public async void InputEmpty(string input, string[] targetFiles)
        {
            _context.CreateFiles(targetFiles);

            var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
            var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};
            var executableFileExts = new[] {"exe"};

            var db = new ExecutableUnitDataBase(dbFilePath);
            var r = await db.UpdateIndexAsync(targetPaths, executableFileExts);

            Assert.Equal(IndexOpeningResults.Ok, r);
            Assert.Equal(3, db.ExecutableUnitCount);

            var f = db.Find(input, executableFileExts).ToArray();

            Assert.Equal(0, f.Length);
        }

        [Theory]
        [InlineData(
            new[] {@"target1\abc.exe", @"target1\abcd.exe", @"target1\123abc456.exe"},
            new[] {@"target1\123abc456.exe", @"target1\abcd.exe", "target1/abc.exe"},
            "abc")]
        [InlineData(
            new[] {@"target1\abc.exe", @"target1\abcd.exe", @"target1\123 abc123.exe"},
            new[] {@"target1\123 abc123.exe", @"target1\abcd.exe", "target1/abc.exe"},
            "abc")]
        [InlineData(
            new[] {@"target1\abcd.exe", @"target1\123 abc123.exe", @"target1\xabc.exe"},
            new[] {@"target1\123 abc123.exe", @"target1\abcd.exe", "target1/xabc.exe"},
            "abc")]
        // 大文字小文字
        [InlineData(
            new[] {@"target1\abc.exe", @"target1\abcd.exe", @"target1\123abc456.exe"},
            new[] {@"target1\123abc456.exe", @"target1\abcd.exe", "target1/abc.exe"},
            "AbC")]
        // 入力前後にスペース
        [InlineData(
            new[] {@"target1\abc.exe", @"target1\abcd.exe", @"target1\123abc456.exe"},
            new[] {@"target1\123abc456.exe", @"target1\abcd.exe", "target1/abc.exe"},
            " abc")]
        [InlineData(
            new[] {@"target1\abc.exe", @"target1\abcd.exe", @"target1\123abc456.exe"},
            new[] {@"target1\123abc456.exe", @"target1\abcd.exe", "target1/abc.exe"},
            "abc ")]
        [InlineData(
            new[] {@"target1\abc.exe", @"target1\abcd.exe", @"target1\123abc456.exe"},
            new[] {@"target1\123abc456.exe", @"target1\abcd.exe", "target1/abc.exe"},
            " abc ")]
        // 複数入力
        [InlineData(
            new[] {@"target1\123abc456.exe"},
            new[] {@"target1\123abc456.exe", @"target1\abcd.exe", "target1/abc.exe"},
            "456 123")]
        // 見つからない
        [InlineData(
            new string[0],
            new[] {@"target1\123abc456.exe", @"target1\abcd.exe", "target1/abc.exe"},
            "XXX")]
        // ファイル名とディレクトリ名
        [InlineData(
            new[] {@"target1\aaa.exe", @"target1\zzz\aaa.exe", @"target1\aaa\zzz.exe"},
            new[] {@"target1\zzz\aaa.exe", @"target1\aaa\zzz.exe", @"target1\aaa.exe"},
            "aaa")]
        // 複数ターゲット
        [InlineData(
            new[] {@"target3\aaa.exe", @"target1\zzz\aaa.exe", @"target2\aaa\zzz.exe"},
            new[] {@"target1\zzz\aaa.exe", @"target2\aaa\zzz.exe", @"target3\aaa.exe"},
            "aaa")]
        public async void InputScore(string[] expected, string[] targetFiles, string input)
        {
            _context.CreateFiles(targetFiles);

            var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
            var targetPaths = new[]
            {
                System.IO.Path.Combine(_context.RootPath, "target1"),
                System.IO.Path.Combine(_context.RootPath, "target2"),
                System.IO.Path.Combine(_context.RootPath, "target3")
            };
            var executableFileExts = new[] {"exe"};

            var db = new ExecutableUnitDataBase(dbFilePath);
            var r = await db.UpdateIndexAsync(targetPaths, executableFileExts);

            Assert.Equal(IndexOpeningResults.Ok, r);
            Assert.Equal(3, db.ExecutableUnitCount);

            var baseLength = _context.RootPath.Length + 1;
            var candidates = db.Find(input, executableFileExts).ToArray();

            Assert.Equal(
                expected,
                candidates
                    .Select(c => c.Path)
                    .Select(p => p.Substring(baseLength))
                );
        }

        [Theory]
        [InlineData(
            new[] {@"target1\abc.exe", @"target1\abc.bat", @"target1\abc.lnk"},
            new[] {@"target1\abc.lnk", @"target1\abc.bat", @"target1\abc.exe"},
            new[] {"exe", "bat", "lnk"})]
        [InlineData(
            new[] {@"target1\abc.exe", @"target1\abc.bat"},
            new[] {@"target1\abc.txt", @"target1\abc.bat", @"target1\abc.exe"},
            new[] {"exe", "bat", "lnk"})]
        // 大文字小文字
        [InlineData(
            new[] {@"target1\abc.exe", @"target1\abc.bat", @"target1\abc.lnk"},
            new[] {@"target1\abc.lnk", @"target1\abc.bat", @"target1\abc.exe"},
            new[] {"EXE", "BAT", "LNK"})]
        // ドットあり
        [InlineData(
            new[] {@"target1\abc.exe", @"target1\abc.bat"},
            new[] {@"target1\abc.txt", @"target1\abc.bat", @"target1\abc.exe"},
            new[] {".exe", ".bat", ".lnk"})]
        public async void ExtScore(string[] expected, string[] targetFiles, string[] exts)
        {
            _context.CreateFiles(targetFiles);

            var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
            var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};
            var executableFileExts = exts;

            var db = new ExecutableUnitDataBase(dbFilePath);
            var r = await db.UpdateIndexAsync(targetPaths, executableFileExts);

            Assert.Equal(IndexOpeningResults.Ok, r);

            var baseLength = _context.RootPath.Length + 1;
            var candidates = db.Find("abc", executableFileExts).ToArray();

            Assert.Equal(
                candidates
                    .Select(c => c.Path)
                    .Select(p => p.Substring(baseLength)),
                expected);
        }

        [Theory]
        [InlineData(
            new[] {@"target1\abc.exe", @"target1\xxx\abc.exe", @"target1\yyy\abc.exe", @"target1\zzz\abc.exe"},
            new[] {@"target1\abc.exe", @"target1\xxx\abc.exe", @"target1\yyy\abc.exe", @"target1\zzz\abc.exe"},
            "abc")]
        [InlineData(
            new[] {@"target1\xxx\abc.exe"},
            new[] {@"target1\abc.exe", @"target1\xxx\abc.exe", @"target1\yyy\abc.exe", @"target1\zzz\abc.exe"},
            "xxx")]
        [InlineData(
            new[] {@"target1\xxx\def.exe"},
            new[] {@"target1\abc.exe", @"target1\xxx\abc.exe", @"target1\xxx\def.exe", @"target1\xxx\xxx.exe"},
            "def xxx")]
        [InlineData(
            new[] {@"target1\qqq xxx\abc.exe"},
            new[] {@"target1\eee xxx\abc.exe",@"target1\qqq xxx\abc.exe",  @"target1\ggg xxx\abc.exe"},
            "xxx qqq")]
        public async void InputDirectory(string[] expected, string[] targetFiles, string input)
        {
            _context.CreateFiles(targetFiles);

            var dbFilePath = System.IO.Path.Combine(_context.RootPath, "index.dat");
            var targetPaths = new[] {System.IO.Path.Combine(_context.RootPath, "target1")};
            var executableFileExts = new[] {"exe"};

            var db = new ExecutableUnitDataBase(dbFilePath);
            var r = await db.UpdateIndexAsync(targetPaths, executableFileExts);

            Assert.Equal(IndexOpeningResults.Ok, r);

            var baseLength = _context.RootPath.Length + 1;
            var candidates = db.Find(input, executableFileExts).ToArray();

            Assert.Equal(
                candidates
                    .Select(c => c.Path)
                    .Select(p => p.Substring(baseLength)).OrderBy(x => x),
                expected.OrderBy(x => x));
        }
    }
}