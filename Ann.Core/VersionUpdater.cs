﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Ann.Foundation;
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

        private async Task UpdateApp()
        {
            UpdateProgress = 0;

            if (IsEnableSilentUpdate == false)
                return;

            using (var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/YoshihiroIto/Ann",
                accessToken: App.Instance.Config.GitHubPersonalAccessToken))
            {
                await mgr.UpdateApp(p => UpdateProgress = p);
                UpdateProgress = 100;
            }
        }

        #region CheckForUpdateProgress

        private int _CheckForUpdateProgress;

        public int CheckForUpdateProgress
        {
            get { return _CheckForUpdateProgress; }
            set { SetProperty(ref _CheckForUpdateProgress, value); }
        }

        #endregion

        private async Task CheckForUpdate()
        {
            CheckForUpdateProgress = 0;

            if (IsEnableSilentUpdate == false)
                return;

            using (var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/YoshihiroIto/Ann",
                accessToken: App.Instance.Config.GitHubPersonalAccessToken))
            {
                var updateInfo = await mgr.CheckForUpdate(progress: p => CheckForUpdateProgress = p);
                IsAvailableUpdate = updateInfo.CurrentlyInstalledVersion.SHA1 != updateInfo.FutureReleaseEntry.SHA1;
                CheckForUpdateProgress = 100;
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

        private bool _isChecking;

        public async Task CheckAsync()
        {
            if (_isChecking)
                return;

            using (new AnonymousDisposable(() => _isChecking = false))
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
    }
}