using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Ann.Foundation.Mvvm;
using Reactive.Bindings.Extensions;
using Squirrel;
using System.Reactive.Disposables;

namespace Ann.Core
{
    public class VersionUpdater : DisposableModelBase
    {
        public static VersionUpdater Instance { get; } = new VersionUpdater();

        public bool IsEnableSilentUpdate { get; }
        public bool IsRestartRequested { get; private set; }
        public bool IsAvailableUpdate { get; private set; }

        public static void Initialize()
        {
        }

        public static void Destory()
        {
            var isRestart = Instance.IsEnableSilentUpdate && Instance.IsRestartRequested;

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
            set { SetProperty(ref _UpdateProgress, value); }
        }

        #endregion

        public void RequestRestart()
        {
            IsRestartRequested = true;
        }

        private bool _isChecking;

        public async Task CheckAsync()
        {
            if (_isChecking)
                return;

            using (Disposable.Create(() => _isChecking = false))
            {
                _isChecking = true;

                if (IsEnableSilentUpdate == false)
                {
                    VersionCheckingState = VersionCheckingStates.Unknown;
                    return;
                }

                if (IsAvailableUpdate && UpdateProgress == 100)
                {
                    VersionCheckingState = VersionCheckingStates.Downloaded;
                    return;
                }

                VersionCheckingState = VersionCheckingStates.Checking;

                try
                {
                    await CheckForUpdate();

                    VersionCheckingState = IsAvailableUpdate
                        ? VersionCheckingStates.Downloading
                        : VersionCheckingStates.Latest;

                    await UpdateApp();
                }
                catch
                {
                    VersionCheckingState = VersionCheckingStates.Unknown;
                }
            }
        }

        private async Task UpdateApp()
        {
            UpdateProgress = 0;

            if (IsEnableSilentUpdate == false)
                return;

            using (var mgr = await UpdateManager.GitHubUpdateManager(Constants.AnnGitHubUrl,
                accessToken: App.Instance.Config.GitHubPersonalAccessToken))
            {
                await mgr.UpdateApp(p => UpdateProgress = p);
                UpdateProgress = 100;
            }
        }

        private async Task CheckForUpdate()
        {
            if (IsEnableSilentUpdate == false)
                return;

            using (var mgr = await UpdateManager.GitHubUpdateManager(Constants.AnnGitHubUrl,
                accessToken: App.Instance.Config.GitHubPersonalAccessToken))
            {
                var updateInfo = await mgr.CheckForUpdate();
                IsAvailableUpdate = updateInfo.CurrentlyInstalledVersion.SHA1 != updateInfo.FutureReleaseEntry.SHA1;
            }
        }

        public async Task CreateStartupShortcut()
        {
            if (IsEnableSilentUpdate == false)
                return;

            using (var mgr = await UpdateManager.GitHubUpdateManager(Constants.AnnGitHubUrl,
                accessToken: App.Instance.Config.GitHubPersonalAccessToken))
            {
                mgr.CreateShortcutsForExecutable("Ann.exe", ShortcutLocation.Startup, false);
            }
        }

        public async Task RemoveStartupShortcut()
        {
            if (IsEnableSilentUpdate == false)
                return;

            using (var mgr = await UpdateManager.GitHubUpdateManager(Constants.AnnGitHubUrl,
                accessToken: App.Instance.Config.GitHubPersonalAccessToken))
            {
                mgr.RemoveShortcutsForExecutable("Ann.exe", ShortcutLocation.Startup);
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
                this.ObserveProperty(x => x.UpdateProgress)
                    .Subscribe(p =>
                    {
                        if (p == 100)
                            if (IsAvailableUpdate)
                                VersionCheckingState = VersionCheckingStates.Downloaded;
                    }).AddTo(CompositeDisposable);
            }
        }
    }
}