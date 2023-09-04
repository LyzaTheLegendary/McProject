namespace Common.Helpers
{
    public static class TimeHelper
    {
        public static string GetDMY() => DateTime.Now.ToString("dd/MM/yy");
        public static string GetHMS() => DateTime.Now.ToString("H:m:s");
    }
}
