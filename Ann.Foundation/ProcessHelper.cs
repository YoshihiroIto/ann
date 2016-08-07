using System.Diagnostics;
using System.Threading.Tasks;

namespace Ann.Foundation
{
    public static class ProcessHelper
    {
        public static async Task<bool> RunAsync(string cmd, string args, bool isRunAsAdmin)
        {
            return await Task.Run(() =>
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
                    return true;
                }
                catch
                {
                    // ignored
                }

                return false;
            });
        }
    }
}