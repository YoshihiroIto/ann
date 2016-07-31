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

        public static void Initialize()
        {
        }

        public static void Destory()
        {
            var isRestart = Instance.IsEnableSilentUpdate && Instance.IsRestartRequested;

            if (isRestart)
                UpdateManager.RestartApp();
        }

        #region UpdatingStates

        private VersionCheckingStates _VersionCheckingState = VersionCheckingStates.Wait;

        public VersionCheckingStates VersionCheckingState
        {
            get { return _VersionCheckingState; }
            set { SetProperty(ref _VersionCheckingState, value); }
        }

        #endregion

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
                accessToken: App.Instance.Config.GitHubPersonalAccessToken))
            {
                await mgr.UpdateApp(p => UpdateProgress = p);
                UpdateProgress = 100;
            }
        }

        private async Task<bool> CheckForUpdate()
        {
            CheckForUpdateProgress = 0;

            if (IsEnableSilentUpdate == false)
                return false;

            using (var mgr = await UpdateManager.GitHubUpdateManager(
                "https://github.com/YoshihiroIto/Ann",
                accessToken: App.Instance.Config.GitHubPersonalAccessToken))
            {
                var updateInfo = await mgr.CheckForUpdate(progress: p => CheckForUpdateProgress = p);
                CheckForUpdateProgress = 100;
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged(nameof(VersionCheckingState));
                return updateInfo.CurrentlyInstalledVersion.SHA1 != updateInfo.FutureReleaseEntry.SHA1;
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
        }

        public async Task CheckAsync()
        {
            if (VersionCheckingState == VersionCheckingStates.Checking)
                return;

            VersionCheckingState = VersionCheckingStates.Checking;

            try
            {
                if (IsEnableSilentUpdate == false)
                {
                    VersionCheckingState = VersionCheckingStates.Unknown;
                    return;
                }

                if (await CheckForUpdate() == false)
                {
                    VersionCheckingState = VersionCheckingStates.Latest;
                }
                else
                {
                    VersionCheckingState =
                        UpdateProgress == 100
                            ? VersionCheckingStates.Latest
                            : VersionCheckingStates.Old;
                }
            }
            catch
            {
                VersionCheckingState = VersionCheckingStates.Unknown;
            }
        }
    }
}