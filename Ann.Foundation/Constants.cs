using System;

namespace Ann.Foundation
{
    public static class Constants
    {
        public const double IconSize = 48;
        public const int DefaultMaxCandidateLinesCount = 8;

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
    }

    public enum IndexOpeningResults
    {
        Ok,
        NotFound,
        OldIndex,
        CanNotOpen
    }
}