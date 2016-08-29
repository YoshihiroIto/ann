using Ann.Core.Candidate;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test.Candidate
{
    public class TranslateResultTest 
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public TranslateResultTest()
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
                var r = new TranslateResult("ABC", languagesService);

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
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            {
                var r = new TranslateResult("ABC", languagesService) as ICandidate;

                r.RunCommand.Execute(null);
            }
        }
    }
}