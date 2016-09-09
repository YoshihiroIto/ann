using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Ann.Foundation;
using Ann.Foundation.Mvvm;

namespace Ann.Core.Candidate
{
    [DebuggerDisplay("Id:{_id}, MaxId:{_maxId}, Score:{_score}, Path:{Path}")]
    public class ExecutableFile : IComparable<ExecutableFile>, ICandidate
    {
        public readonly string Path;
        public readonly string Name;
        public readonly string Directory;
        public readonly string FileName;
        public readonly string SearchKey;

        public readonly string[] NameParts;
        public readonly string[] DirectoryParts;
        public readonly string[] FileNameParts;

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

            var fileDescription = path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? FileVersionInfo.GetVersionInfo(path).FileDescription : null;

            var name = string.IsNullOrWhiteSpace(fileDescription)
                ? System.IO.Path.GetFileNameWithoutExtension(path)
                : fileDescription;

            Path = stringPool.GetOrAdd(path, path);
            Name = stringPool.GetOrAdd(name, name);

            var dir = System.IO.Path.GetDirectoryName(path) ?? string.Empty;
            dir = ShrinkDir(dir, targetFolders);

            Directory = stringPool.GetOrAdd(dir, s => s);
            FileName = stringPool.GetOrAdd(System.IO.Path.GetFileNameWithoutExtension(path), s => s);
            SearchKey = stringPool.GetOrAdd($"{Name}*{Directory}*{FileName}", s => s);

            NameParts = Name.Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => stringPool.GetOrAdd(s, s))
                .ToArray();

            DirectoryParts = Directory.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => stringPool.GetOrAdd(s, s))
                .ToArray();

            FileNameParts = FileName.Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => stringPool.GetOrAdd(s, s))
                .ToArray();

            if (NameParts.Length <= 1)
                NameParts = null;

            if (DirectoryParts.Length <= 1)
                DirectoryParts = null;

            if (FileNameParts.Length <= 1)
                FileNameParts = null;

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
            foreach (var f in targetFolders)
            {
                var ft = f.Trim('\\');
                if (srcDir.StartsWith(ft))
                    return srcDir.Substring(ft.Length);
            }

            return srcDir;
        }

        private static readonly char[] Separator = {' ', '_', '-', '/', '\\'};

        int IComparable<ExecutableFile>.CompareTo(ExecutableFile other) => _score - other._score;
        string ICandidate.Name => string.IsNullOrWhiteSpace(Name) == false ? Name : System.IO.Path.GetFileName(Path);
        string ICandidate.Comment => Path;
        Brush ICandidate.Icon => _iconDecoder.GetIcon(Path);

        private readonly DelegateCommand _RunCommand;
        ICommand ICandidate.RunCommand => _RunCommand;

        private readonly MenuCommand[] _SubCommands;
        MenuCommand[] ICandidate.SubCommands => _SubCommands;

        bool ICandidate.CanSetPriority => true;

        SelectedBehavior ICandidate.SelectedBehavior => SelectedBehavior.NotAnything;
        string ICandidate.CommandWord => null;
    }
}