using System.IO;

namespace Ann.GenLanguageFile
{
    public class Program
    {
        public static int Main(string[] argv)
        {
            if (argv.Length != 3)
                return 1;

            var options = new Exporter.OutputOptions
            {
                Namespace = argv[2]
            };

            var r = new Exporter().Export(options).Result;

            File.WriteAllText(argv[0], r.Class);
            File.WriteAllText(argv[1], r.DefaultXaml);

            return 0;
        }
    }
}
