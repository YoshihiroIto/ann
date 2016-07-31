using System.Threading.Tasks;
using Ann.Foundation.Mvvm;
using Squirrel;

namespace Ann.Core
{
    public class VersionUpdater : ModelBase
    {
        public static VersionUpdater Instance { get; } = new VersionUpdater();

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
            using (var mgr = UpdateManager.GitHubUpdateManager("https://github.com/YoshihiroIto/Ann"))
                await mgr.Result.UpdateApp(p => UpdateProgress = p);
        }

        private VersionUpdater() { }
    }
}