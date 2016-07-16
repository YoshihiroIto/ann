using System;
using System.Windows.Input;

namespace Ann.Config
{
    public class App
    {
        public string[] TargetFolders { get; set; } =
            {
                @"%ProgramFiles%",
                @"%ProgramFiles(x86)%",
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                Environment.GetFolderPath(Environment.SpecialFolder.SystemX86),
                Environment.GetFolderPath(Environment.SpecialFolder.Programs)
            };

        public string[] HighPriorities { get; set; }

        public MainWindow MainWindow { get; set; }
    }

    public class MainWindow
    {
        public double Left { get; set; } = double.NaN;
        public double Top { get; set; } = double.NaN;

        public int MaxCandidateLinesCount { get; set; } = Constants.DefaultMaxCandidateLinesCount;

        public ShortcutKeys ShortcutKeys { get; set; } = new ShortcutKeys();
    }

    public class ShortcutKeys
    {
        public ShortcutKey Activate { get; set; } =
            new ShortcutKey {Key = Key.Space, Modifiers = ModifierKeys.Control};

        public ShortcutKey[] Hide { get; set; } =
            {
                new ShortcutKey {Key = Key.J, Modifiers = ModifierKeys.Control}
            };
    }
}