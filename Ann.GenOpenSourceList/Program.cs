using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            var packagesConfigPaths = MakePackegesFilePath(solutionDirPath);

            var yaml = new Generator().Generate(packagesConfigPaths);
            File.WriteAllText(outputFilePath, yaml);

            return 0;
        }

        public static IEnumerable<string> MakePackegesFilePath(string solutionDirPath)
        {
            var allPackagesConfigPaths = Directory.EnumerateFiles(
                solutionDirPath,
                "packages.config", SearchOption.AllDirectories);

            // packagesフォルダーは無視する
            var packagesConfigPaths =
                allPackagesConfigPaths
                    .Where(p =>
                        p.StartsWith(Path.Combine(solutionDirPath, "packages") + @"\") == false);

            return packagesConfigPaths;
        }
    }
}