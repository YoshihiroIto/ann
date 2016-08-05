using System.IO;

namespace Ann.GenOpenSourceList
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 2)
                return 1;

            var solutionDirPath = args[0];
            var outputFilePath = args[1];

            var packagesConfigPaths = Directory.EnumerateFiles(
                solutionDirPath,
                "packages.config", SearchOption.AllDirectories);

            var yaml = new Generator().Generate(packagesConfigPaths);
            File.WriteAllText(outputFilePath, yaml);

            return 0;
        }
    }
}