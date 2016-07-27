using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ann.Foundation.Mvvm;
using Octokit;

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

        public async Task Check()
        {
            VersionCheckingState = VersionCheckingStates.Checking;

            var github = new GitHubClient(new ProductHeaderValue("Ann"));
            var latest = await github.Repository.Release.GetLatest("YoshihiroIto", "Ann");

            var r = new Regex("[0-9]+.[0-9]+.[0-9]+");
            var m = r.Match(latest.Name);

            if (m.Success == false)
            {
                VersionCheckingState = VersionCheckingStates.Unknown;
                return;
            }

            var originVersion = m.Groups[0].Value + ".0";
            var thisVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            VersionCheckingState = originVersion == thisVersion ? VersionCheckingStates.Latest : VersionCheckingStates.Old;
        }
    }
}