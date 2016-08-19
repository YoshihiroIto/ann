using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using Xunit;
using System.Windows.Input;
using Ann.Core.Config;

namespace Ann.Core.Test.Config
{
    public class AppTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            var c = new Core.Config.App();

            Assert.Equal(Key.None, c.ShortcutKeys.Activate.Key);
            Assert.Equal(ModifierKeys.None, c.ShortcutKeys.Activate.Modifiers);

            c.ShortcutKeys.Activate = new ShortcutKey
            {
                Key = Key.Space,
                Modifiers = ModifierKeys.Alt
            };
            Assert.Equal(Key.Space, c.ShortcutKeys.Activate.Key);
            Assert.Equal(ModifierKeys.Alt, c.ShortcutKeys.Activate.Modifiers);

            Assert.NotNull(c.ShortcutKeys.Hide);
            Assert.Equal(0, c.ShortcutKeys.Hide.Count);

            c.ShortcutKeys.Hide = new ObservableCollection<ShortcutKey>
            {
                new ShortcutKey {Key = Key.J, Modifiers = ModifierKeys.Control}
            };
            Assert.Equal(1, c.ShortcutKeys.Hide.Count);
            Assert.Equal(Key.J, c.ShortcutKeys.Hide[0].Key);
            Assert.Equal(ModifierKeys.Control, c.ShortcutKeys.Hide[0].Modifiers);

            c.ShortcutKeys = new ShortcutKeys();
            Assert.Equal(Key.None, c.ShortcutKeys.Activate.Key);
            Assert.Equal(ModifierKeys.None, c.ShortcutKeys.Activate.Modifiers);
            Assert.NotNull(c.ShortcutKeys.Hide);
            Assert.Equal(0, c.ShortcutKeys.Hide.Count);

            Assert.NotNull(c.TargetFolder);
            Assert.True(c.TargetFolder.IsIncludeSystemFolder);
            Assert.True(c.TargetFolder.IsIncludeSystemX86Folder);
            Assert.True(c.TargetFolder.IsIncludeProgramsFolder);
            Assert.True(c.TargetFolder.IsIncludeProgramFilesX86Folder);
            Assert.True(c.TargetFolder.IsIncludeProgramFilesFolder);
            Assert.True(c.TargetFolder.IsIncludeProgramsFolder);
            Assert.True(c.TargetFolder.IsIncludeCommonStartMenu);

            Assert.NotNull(c.TargetFolder.Folders);
            Assert.Equal(0, c.TargetFolder.Folders.Count);

            c.TargetFolder.IsIncludeSystemFolder = false;
            c.TargetFolder.IsIncludeSystemX86Folder = false;
            c.TargetFolder.IsIncludeProgramsFolder = false;
            c.TargetFolder.IsIncludeProgramFilesX86Folder = false;
            c.TargetFolder.IsIncludeProgramFilesFolder = false;
            c.TargetFolder.IsIncludeProgramsFolder = false;
            c.TargetFolder.IsIncludeCommonStartMenu = false;
            Assert.False(c.TargetFolder.IsIncludeSystemFolder);
            Assert.False(c.TargetFolder.IsIncludeSystemX86Folder);
            Assert.False(c.TargetFolder.IsIncludeProgramsFolder);
            Assert.False(c.TargetFolder.IsIncludeProgramFilesX86Folder);
            Assert.False(c.TargetFolder.IsIncludeProgramFilesFolder);
            Assert.False(c.TargetFolder.IsIncludeProgramsFolder);
            Assert.False(c.TargetFolder.IsIncludeCommonStartMenu);

            c.TargetFolder = new TargetFolder();
            Assert.True(c.TargetFolder.IsIncludeSystemFolder);
            Assert.True(c.TargetFolder.IsIncludeSystemX86Folder);
            Assert.True(c.TargetFolder.IsIncludeProgramsFolder);
            Assert.True(c.TargetFolder.IsIncludeProgramFilesX86Folder);
            Assert.True(c.TargetFolder.IsIncludeProgramFilesFolder);
            Assert.True(c.TargetFolder.IsIncludeProgramsFolder);
            Assert.True(c.TargetFolder.IsIncludeCommonStartMenu);
            Assert.NotNull(c.TargetFolder.Folders);
            Assert.Equal(0, c.TargetFolder.Folders.Count);

            c.TargetFolder.Folders = new ObservableCollection<Path>
            {
                new Path("AAA")
            };
            Assert.Equal(1, c.TargetFolder.Folders.Count);
            Assert.Equal("AAA", c.TargetFolder.Folders[0].Value);

            Assert.NotNull(c.PriorityFiles);
            Assert.Equal(0, c.PriorityFiles.Count);

            c.PriorityFiles = new ObservableCollection<Path> {new Path("AA"), new Path("BB")};
            Assert.Equal(2, c.PriorityFiles.Count);
            Assert.Equal("AA", c.PriorityFiles[0].Value);
            Assert.Equal("BB", c.PriorityFiles[1].Value);

            Assert.Equal(8, c.MaxCandidateLinesCount);
            c.MaxCandidateLinesCount = 10;
            Assert.Equal(10, c.MaxCandidateLinesCount);

            Assert.True(c.IsStartOnSystemStartup);
            c.IsStartOnSystemStartup = false;
            Assert.False(c.IsStartOnSystemStartup);

            Assert.NotEqual(string.Empty, c.Culture);
            c.Culture = "ja";
            Assert.Equal("ja", c.Culture);

            Assert.Equal(256, c.IconCacheSize);
            c.IconCacheSize = 0;
            Assert.Equal(0, c.IconCacheSize);

            Assert.Equal(100, c.CandidatesCensoringSize);
            c.CandidatesCensoringSize = 20;
            Assert.Equal(20, c.CandidatesCensoringSize);

            Assert.Equal(
                new[] {"exe", "lnk", "appref-ms", "bat", "cmd", "com", "vbs", "vbe", "js", "jse", "wsf", "wsh"},
                c.ExecutableFileExts);

            c.ExecutableFileExts = new ObservableCollection<string>();
            Assert.Equal(0, c.ExecutableFileExts.Count);

            Assert.Empty(c.GitHubPersonalAccessToken);
            c.GitHubPersonalAccessToken = "AAA";
            Assert.Equal("AAA", c.GitHubPersonalAccessToken);
        }

        [Theory]
        [InlineData("ja")]
        [InlineData("en")]
        public void DefaultCulture(string lang)
        {
            var culture = default(string);

            var th = new Thread(() =>
            {
                CultureInfo.CurrentUICulture = new CultureInfo(lang);

                var c = new Core.Config.App();

                culture = c.Culture;
            });

            th.Start();
            th.Join();

            Assert.Equal(lang, culture);
        }
    }
}