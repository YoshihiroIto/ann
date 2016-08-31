using System;
using Ann.Core.Candidate;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test.Candidate
{
    public class GoogleSearchResultTest : IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public GoogleSearchResultTest()
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
                var r = new GoogleSearchResult("c#", languagesService, StringTags.GoogleSuggest);

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
                var i = new GoogleSearchResult("c#", languagesService, StringTags.GoogleSuggest) as ICandidate;

                Assert.Equal(app.GetString(StringTags.GoogleSuggest), i.Comment);
                Assert.Equal("c#", i.Name);
                Assert.NotNull(i.RunCommand);
                Assert.Null(i.SubCommands);
                Assert.False(i.CanSetPriority);
            }
        }
    }
}