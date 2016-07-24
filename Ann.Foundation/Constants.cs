using System;

namespace Ann.Foundation
{
    public static class Constants
    {
        public const double IconSize = 48;

        public static string SystemFolder => Environment.GetFolderPath(Environment.SpecialFolder.System);
        public static string SystemX86Folder => Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
        public static string ProgramsFolder => Environment.GetFolderPath(Environment.SpecialFolder.Programs);

        public static string ProgramFilesFolder =>
            Environment.Is64BitProcess
                ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                : Environment.GetEnvironmentVariable("ProgramW6432");

        public static string ProgramFilesX86Folder =>
            Environment.Is64BitProcess
                ? Environment.GetEnvironmentVariable("PROGRAMFILES(X86)")
                : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

        public static readonly OpenSource[] OpenSources =
        {
            new OpenSource
            {
                Name = "FlatBuffers",
                Auther = "Google",
                Summry = "Memory Efficient Serialization Library",
                Url = "https://github.com/google/flatbuffers"
            },
            new OpenSource
            {
                Name = "LRU Cache",
                Auther = "Yoshihiro Ito",
                Summry = "Simple Implementation C# LRU Cache",
                Url = "https://github.com/YoshihiroIto/Jewelry"
            },
            new OpenSource
            {
                Name = "Hardcodet WPF NotifyIcon",
                Auther = "Philipp Sumi",
                Summry =
                    "This is an implementation of a NotifyIcon (aka system tray icon or taskbar icon) for the WPF platform. It does not just rely on the Windows Forms NotifyIcon component, but is a purely independent control which leverages several features of the WPF framework in order to display rich tooltips, popups, context menus, and balloon messages. It can be used directly in code or embedded in any XAML file.",
                Url = "http://www.hardcodet.net/wpf-notifyicon"
            },
            new OpenSource
            {
                Name = "Livet Cask",
                Auther = "Livet Project",
                Summry = "Livet(リベット)はWPF4のためのMVVM(Model/View/ViewModel)パターン用インフラストラクチャです。",
                Url = "http://ugaya40.hateblo.jp/entry/Livet"
            },
            new OpenSource
            {
                Name = "Material Design In XAML Toolkit",
                Auther = "James Willock",
                Summry = "Material Design styles for all major WPF Framework controls",
                Url = "https://github.com/ButchersBoy/MaterialDesignInXamlToolkit"
            },
            new OpenSource
            {
                Name = "Material Design Icons",
                Auther = "Austin Andrews",
                Summry =
                    "Material Design Icons' growing icon collection allows designers and developers targeting various platforms to download icons in the format, color and size they need for any project.",
                Url = "https://materialdesignicons.com/"
            },
            new OpenSource
            {
                Name = "ReactiveProperty",
                Auther = "neuecc xin9le okazuki",
                Summry = "ReactiveProperty is MVVM and Asynchronous Extensions for Reactive Extensions(Rx-Main).",
                Url = "https://github.com/runceel/ReactiveProperty"
            },
            new OpenSource
            {
                Name = "Reactive Extensions",
                Auther = ".NET Foundation and Contributors",
                Summry =
                    "Reactive Extensions Main Library combining the interfaces, core, LINQ, and platform services libraries.",
                Url = "https://github.com/Reactive-Extensions/Rx.NET"
            },
            new OpenSource
            {
                Name = "Interactive Extensions",
                Auther = ".NET Foundation and Contributors",
                Summry = "Interactive Extensions Main Library used to express queries over enumerable sequences.",
                Url = "https://github.com/Reactive-Extensions/Rx.NET"
            },
            new OpenSource
            {
                Name = "Windows API Code Pack",
                Auther = "Aybe",
                Summry = "Windows API Code Pack 1.1",
                Url = "https://github.com/aybe/Windows-API-Code-Pack-1.1"
            },
            new OpenSource
            {
                Name = "YamlDotNet",
                Auther = "Antoine Aubry",
                Summry = "A .NET library for YAML.",
                Url = "http://aaubry.net/pages/yamldotnet.html"
            },
        };
    }

    public enum IndexOpeningResults
    {
        Ok,
        NotFound,
        OldIndex,
        CanNotOpen
    }
}