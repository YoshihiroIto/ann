using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace Ann.Foundation
{
    public static class ProcessHelper
    {
        public static async Task<bool> RunAsync(string cmd, string args, bool isRunAsAdmin)
        {
            return await Task.Run(() =>
            {
                var isShortcutFile = string.Equals(Path.GetExtension(cmd), ".lnk", StringComparison.OrdinalIgnoreCase);
                var startInfo = isShortcutFile
                    ? MakeProcessStartInfoForShortcut(cmd)
                    : new ProcessStartInfo(cmd) { Arguments = args };

                if (startInfo == null)
                    return false;

                if (isRunAsAdmin)
                    startInfo.Verb = "runas";

                try
                {
                    Process.Start(startInfo);
                    return true;
                }
                catch
                {
                    // ignored
                }

                return false;
            });
        }

        private static ProcessStartInfo MakeProcessStartInfoForShortcut(string shortcutFilePath)
        {
            // http://stackoverflow.com/questions/19523419/unable-to-launch-shortcut-lnk-files-from-32-bit-c-sharp-application-when-the-f

            var shell = new IWshShell_Class();
            var shortcut = (IWshShortcut_Class)shell.CreateShortcut(shortcutFilePath);

            var cmdPath = shortcut.TargetPath;

            if (File.Exists(cmdPath) == false)
                cmdPath = cmdPath.Replace("SysWOW64", "System32");

            if (File.Exists(cmdPath) == false)
                cmdPath = cmdPath.Replace("Program Files (x86)", "Program Files");

            if (File.Exists(cmdPath) == false)
                return null;

            var startInfo = new ProcessStartInfo(cmdPath)
            {
                WorkingDirectory = shortcut.WorkingDirectory,
                Arguments = shortcut.Arguments
            };

            return startInfo;
        }
    }
}