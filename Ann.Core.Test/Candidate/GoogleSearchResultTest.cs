using System;
using Ann.Core.Candidate;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test.Candidate
{
    public class GoogleSearchResultTest : IDisposable
    {
        private readonly TestContext _context = new TestContext();

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Basic()
        {
            var languagesService = _context.GetInstance<LanguagesService>();
            {
                var r = new GoogleSearchResult("c#", languagesService, StringTags.GoogleSuggest);

                // ReSharper disable once IsExpressionAlwaysTrue
                Assert.True(r is ICandidate);
            }
        }

        [Fact]
        public void Interface()
        {
            var app = _context.GetInstance<App>();
            var languagesService = _context.GetInstance<LanguagesService>();
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