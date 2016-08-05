using System;
using System.IO;
using Ann.Foundation;
using Xunit;
using YamlDotNet.Serialization;

namespace Ann.GenOpenSourceList.Test
{
    public class GenerateTest : IDisposable
    {
        private readonly DisposableFileSystem _context = new DisposableFileSystem();

        private const string Packages =
            "<packages> <package id=\"ReactiveProperty\" version=\"2.9.0\" targetFramework=\"net461\" developmentDependency=\"true\"/> </packages>";

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Generate()
        {
            var packagesFilePath = Path.Combine(_context.RootPath, "packages.config");
            File.WriteAllText(packagesFilePath, Packages);

            var g = new Generator();
            var d = g.Generate(new[] {packagesFilePath});

            using (var reader = new StringReader(d))
            {
                var list = new Deserializer().Deserialize<OpenSource[]>(reader);
                Assert.Equal(1, list.Length);

                Assert.Equal("ReactiveProperty", list[0].Name);
                Assert.Equal("neuecc xin9le okazuki", list[0].Auther);
                Assert.Equal(
                    "ReactiveProperty is MVVM and Asynchronous Extensions for Reactive Extensions(Rx-Main). Target is .NET 4, .NET 4.x, WP8, Windows store apps(Win8.1), Windows Phone 8.1, UWP.",
                    list[0].Summry);
                Assert.Equal("https://github.com/runceel/ReactiveProperty",list[0].Url);
            }
        }

        [Fact]
        public void ProgramNoArgs()
        {
            var i = GenOpenSourceList.Program.Main(new string[0]);
            Assert.Equal(i, 1);
        }

        [Fact]
        public void Program()
        {
            var packagesFilePath = Path.Combine(_context.RootPath, "packages.config");
            File.WriteAllText(packagesFilePath, Packages);

            var i = GenOpenSourceList.Program.Main(new[]
            {
                _context.RootPath,
                Path.Combine(_context.RootPath, "output.yaml")
            });

            Assert.Equal(i, 0);

            using (var reader = new StringReader(File.ReadAllText(Path.Combine(_context.RootPath, "output.yaml"))))
            {
                var list = new Deserializer().Deserialize<OpenSource[]>(reader);
                Assert.Equal(1, list.Length);

                Assert.Equal("ReactiveProperty", list[0].Name);
                Assert.Equal("neuecc xin9le okazuki", list[0].Auther);
                Assert.Equal(
                    "ReactiveProperty is MVVM and Asynchronous Extensions for Reactive Extensions(Rx-Main). Target is .NET 4, .NET 4.x, WP8, Windows store apps(Win8.1), Windows Phone 8.1, UWP.",
                    list[0].Summry);
                Assert.Equal("https://github.com/runceel/ReactiveProperty",list[0].Url);
            }
        }
    }
}