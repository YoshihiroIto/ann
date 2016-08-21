using System.IO;

namespace Ann.GenLanguageFile
{
    class Program
    {
        static void Main(string[] argv)
        {
            var options = new Exporter.OutputOptions
            {
                Namespace = argv[2]
            };

            var r = new Exporter().Export(options).Result;

            File.WriteAllText(argv[0], r.Class);
            File.WriteAllText(argv[1], r.DefaultXaml);
        }
    }
}
