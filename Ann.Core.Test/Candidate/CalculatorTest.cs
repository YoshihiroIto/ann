using System;
using Ann.Core.Candidate;
using Ann.Foundation;
using Xunit;

namespace Ann.Core.Test.Candidate
{
    public class CalculatorTest : IDisposable
    {
        private readonly DisposableFileSystem _config = new DisposableFileSystem();

        public CalculatorTest()
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
            using (var app = new App(configHolder, languagesService))
            {
                var c = new Calculator();

                var r = c.Calculate(app, "4*5");

                Assert.Equal(r.Name, "20");
            }
        }

        [Fact]
        public void Error()
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                var c = new Calculator();

                var r = c.Calculate(app, "(123");

                Assert.Null(r);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("a")]
        public void Null(string input)
        {
            var configHolder = new ConfigHolder(_config.RootPath);
            using (var languagesService = new LanguagesService(configHolder.Config))
            using (var app = new App(configHolder, languagesService))
            {
                var c = new Calculator();

                var actual = c.Calculate(app, input);

                Assert.Null(actual);
            }
        }
    }
}