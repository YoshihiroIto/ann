using Ann.Properties;
using Xunit;

namespace Ann.Test
{
    public class ResourcesTest
    {
        [Fact]
        public void Text()
        {
            Assert.NotEmpty(Resources.AllFiles);
            Assert.NotEmpty(Resources.Ann_Introduction);
            Assert.NotEmpty(Resources.AutoUpdateStates_CloseAfter0Sec_Restart);
            Assert.NotEmpty(Resources.AutoUpdateStates_CloseAfterNSec);
            Assert.NotEmpty(Resources.Download);
            Assert.NotEmpty(Resources.ExecutableFile);
            Assert.NotEmpty(Resources.File);
            Assert.NotEmpty(Resources.Folder);
            Assert.NotEmpty(Resources.MaxCandidateLines);
            Assert.NotEmpty(Resources.MenuItem_Exit);
            Assert.NotEmpty(Resources.MenuItem_Settings);
            Assert.NotEmpty(Resources.MenuItem_UpdateIndex);
            Assert.NotEmpty(Resources.Message_ActivationShortcutKeyIsAlreadyInUse);
            Assert.NotEmpty(Resources.Message_AlreadySetSameFile);
            Assert.NotEmpty(Resources.Message_AlreadySetSameFolder);
            Assert.NotEmpty(Resources.Message_FailedToStart);
            Assert.NotEmpty(Resources.Message_FileNotFound);
            Assert.NotEmpty(Resources.Message_FolderNotFound);
            Assert.NotEmpty(Resources.Message_InOpening);
            Assert.NotEmpty(Resources.Message_IndexIsNotOpened);
            Assert.NotEmpty(Resources.Message_IndexIsOld);
            Assert.NotEmpty(Resources.Message_IndexNotFound);
            Assert.NotEmpty(Resources.Message_IndexUpdating);
            Assert.NotEmpty(Resources.Restart);
            Assert.NotEmpty(Resources.Settings);
            Assert.NotEmpty(Resources.Settings_About);
            Assert.NotEmpty(Resources.Settings_Activate);
            Assert.NotEmpty(Resources.Settings_AddExecutableFile);
            Assert.NotEmpty(Resources.Settings_AddFolder);
            Assert.NotEmpty(Resources.Settings_AddKey);
            Assert.NotEmpty(Resources.Settings_FrequentlyUsedFolders);
            Assert.NotEmpty(Resources.Settings_General);
            Assert.NotEmpty(Resources.Settings_Hide);
            Assert.NotEmpty(Resources.Settings_KeyStroke);
            Assert.NotEmpty(Resources.Settings_PriorityFiles);
            Assert.NotEmpty(Resources.Settings_Shortcuts);
            Assert.NotEmpty(Resources.Settings_StartOnSystemUpdate);
            Assert.NotEmpty(Resources.Settings_TargetFolders);
            Assert.NotEmpty(Resources.Settings_UserFolders);
            Assert.NotEmpty(Resources.UseOpenSourceLibraries);
            Assert.NotEmpty(Resources.Version);
            Assert.NotEmpty(Resources.VersionChecker_Checking);
            Assert.NotEmpty(Resources.VersionChecker_Downloaded);
            Assert.NotEmpty(Resources.VersionChecker_Downloading);
            Assert.NotEmpty(Resources.VersionChecker_Latest);
            Assert.NotEmpty(Resources.VersionChecker_Unknown);
        }
    }
}
