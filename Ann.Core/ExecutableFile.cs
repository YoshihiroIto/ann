using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Ann.Core.Interface;
using Ann.Foundation;
using Ann.Foundation.Mvvm;

namespace Ann.Core
{
    [DebuggerDisplay("Id:{_id}, MaxId:{_maxId}, Score:{_score}, Path:{Path}")]
    public class ExecutableFile : IComparable<ExecutableFile>, ICandidate
    {
        public readonly string Path;
        public readonly string Name;
        public readonly string LowerName;
        public readonly string LowerDirectory;
        public readonly string LowerFileName;
        public readonly string SearchKey;

        public readonly string[] LowerNameParts;
        public readonly string[] LowerDirectoryParts;
        public readonly string[] LowerFileNameParts;

        //
        private int _id;
        private int _maxId;
        private int _score;

        public void SetScore(int r)
        {
            if (r == int.MaxValue)
                _score = int.MaxValue;
            else
                _score = r*_maxId + _id;
        }

        public void SetId(int id, int maxId)
        {
            _id = id;
            _maxId = maxId;
        }

        private readonly App _app;
        private readonly IconDecoder _iconDecoder;

        public ExecutableFile(
            string path,
            App app,
            IconDecoder iconDecoder,
            ConcurrentDictionary<string, string> stringPool,
            string[] targetFolders)
        {
            Debug.Assert(app != null);
            Debug.Assert(iconDecoder != null);

            _app = app;
            _iconDecoder = iconDecoder;

            var fvi = FileVersionInfo.GetVersionInfo(path);

            var name = string.IsNullOrWhiteSpace(fvi.FileDescription)
                ? System.IO.Path.GetFileNameWithoutExtension(path)
                : fvi.FileDescription;

            Path = stringPool.GetOrAdd(path, path);
            Name = stringPool.GetOrAdd(name, name);
            LowerName = stringPool.GetOrAdd(name.ToLower(), s => s);

            var dir = System.IO.Path.GetDirectoryName(path) ?? string.Empty;
            dir = ShrinkDir(dir, targetFolders);

            LowerDirectory = stringPool.GetOrAdd(dir, s => s);
            LowerFileName = stringPool.GetOrAdd(System.IO.Path.GetFileNameWithoutExtension(path).ToLower(), s => s);
            SearchKey = stringPool.GetOrAdd($"{LowerName}*{LowerDirectory}*{LowerFileName}", s => s);

            LowerNameParts = LowerName.Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => stringPool.GetOrAdd(s, s))
                .ToArray();

            LowerDirectoryParts = LowerDirectory.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => stringPool.GetOrAdd(s, s))
                .ToArray();

            LowerFileNameParts = LowerFileName.Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => stringPool.GetOrAdd(s, s))
                .ToArray();

            if (LowerNameParts.Length <= 1)
                LowerNameParts = null;

            if (LowerDirectoryParts.Length <= 1)
                LowerDirectoryParts = null;

            if (LowerFileNameParts.Length <= 1)
                LowerFileNameParts = null;

            _RunCommand = new DelegateCommand(async () => await RunAsync(false));

            _SubCommands = new[]
            {
                new MenuCommand
                {
                    Caption = StringTags.MenuItem_RunAsAdministrator,
                    Command = new DelegateCommand(async () => await RunAsync(true))
                },
                new MenuCommand
                {
                    Caption = StringTags.MenuItem_OpenContainingFolder,
                    Command = new DelegateCommand(async () =>
                            await ProcessHelper.RunAsync("EXPLORER", $"/select,\"{path}\"", false))
                }
            };
        }

        private async Task RunAsync(bool isRunAsAdmin)
        {
            var i = await _app.RunAsync(Path, isRunAsAdmin);
            if (i)
                return;

            var errMes = new List<StringTags> { StringTags.Message_FailedToStart };
            if (File.Exists(Path) == false)
                errMes.Add(StringTags.Message_FileNotFound);

            _app.NoticeMessages(errMes);
        }

        public ExecutableFile(
            int id, int maxId, string path,
            App app,
            IconDecoder iconDecoder,
            ConcurrentDictionary<string, string> stringPool,
            string[] targetFolders)
            : this(path, app, iconDecoder, stringPool, targetFolders)
        {
            SetId(id, maxId);
        }

        private static string ShrinkDir(string srcDir, IEnumerable<string> targetFolders)
        {
            var srcLower = srcDir.ToLower();

            foreach (var f in targetFolders)
            {
                var ft = f.ToLower().Trim('\\');
                if (srcLower.StartsWith(ft))
                    return srcLower.Substring(ft.Length);
            }

            return srcDir;
        }

        private static readonly char[] Separator = {' ', '_', '-', '/', '\\'};

        int IComparable<ExecutableFile>.CompareTo(ExecutableFile other) => _score - other._score;
        string ICandidate.Name => string.IsNullOrWhiteSpace(Name) == false ? Name : System.IO.Path.GetFileName(Path);
        string ICandidate.Comment => Path;
        ImageSource ICandidate.Icon => _iconDecoder.GetIcon(Path);

        private readonly DelegateCommand _RunCommand;
        ICommand ICandidate.RunCommand => _RunCommand;

        private readonly MenuCommand[] _SubCommands;
        MenuCommand[] ICandidate.SubCommands => _SubCommands;
    }
}