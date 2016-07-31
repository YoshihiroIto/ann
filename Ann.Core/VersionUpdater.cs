using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Ann.Foundation.Mvvm;
using Squirrel;

namespace Ann.Core
{
    public class VersionUpdater : ModelBase
    {
        public static VersionUpdater Instance { get; } = new VersionUpdater();

        public bool IsEnableSilentUpdate { get; }
        public bool IsRestartRequested { get; private set;}

        public static void Initialize()
        {
        }

        public static void Destory()
        {
            if (Instance.IsEnableSilentUpdate && Instance.IsRestartRequested)
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

        #region CheckForUpdateProgress

        private int _CheckForUpdateProgress;

        public int CheckForUpdateProgress
        {
            get { return _CheckForUpdateProgress; }
            set { SetProperty(ref _CheckForUpdateProgress, value); }
        }

        #endregion

        public void RequestRestart()
        {
            IsRestartRequested = true;
        }

        public async Task UpdateApp()
        {
            UpdateProgress = 0;

            if (IsEnableSilentUpdate == false)
                return;

            using (var mgr = await UpdateManager.GitHubUpdateManager(
                "https://github.com/YoshihiroIto/Ann",
                accessToken:App.Instance.Config.GitHubPersonalAccessToken))
                await mgr.UpdateApp(p => UpdateProgress = p);
        }

        public async Task<bool> CheckForUpdate()
        {
            CheckForUpdateProgress = 0;

            if (IsEnableSilentUpdate == false)
                return false;

            using (var mgr = await UpdateManager.GitHubUpdateManager(
                "https://github.com/YoshihiroIto/Ann",
                accessToken:App.Instance.Config.GitHubPersonalAccessToken))
            {
                var r = await mgr.CheckForUpdate(progress: p => CheckForUpdateProgress = p);

                return r.CurrentlyInstalledVersion.SHA1 != r.FutureReleaseEntry.SHA1;
            }
        }

        private VersionUpdater()
        {
            var dir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var parentDir = System.IO.Path.GetDirectoryName(dir) ?? string.Empty;
            var updaterExe = System.IO.Path.Combine(parentDir, "Update.exe");

            IsEnableSilentUpdate = File.Exists(updaterExe);
        }
    }
}