using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Ann.Foundation.Mvvm;
using Ann.Foundation.Mvvm.Message;
using Reactive.Bindings.Notifiers;
using Squirrel;

namespace Ann.Core
{
    public class VersionUpdater : ModelBase
    {
        public static VersionUpdater Instance { get; } = new VersionUpdater();

        private readonly bool _isEnableSquirrel;

        public bool IsRestartRequested { get; private set;}

        public static void Initialize()
        {
        }

        public static void Destory()
        {
            if (Instance._isEnableSquirrel && Instance.IsRestartRequested)
                UpdateManager.RestartApp();
        }

        #region UpdateProgress

        private int _UpdateProgress;

        public int UpdateProgress
        {
            get { return _UpdateProgress; }
            set { SetProperty(ref _UpdateProgress, value); }
        }

        #endregion

        public async Task UpdateApp()
        {
            using (var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/YoshihiroIto/Ann"))
                await mgr.UpdateApp(p => UpdateProgress = p);
        }

        private VersionUpdater()
        {
            var dir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var parentDir = System.IO.Path.GetDirectoryName(dir) ?? string.Empty;
            var updaterExe = System.IO.Path.Combine(parentDir, "Update.exe");

            _isEnableSquirrel = Directory.Exists(updaterExe);
        }

        public void RequestRestart()
        {
            IsRestartRequested = true;
        }
    }
}