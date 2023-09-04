namespace Common.Helpers
{
    public static class LogHelper
    {
        public static void DisplayLog(string text)
        {
            FileStream stream = File.OpenWrite("test");
            stream.Close();
        }
    }
}
