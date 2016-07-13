namespace Ann.Config
{
    public class App
    {
        public string[] HighPriorities { get; set; }
        public MainWindow MainWindow { get; set; }
    }

    public class MainWindow
    {
        public double Left { get; set; }
        public double Top { get; set; }

        public int MaxCandidateLinesCount { get; set; } = Constants.DefaultMaxCandidateLinesCount;
    }
}