using System;
using System.Linq;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test
{
    public class ConstantsTest
    {
        [Fact]
        public void Basic()
        {
            TestHelper.CleanTestEnv();

            Assert.Contains("github.com", Constants.AnnGitHubUrl);
            Assert.Contains("twitter.com", Constants.AnnTwitterUrl);

            Assert.Equal(Environment.GetFolderPath(Environment.SpecialFolder.System), Constants.SystemFolder);
            Assert.Equal(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), Constants.SystemX86Folder);
            Assert.Equal(Environment.GetFolderPath(Environment.SpecialFolder.Programs), Constants.ProgramsFolder);

            Assert.Contains("Program Files", Constants.ProgramFilesFolder);
            Assert.Contains("Program Files", Constants.ProgramFilesX86Folder);

            var lruCache = Constants.OpenSources.Single(s => s.Name == "LRU Cache");
            Assert.Contains("Yoshihiro Ito", lruCache.Auther);

            var squirrel = Constants.OpenSources.Single(s => s.Name == "Squirrel for Windows");
            Assert.Contains("GitHub", squirrel.Auther);

            Assert.Contains(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.ConfigDirPath);
            Assert.Contains(AssemblyConstants.Company, Constants.ConfigDirPath);
            Assert.Contains(AssemblyConstants.Product, Constants.ConfigDirPath);
        }
    }
}