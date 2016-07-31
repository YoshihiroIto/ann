using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;
using Squirrel;

namespace Ann.Core
{
    public class VersionUpdater : DisposableModelBase
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

            Instance?._UpdateManager?.Dispose();
            Instance.Dispose();

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
            set
            {
                if (SetProperty(ref _UpdateProgress, value))
                    // ReSharper disable once ExplicitCallerInfoArgument
                    RaisePropertyChanged(nameof(VersionCheckingState));
            }
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

        #region IsAvailableUpdate

        private bool _IsAvailableUpdate;

        public bool IsAvailableUpdate
        {
            get { return _IsAvailableUpdate; }
            set { SetProperty(ref _IsAvailableUpdate, value); }
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
            {
                await mgr.UpdateApp(p => UpdateProgress = p);
                UpdateProgress = 100;
            }
        }

        private async Task CheckForUpdate()
        {
            CheckForUpdateProgress = 0;

            if (IsEnableSilentUpdate == false)
            {
                IsAvailableUpdate = false;
                return;
            }

            var mgr = await _UpdateManager;
            {
                var updateInfo = await mgr.CheckForUpdate(progress: p => CheckForUpdateProgress = p);
                CheckForUpdateProgress = 100;

                IsAvailableUpdate = updateInfo.CurrentlyInstalledVersion.SHA1 != updateInfo.FutureReleaseEntry.SHA1;
            }
        }

        private VersionUpdater()
        {
            var dir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var parentDir = System.IO.Path.GetDirectoryName(dir) ?? string.Empty;
            var updaterExe = System.IO.Path.Combine(parentDir, "Update.exe");

            IsEnableSilentUpdate = File.Exists(updaterExe);

            if (IsEnableSilentUpdate)
            {
                _UpdateManager =
                    UpdateManager.GitHubUpdateManager(
                        "https://github.com/YoshihiroIto/Ann",
                        accessToken: App.Instance.Config.GitHubPersonalAccessToken);

                this.ObserveProperty(x => x.IsAvailableUpdate)
                .Subscribe(i => VersionCheckingState = i ? VersionCheckingStates.Old : VersionCheckingStates.Latest)
                .AddTo(CompositeDisposable);
            }
        }

        public async Task CheckAsync()
        {
            if (VersionCheckingState == VersionCheckingStates.Checking)
                return;

            if (IsEnableSilentUpdate == false)
            {
                VersionCheckingState = VersionCheckingStates.Unknown;
                return;
            }

            VersionCheckingState = VersionCheckingStates.Checking;

            try
            {
                await CheckForUpdate();

                VersionCheckingState = IsAvailableUpdate ? VersionCheckingStates.Old : VersionCheckingStates.Latest;
            }
            catch
            {
                VersionCheckingState = VersionCheckingStates.Unknown;
            }
        }
    }
}