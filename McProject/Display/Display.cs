using Common.Helpers;

namespace Common.Console
{
    public class Display
    {
        private static DisplayInterface? _instance;
        public static void Setinstance(DisplayInterface instance) => _instance = instance;
        public static void Write(string text)
        {
            System.Console.WriteLine(text);
            _instance?.Write(text);
        }
        public static void WriteText(string text)
        {
            System.Console.WriteLine(TimeHelper.GetHMS() + ':'+ text);
            _instance?.Text(text);
        }
        public static void WriteInfo(string text)
        {
            System.Console.WriteLine(TimeHelper.GetHMS() + ":(Info):" + text);
            _instance?.Info(text);
        }
        public static void WriteWarning(string text)
        {
            System.Console.WriteLine(TimeHelper.GetHMS() + ":(Warning):" + text);
            _instance?.Warn(text);
        }
        public static void WriteError(string text)
        {
            System.Console.WriteLine(TimeHelper.GetHMS() + ":(Error):" + text);
            _instance?.Error(text);
        }
        public static void WriteFatal(string text)
        {
            System.Console.WriteLine(TimeHelper.GetHMS() + ":(Fatal):" + text);
            _instance?.Fatal(text);
        }
        public static void WriteLink(string text)
        {
            System.Console.WriteLine(TimeHelper.GetHMS() + ":(Link):" + text);
            _instance?.Link(text);
        }
    }
}
