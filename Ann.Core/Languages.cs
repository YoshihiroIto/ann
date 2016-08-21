//------------------------------------------------------------------------------
// <auto-generated>
// </auto-generated>
//------------------------------------------------------------------------------
using System;

namespace Ann.Core
{
public enum Languages
{
en,
ja
}

public enum StringTags
{
AllFiles,
Ann_Introduction,
AutoUpdateStates_CloseAfter0Sec_Restart,
AutoUpdateStates_CloseAfterNSec,
Download,
ExecutableFile,
File,
Folder,
MaxCandidateLines,
MenuItem_Exit,
MenuItem_Settings,
MenuItem_UpdateIndex,
Message_ActivationShortcutKeyIsAlreadyInUse,
Message_AlreadySetSameFile,
Message_AlreadySetSameFolder,
Message_AlreadySetSameKeyStroke,
Message_FailedToStart,
Message_FileNotFound,
Message_FolderNotFound,
Message_IndexIsNotOpened,
Message_IndexIsOld,
Message_IndexNotFound,
Message_IndexUpdating,
Message_InOpening,
Restart,
Settings,
Settings_About,
Settings_Activate,
Settings_AddExecutableFile,
Settings_AddFolder,
Settings_AddKey,
Settings_FrequentlyUsedFolders,
Settings_General,
Settings_Hide,
Settings_KeyStroke,
Settings_PriorityFiles,
Settings_Shortcuts,
Settings_StartOnSystemUpdate,
Settings_TargetFolders,
Settings_UserFolders,
UseOpenSourceLibraries,
Version,
VersionChecker_Checking,
VersionChecker_Downloaded,
VersionChecker_Downloading,
VersionChecker_Latest,
VersionChecker_Unknown
}

public static class Localization
{
    public static string GetString(Languages language, StringTags tag)
{
switch(language)
{
case Languages.en:
switch(tag)
{
case StringTags.AllFiles: return "All files";
case StringTags.Ann_Introduction: return "Simple implementation commandline launcher";
case StringTags.AutoUpdateStates_CloseAfter0Sec_Restart: return "Restart.";
case StringTags.AutoUpdateStates_CloseAfterNSec: return "Update is now ready. Restart after {0} seconds.";
case StringTags.Download: return "Download";
case StringTags.ExecutableFile: return "Executable file";
case StringTags.File: return "File";
case StringTags.Folder: return "Folder";
case StringTags.MaxCandidateLines: return "Max Candidate Lines";
case StringTags.MenuItem_Exit: return "Exit";
case StringTags.MenuItem_Settings: return "Settings...";
case StringTags.MenuItem_UpdateIndex: return "Update Index";
case StringTags.Message_ActivationShortcutKeyIsAlreadyInUse: return "Activation Shortcut key is already in use.";
case StringTags.Message_AlreadySetSameFile: return "Already set same file.";
case StringTags.Message_AlreadySetSameFolder: return "Already set same folder.";
case StringTags.Message_AlreadySetSameKeyStroke: return "Already set same key stroke.";
case StringTags.Message_FailedToStart: return "Failed to start.";
case StringTags.Message_FileNotFound: return "File not found.";
case StringTags.Message_FolderNotFound: return "Folder not found.";
case StringTags.Message_IndexIsNotOpened: return "Cannot open the index.";
case StringTags.Message_IndexIsOld: return "Index is old. Update the index.";
case StringTags.Message_IndexNotFound: return "Index not found. Update the index.";
case StringTags.Message_IndexUpdating: return "Index Updating...{0}";
case StringTags.Message_InOpening: return "Index Initializing...";
case StringTags.Restart: return "Restart";
case StringTags.Settings: return "Settings";
case StringTags.Settings_About: return "About";
case StringTags.Settings_Activate: return "Activate";
case StringTags.Settings_AddExecutableFile: return "Add executable file";
case StringTags.Settings_AddFolder: return "Add folder";
case StringTags.Settings_AddKey: return "Add key";
case StringTags.Settings_FrequentlyUsedFolders: return "Frequently used folders";
case StringTags.Settings_General: return "General";
case StringTags.Settings_Hide: return "Hide";
case StringTags.Settings_KeyStroke: return "Key Stroke";
case StringTags.Settings_PriorityFiles: return "Priority Files";
case StringTags.Settings_Shortcuts: return "Shortcuts";
case StringTags.Settings_StartOnSystemUpdate: return "Start on system update";
case StringTags.Settings_TargetFolders: return "Target Folders";
case StringTags.Settings_UserFolders: return "User Folders";
case StringTags.UseOpenSourceLibraries: return "Use Open Source Libraries";
case StringTags.Version: return "Version";
case StringTags.VersionChecker_Checking: return "Checking for updates.";
case StringTags.VersionChecker_Downloaded: return "Download was completed. Version up at after restart.";
case StringTags.VersionChecker_Downloading: return "Now downloading files...";
case StringTags.VersionChecker_Latest: return "Ann is up to date.";
case StringTags.VersionChecker_Unknown: return "Cannot confirm the version update. Check Internet connection.";

default:
    throw new NotImplementedException();
}
case Languages.ja:
switch(tag)
{
case StringTags.AllFiles: return "すべてのファイル";
case StringTags.Ann_Introduction: return "シンプルなコマンドラインランチャー";
case StringTags.AutoUpdateStates_CloseAfter0Sec_Restart: return "再起動します。";
case StringTags.AutoUpdateStates_CloseAfterNSec: return "更新の準備が整いました。{0}秒後に再起動します。";
case StringTags.Download: return "ダウンロード";
case StringTags.ExecutableFile: return "実行可能ファイル";
case StringTags.File: return "ファイル";
case StringTags.Folder: return "フォルダー";
case StringTags.MaxCandidateLines: return "表示候補数";
case StringTags.MenuItem_Exit: return "終了";
case StringTags.MenuItem_Settings: return "設定...";
case StringTags.MenuItem_UpdateIndex: return "インデックスの更新";
case StringTags.Message_ActivationShortcutKeyIsAlreadyInUse: return "アクティブ化ショートカットキーはすでに使われています。";
case StringTags.Message_AlreadySetSameFile: return "すでに同じファイルが設定済みです。";
case StringTags.Message_AlreadySetSameFolder: return "すでに同じフォルダーが設定済みです。";
case StringTags.Message_AlreadySetSameKeyStroke: return "すでに同じキーストロークが設定済みです。";
case StringTags.Message_FailedToStart: return "起動に失敗しました。";
case StringTags.Message_FileNotFound: return "ファイルが見つかりません。";
case StringTags.Message_FolderNotFound: return "フォルダーが見つかりません。";
case StringTags.Message_IndexIsNotOpened: return "インデックスが開けません。";
case StringTags.Message_IndexIsOld: return "インデックスが古いです。インデックスを更新してください。";
case StringTags.Message_IndexNotFound: return "インデックスが見つかりません。インデックスを更新してください。";
case StringTags.Message_IndexUpdating: return "インデックス更新中...{0}";
case StringTags.Message_InOpening: return "インデックス初期化中...";
case StringTags.Restart: return "再起動";
case StringTags.Settings: return "設定";
case StringTags.Settings_About: return "概要";
case StringTags.Settings_Activate: return "アクティブ化";
case StringTags.Settings_AddExecutableFile: return "実行可能ファイル追加";
case StringTags.Settings_AddFolder: return "フォルダー追加";
case StringTags.Settings_AddKey: return "キー追加";
case StringTags.Settings_FrequentlyUsedFolders: return "よく使われるフォルダー";
case StringTags.Settings_General: return "全般";
case StringTags.Settings_Hide: return "非表示";
case StringTags.Settings_KeyStroke: return "キーストローク";
case StringTags.Settings_PriorityFiles: return "優先ファイル";
case StringTags.Settings_Shortcuts: return "ショートカット";
case StringTags.Settings_StartOnSystemUpdate: return "システム起動時に起動する";
case StringTags.Settings_TargetFolders: return "対象フォルダー";
case StringTags.Settings_UserFolders: return "フォルダー";
case StringTags.UseOpenSourceLibraries: return "利用オープンソースライブラリ";
case StringTags.Version: return "バージョン";
case StringTags.VersionChecker_Checking: return "新しいバージョンを確認しています。";
case StringTags.VersionChecker_Downloaded: return "ダウンロード完了しました。再起動時にバージョンアップします。";
case StringTags.VersionChecker_Downloading: return "ダウンロード中です...";
case StringTags.VersionChecker_Latest: return "最新バージョンです。";
case StringTags.VersionChecker_Unknown: return "バージョン更新の確認ができません。インターネット接続を確認してください。";

default:
    throw new NotImplementedException();
}

default:
    throw new NotImplementedException();
}
}
}


}