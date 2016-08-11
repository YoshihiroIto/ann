namespace Ann
{
    public static class TestHelper
    {
        public static void CleanTestEnv()
        {
            ViewManager.Clean();
            Core.TestHelper.CleanTestEnv();
        }
    }
}