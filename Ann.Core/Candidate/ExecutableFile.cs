using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            string dir,
            string name,
            App app,
            IconDecoder iconDecoder,
            ConcurrentDictionary<string, string> stringPool,
            string[] targetFolders)
        {
            Debug.Assert(app != null);
            Debug.Assert(iconDecoder != null);

            _app = app;
            _iconDecoder = iconDecoder;

            if (name == null)
                name = MakeNameFromFilePath(path);

            Path = stringPool.GetOrAdd(path, path);
            Name = stringPool.GetOrAdd(name, name);

            if (dir == null)
            {
                dir = System.IO.Path.GetDirectoryName(path) ?? string.Empty;
                dir = ShrinkDir(dir, targetFolders);
            }

            Directory = stringPool.GetOrAdd(dir, s => s);
            FileName = stringPool.GetOrAdd(System.IO.Path.GetFileNameWithoutExtension(path), s => s);
            SearchKey = stringPool.GetOrAdd($"{Name}*{Directory}*{FileName}", s => s);

            NameParts = SplitString(Name, Separators, stringPool);
            DirectoryParts = SplitString(Directory, DirectorySeparators, stringPool);
            FileNameParts = SplitString(FileName, Separators, stringPool);
        }

        private async Task RunAsync(bool isRunAsAdmin)
        {
            var i = await _app.RunAsync(Path, isRunAsAdmin);
            if (i)
                return;

            var errMes = new List<StringTags> {StringTags.Message_FailedToStart};
            if (File.Exists(Path) == false)
                errMes.Add(StringTags.Message_FileNotFound);

            _app.NoticeMessages(errMes);
        }

        public ExecutableFile(
            int id, int maxId,
            string path,
            string dir,
            string name,
            App app,
            IconDecoder iconDecoder,
            ConcurrentDictionary<string, string> stringPool,
            string[] targetFolders)
            : this(path, dir, name, app, iconDecoder, stringPool, targetFolders)
        {
            SetId(id, maxId);
        }

        private static string ShrinkDir(string srcDir, string[] targetFolders)
        {
            foreach (var f in targetFolders)
                if (srcDir.StartsWith(f, StringComparison.OrdinalIgnoreCase))
                    return srcDir.Substring(f.Length);

            return srcDir;
        }

        private static string[] SplitString(string src, char[] separators, ConcurrentDictionary<string, string> stringPool)
        {
            if (src.IndexOfAny(separators) == -1)
                return null;

            var parts = src.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length <= 1)
                return null;

            for (var i = 0; i != parts.Length; ++i)
                parts[i] = stringPool.GetOrAdd(parts[i], parts[i]);

            return parts;
        }

        private static string MakeNameFromFilePath(string path)
        {
            var fileDescription = path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                ? FileVersionInfo.GetVersionInfo(path).FileDescription
                : null;

            var name = string.IsNullOrWhiteSpace(fileDescription)
                ? System.IO.Path.GetFileNameWithoutExtension(path)
                : fileDescription;

            return name;
        }

        private static readonly char[] DirectorySeparators = {'\\'};
        private static readonly char[] Separators = {' ', '_', '-', '/', '\\'};

        int IComparable<ExecutableFile>.CompareTo(ExecutableFile other) => _score - other._score;
        string ICandidate.Name => string.IsNullOrWhiteSpace(Name) == false ? Name : System.IO.Path.GetFileName(Path);
        string ICandidate.Comment => Path;
        Brush ICandidate.Icon => _iconDecoder.GetIcon(Path);

        private DelegateCommand _RunCommand;

        ICommand ICandidate.RunCommand
        {
            get
            {
                if (_RunCommand != null)
                    return _RunCommand;

                _RunCommand = new DelegateCommand(async () => await RunAsync(false));

                return _RunCommand;
            }
        }

        private MenuCommand[] _SubCommands;

        MenuCommand[] ICandidate.SubCommands
        {
            get
            {
                if (_SubCommands != null)
                    return _SubCommands;

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
                                await ProcessHelper.RunAsync("EXPLORER", $"/select,\"{Path}\"", false))
                    }
                };

                return _SubCommands;
            }
        }

        bool ICandidate.CanSetPriority => true;

        SelectedBehavior ICandidate.SelectedBehavior => SelectedBehavior.NotAnything;
        string ICandidate.CommandWord => null;
    }
}