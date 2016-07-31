using System.Diagnostics;
using System.Threading.Tasks;

namespace Ann.Foundation
{
    public static class ProcessHelper
    {
        public static async Task RunAsync(string cmd, string args, bool isRunAsAdmin)
        {
            await Task.Run(() =>
            {
                var info = new ProcessStartInfo(cmd)
                {
                    Arguments = args
                };

                if (isRunAsAdmin)
                    info.Verb = "runas";

                try
                {
                    Process.Start(info);
                }
                catch
                {
                    // ignored
                }
            });
        }
    }
}