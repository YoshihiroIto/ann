using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
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
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            var packagesFilePath = Path.Combine(_context.RootPath, "packages.config");
            File.WriteAllText(packagesFilePath, Packages);

            var g = new Generator();
            var d = g.Generate(new[] {packagesFilePath});

            using (var reader = new StringReader(d))
            {
                var list = new Deserializer().Deserialize<OpenSource[]>(reader);
                Assert.Single(list);

                Assert.Equal("ReactiveProperty", list[0].Name);
                Assert.Equal("neuecc xin9le okazuki", list[0].Auther);
                Assert.Equal(
                    "ReactiveProperty is MVVM and Asynchronous Extensions for Reactive Extensions(System.Reactive). Target is .NET 4.5, .NET 4.6, UWP ,Xamarin(Android, iOS, Forms) and .NET Standard 1.1.",
                    list[0].Summry);
                Assert.Equal("https://github.com/runceel/ReactiveProperty",list[0].Url);
            }
        }

        [Fact]
        public void ProgramNoArgs()
        {
            var i = GenOpenSourceList.Program.Main(new string[0]);
            Assert.Equal(1, i);
        }

        [Fact]
        public void Program()
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            var packagesFilePath = Path.Combine(_context.RootPath, "packages.config");
            File.WriteAllText(packagesFilePath, Packages);

            var i = GenOpenSourceList.Program.Main(new[]
            {
                _context.RootPath,
                Path.Combine(_context.RootPath, "output.yaml")
            });

            Assert.Equal(0, i);

            using (var reader = new StringReader(File.ReadAllText(Path.Combine(_context.RootPath, "output.yaml"))))
            {
                var list = new Deserializer().Deserialize<OpenSource[]>(reader);
                Assert.Single(list);

                Assert.Equal("ReactiveProperty", list[0].Name);
                Assert.Equal("neuecc xin9le okazuki", list[0].Auther);
                Assert.Equal(
                    "ReactiveProperty is MVVM and Asynchronous Extensions for Reactive Extensions(System.Reactive). Target is .NET 4.5, .NET 4.6, UWP ,Xamarin(Android, iOS, Forms) and .NET Standard 1.1.",
                    list[0].Summry);
                Assert.Equal("https://github.com/runceel/ReactiveProperty",list[0].Url);
            }
        }

        [Fact]
        public void AnnAll()
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            var cd = Directory.GetCurrentDirectory();
            var solutionDirPath = Path.Combine(cd, @"..\..\..\Ann");

            var packagesConfigPaths = GenOpenSourceList.Program.MakePackegesFilePath(solutionDirPath);

            var yaml = new Generator().Generate(packagesConfigPaths);

            using (var reader = new StringReader(yaml))
            {
                var list = new Deserializer().Deserialize<OpenSource[]>(reader);

                var rp = list.Single(s => s.Name == "ReactiveProperty");

                Assert.Equal("neuecc xin9le okazuki", rp.Auther);
                Assert.Equal(
                    "ReactiveProperty is MVVM and Asynchronous Extensions for Reactive Extensions(System.Reactive). Target is .NET 4.5, .NET 4.6, UWP ,Xamarin(Android, iOS, Forms) and .NET Standard 1.1.",
                    rp.Summry);
                Assert.Equal("https://github.com/runceel/ReactiveProperty", rp.Url);
            }
        }
    }
}