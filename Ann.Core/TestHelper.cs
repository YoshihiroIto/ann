using System;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Threading;
using Ann.Foundation;
using Reactive.Bindings;

namespace Ann.Core
{
    public static class TestHelper
    {
        public static void CleanTestEnv()
        {
            if (Assembly.GetEntryAssembly() == null)
                Foundation.TestHelper.SetEntryAssembly();

            App.Clean();
            VersionUpdater.Clean();
            ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
            App.RemoveIndexFile();
            DeleteTestConfigs();
        }

        private static void DeleteTestConfigs()
        {
            var categories = Enum.GetValues(typeof(ConfigHelper.Category)).Cast<ConfigHelper.Category>();

            foreach (var category in categories)
            {
                var path = ConfigHelper.MakeFilePath(category, Constants.ConfigDirPath);

                while(true)
                {
                    try
                    {
                        File.Delete(path);
                        if (File.Exists(path) == false)
                            break;
                    }
                    catch
                    {
                        // ignored
                    }

                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }
            }
        }
    }
}