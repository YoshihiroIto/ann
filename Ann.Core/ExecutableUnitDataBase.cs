using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Ann.Core
{
    public class ExecutableUnitDataBase : IDisposable
    {
        public event EventHandler Opend;
        public event EventHandler Closed;

        private bool _isOpend;

        private readonly string _indexFile;

        public ExecutableUnitDataBase(string indexFile)
        {
            _indexFile = indexFile;
            Opend += (_, __) => _isOpend = true;
            Closed += (_, __) => _isOpend = false;
        }

        public void Dispose()
        {
            Close();
        }

        public IEnumerable<ExecutableUnit> Find(string name)
        {
            if (name == null)
                return Enumerable.Empty<ExecutableUnit>();

            if (_isOpend == false)
                return Enumerable.Empty<ExecutableUnit>();

            name = name.Trim();

            if (name == string.Empty)
                return Enumerable.Empty<ExecutableUnit>();

            name = name.ToLower();

            return
                _executableUnits
                    .Where(u => u.Name.ToLower().Contains(name) ||
                                u.Directory.ToLower().Contains(name) ||
                                u.FileName.ToLower().Contains(name))
                    .OrderBy(u => MakeRank(u, name));
        }

        private ExecutableUnit[] _executableUnits;

        public async Task UpdateIndexAsync(IEnumerable<string> targetFolders)
        {
            _executableUnits = await Crawler.ExecuteAsync(targetFolders);

            using (var writer = new StringWriter())
            {
                new Serializer(SerializationOptions.EmitDefaults).Serialize(writer, _executableUnits);

                var dir = Path.GetDirectoryName(_indexFile);
                if (dir != null)
                    Directory.CreateDirectory(dir);

                File.WriteAllText(_indexFile, writer.ToString());
            }

            Opend?.Invoke(this, EventArgs.Empty);
        }

        public async Task OpenAsync()
        {
            await Task.Run(() =>
            {
                if (File.Exists(_indexFile) == false)
                    return;

                using (var reader = new StringReader(File.ReadAllText(_indexFile)))
                    _executableUnits = new Deserializer().Deserialize<ExecutableUnit[]>(reader);

                Opend?.Invoke(this, EventArgs.Empty);
            });
        }

        private void Close()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        // ReSharper disable PossibleNullReferenceException
        private static int MakeRank(ExecutableUnit u, string name)
        {
            var unitNameLength = Math.Min(u.Name.Length, 9999);

            Func<int, string, int> makeRankSub = (rankBase, target) =>
            {
                target = target.ToLower();

                if (target == name)
                    return (rankBase + 0)*10000 + unitNameLength;

                var parts = target.Split(new[] {' ', '_', '-', '/', '\\'}, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Any(t => t.StartsWith(name)))
                    return (rankBase + 1)*10000 + unitNameLength;

                if (target.StartsWith(name))
                    return (rankBase + 2)*10000 + unitNameLength;

                if (target.Contains(name))
                    return (rankBase + 3)*10000 + unitNameLength;

                return int.MaxValue;
            };

            var b = 0;

            var rankFileName = makeRankSub(++b, u.FileName);
            if (rankFileName != int.MaxValue)
                return rankFileName;

            var rankName = makeRankSub(++b, u.Name);
            if (rankName != int.MaxValue)
                return rankName;

            var rankDir = makeRankSub(++b, u.Directory);
            if (rankDir != int.MaxValue)
                return rankDir;

            return int.MaxValue;
        }

        // ReSharper restore PossibleNullReferenceException
    }
}