using System.Collections.ObjectModel;
using Xunit;
using System.Windows.Input;
using Ann.Core.Config;
using Ann.Foundation;

namespace Ann.Core.Test.Config
{
    public class AppTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.SetEntryAssembly();

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

            Assert.NotNull(c.TargetFolder);
            Assert.True(c.TargetFolder.IsIncludeSystemFolder);
            Assert.True(c.TargetFolder.IsIncludeSystemX86Folder);
            Assert.True(c.TargetFolder.IsIncludeProgramsFolder);
            Assert.True(c.TargetFolder.IsIncludeProgramFilesX86Folder);
            Assert.True(c.TargetFolder.IsIncludeProgramFilesFolder);
            Assert.True(c.TargetFolder.IsIncludeProgramsFolder);
            Assert.NotNull(c.TargetFolder.Folders);
            Assert.Equal(0, c.TargetFolder.Folders.Count);

            c.TargetFolder.IsIncludeSystemFolder = false;
            c.TargetFolder.IsIncludeSystemX86Folder = false;
            c.TargetFolder.IsIncludeProgramsFolder = false;
            c.TargetFolder.IsIncludeProgramFilesX86Folder = false;
            c.TargetFolder.IsIncludeProgramFilesFolder = false;
            c.TargetFolder.IsIncludeProgramsFolder = false;
            Assert.False(c.TargetFolder.IsIncludeSystemFolder);
            Assert.False(c.TargetFolder.IsIncludeSystemX86Folder);
            Assert.False(c.TargetFolder.IsIncludeProgramsFolder);
            Assert.False(c.TargetFolder.IsIncludeProgramFilesX86Folder);
            Assert.False(c.TargetFolder.IsIncludeProgramFilesFolder);
            Assert.False(c.TargetFolder.IsIncludeProgramsFolder);

           c.TargetFolder.Folders = new ObservableCollection<Path>
           {
               new Path("AAA")
           };
            Assert.Equal(1, c.TargetFolder.Folders.Count);
            Assert.Equal("AAA", c.TargetFolder.Folders[0].Value);

            Assert.NotNull(c.PriorityFiles);
            Assert.Equal(0, c.PriorityFiles.Count);

            Assert.Equal(8, c.MaxCandidateLinesCount);
            Assert.NotEqual(string.Empty, c.Culture);
            Assert.Equal(256, c.IconCacheSize);
            Assert.Equal(100, c.CandidatesCensoringSize);

            Assert.Equal(
                new[] {"exe", "lnk", "appref-ms", "bat", "cmd", "com", "vbs", "vbe", "js", "jse", "wsf", "wsh"},
                c.ExecutableFileExts);

            Assert.Empty(c.GitHubPersonalAccessToken);
        }
    }
}