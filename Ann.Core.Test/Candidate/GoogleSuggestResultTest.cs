using System;
using Ann.Core.Candidate;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test.Candidate
{
    public class GoogleSuggestResultTest : IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public GoogleSuggestResultTest()
        {
            TestHelper.CleanTestEnv();
        }

        public void Dispose()
        {
            _config.Dispose();
        }

        [Fact]
        public void Basic()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            {
                var r = new GoogleSuggestResult("c#", languagesService);

                // ReSharper disable once IsExpressionAlwaysTrue
                Assert.True(r is ICandidate);
            }
        }

        [Fact]
        public void Interface()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                var i = new GoogleSuggestResult("c#", languagesService) as ICandidate;

                Assert.Equal(app.GetString(StringTags.GoogleSuggest), i.Comment);
                Assert.Equal("c#", i.Name);
                Assert.NotNull(i.RunCommand);
                Assert.Null(i.SubCommands);
                Assert.False(i.CanSetPriority);
            }
        }
    }
}