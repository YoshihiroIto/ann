using System;
using System.Windows.Input;

namespace Ann.Config
{
    public class App
    {
        public string[] TargetFolders { get; set; } =
            {
                @"C:\Program Files",
                @"C:\Program Files (x86)",
                $@"{Environment.ExpandEnvironmentVariables("%SystemRoot%")}\System32"
            };

        public string[] HighPriorities { get; set; }

        public MainWindow MainWindow { get; set; }
    }

    public class MainWindow
    {
        public double Left { get; set; }
        public double Top { get; set; }

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