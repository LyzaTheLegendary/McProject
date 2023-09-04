using Common.Console;
using Common.Setting;
using McAuth;

class Program
{
    static void Main()
    {
        Settings.ConstructInstance("settings");
        Display.Setinstance(new WebDisplay((string)Settings.GetSetting("web_addr"), (int)Settings.GetSetting("web_port"), (string)Settings.GetSetting("web_username"), (string)Settings.GetSetting("web_password")));

        Auth auth = new Auth((string)Settings.GetSetting("auth_addr"), (int)Settings.GetSetting("auth_port"));
        auth.Start();

        Console.Read();
    }
}