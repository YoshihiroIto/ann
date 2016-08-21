using System.IO;

namespace Ann.GenLanguageFile
{
    class Program
    {
        static void Main(string[] argv)
        {
            var options = new Exporter.OutputOptions
            {
                Namespace = argv[1]
            };

            var code = new Exporter().Export(options).Result;

            File.WriteAllText(argv[0], code);
        }
    }
}
