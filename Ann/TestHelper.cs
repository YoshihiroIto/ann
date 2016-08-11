namespace Ann
{
    public static class TestHelper
    {
        public static void CleanTestEnv()
        {
            ViewManager.Clean();
            CultureService.Clean();

            Core.TestHelper.CleanTestEnv();
        }
    }
}