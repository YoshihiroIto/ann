using System.Threading.Tasks;
using Ann.Foundation.Mvvm;

namespace Ann.Core
{
    public class VersionChecker : ModelBase
    {
        #region UpdatingStates

        private VersionCheckingStates _VersionCheckingState = VersionCheckingStates.Wait;

        public VersionCheckingStates VersionCheckingState
        {
            get { return _VersionCheckingState; }
            set { SetProperty(ref _VersionCheckingState, value); }
        }

        #endregion

        public async Task CheckAsync()
        {
            VersionCheckingState = VersionCheckingStates.Checking;

            try
            {
                if (VersionUpdater.Instance.IsEnableSilentUpdate == false)
                {
                    VersionCheckingState = VersionCheckingStates.Unknown;
                    return;
                }

                VersionCheckingState = await VersionUpdater.Instance.CheckForUpdate()
                    ? VersionCheckingStates.Old
                    : VersionCheckingStates.Latest;
            }
            catch
            {
                VersionCheckingState = VersionCheckingStates.Unknown;
            }
        }

        public async Task DownloadReleases()
        {
            await VersionUpdater.Instance.DownloadReleases();
            await CheckAsync();
        }
    }
}