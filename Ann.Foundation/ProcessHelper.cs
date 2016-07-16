using System.Diagnostics;
using System.Threading.Tasks;

namespace Ann.Foundation
{
    public static class ProcessHelper
    {
        public static async Task Run(string cmd, string args, bool isRunAsAdmin)
        {
            await Task.Run(() =>
            {
                var info = new ProcessStartInfo(cmd)
                {
                    Arguments = args
                };

                if (isRunAsAdmin)
                    info.Verb = "runas";

                Process.Start(info);
            });
        }
    }
}