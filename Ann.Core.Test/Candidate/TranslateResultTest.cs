using System;
using Ann.Core.Candidate;
using Xunit;

namespace Ann.Core.Test.Candidate
{
    public class TranslateResultTest : IDisposable
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
                var r = new TranslateResult("ABC", languagesService);

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
                var i = new TranslateResult("ABC", languagesService) as ICandidate;

                Assert.Equal(app.GetString(StringTags.Translation), i.Comment);
                Assert.Equal("ABC", i.Name);
                Assert.NotNull(i.RunCommand);
                Assert.Null(i.SubCommands);
                Assert.False(i.CanSetPriority);
            }
        }

        [StaFact]
        public void RunCommand()
        {
            var languagesService = _context.GetInstance<LanguagesService>();
            {
                var r = new TranslateResult("ABC", languagesService) as ICandidate;

                r.RunCommand.Execute(null);
            }
        }
    }
}