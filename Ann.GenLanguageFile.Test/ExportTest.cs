using System;
using System.IO;
using System.Net.NetworkInformation;
using Ann.Foundation;
using Xunit;

namespace Ann.GenLanguageFile.Test
{
    public class ExportTest : IDisposable
    {
        private readonly DisposableFileSystem _context = new DisposableFileSystem();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Export()
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            if (File.Exists(Exporter.ClientSecretFilePath) == false)
                return;

            var options = new Exporter.OutputOptions
            {
                Namespace = "ABC"
            };

            var r = new Exporter().Export(options).Result;

            Assert.Contains("namespace ABC", r.Class);
            Assert.Contains("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", r.DefaultXaml);
        }

        [Fact]
        public void ProgramNoArgs()
        {
            var i = GenLanguageFile.Program.Main(new string[0]);
            Assert.Equal(i, 1);
        }

        [Fact]
        public void Program()
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            if (File.Exists(Exporter.ClientSecretFilePath) == false)
                return;

            var classPath = Path.Combine(_context.RootPath, "class.cs");
            var defaultXamlPath = Path.Combine(_context.RootPath, "default.xaml");
            var nameSpace = "XYZ";

            var i = GenLanguageFile.Program.Main(new [] {classPath, defaultXamlPath, nameSpace});
            Assert.Equal(i, 0);

            Assert.True(File.Exists(classPath));
            Assert.True(File.Exists(defaultXamlPath));
        }
    }
}