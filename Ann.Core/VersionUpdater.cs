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
        public bool IsRestartRequested { get; private set; }

        private readonly Task<UpdateManager> _UpdateManager;

        public static void Initialize()
        {
        }

        public static void Destory()
        {
            var isRestart = Instance.IsEnableSilentUpdate && Instance.IsRestartRequested;

            Instance?._UpdateManager?.Result.Dispose();

            if (isRestart)
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

        #region DownloadReleasesProgress

        private int _DownloadReleasesProgress;

        public int DownloadReleasesProgress
        {
            get { return _DownloadReleasesProgress; }
            set { SetProperty(ref _DownloadReleasesProgress, value); }
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

            var mgr = await _UpdateManager;
            await mgr.UpdateApp(p => UpdateProgress = p);
            UpdateProgress = 100;
        }

        public async Task<bool> CheckForUpdate()
        {
            CheckForUpdateProgress = 0;

            if (IsEnableSilentUpdate == false)
                return false;

            var mgr = await _UpdateManager;
            {
                var updateInfo = await mgr.CheckForUpdate(progress: p => CheckForUpdateProgress = p);
                CheckForUpdateProgress = 100;
                return updateInfo.CurrentlyInstalledVersion.SHA1 != updateInfo.FutureReleaseEntry.SHA1;
            }
        }

        public async Task DownloadReleases()
        {
            DownloadReleasesProgress = 0;

            if (IsEnableSilentUpdate == false)
                return;

            var mgr = await _UpdateManager;
            {
                var updateInfo = await mgr.CheckForUpdate(progress: p => DownloadReleasesProgress = p);

                await mgr.DownloadReleases(
                    updateInfo.ReleasesToApply,
                    p => DownloadReleasesProgress = p);

                DownloadReleasesProgress = 100;
            }
        }

        private VersionUpdater()
        {
            {
                var dir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var parentDir = System.IO.Path.GetDirectoryName(dir) ?? string.Empty;
                var updaterExe = System.IO.Path.Combine(parentDir, "Update.exe");

                IsEnableSilentUpdate = File.Exists(updaterExe);
            }

            if (IsEnableSilentUpdate)
            {
                _UpdateManager =
                    UpdateManager.GitHubUpdateManager(
                        "https://github.com/YoshihiroIto/Ann",
                        accessToken: App.Instance.Config.GitHubPersonalAccessToken);
            }
        }
    }
}