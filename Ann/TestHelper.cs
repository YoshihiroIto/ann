namespace Ann
{
    public static class TestHelper
    {
        public static void CleanTestEnv()
        {
            CultureService.Clean();

            Core.TestHelper.CleanTestEnv();
        }
    }
}