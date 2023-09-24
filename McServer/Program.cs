using AuroraEngine.Scripting;
using AuroraEngine.Storage;
using AuroraEngine.Worlds;
using Common.Console;
using Common.Setting;
using Gs;
using McAuth;

class Program
{
    // This exists to connect all the servers together, This will make it easier to introduce a Proxy server potentially later on!
    static void Main() 
    {
        Settings.ConstructInstance("Resource/settings");
        Display.Setinstance(new WebDisplay((string)Settings.GetSetting("web_addr"), (int)Settings.GetSetting("web_port"), (string)Settings.GetSetting("web_username"), (string)Settings.GetSetting("web_password")));
        LuaEngine.ConstructInstance(Path.Combine("Resource", "scripts"));
        World.ConstructInstance(new WorldStorage((string)Settings.GetSetting("world_name")).GetWorlds());

        Gameserver gameserver = new Gameserver(0, "test");
        Auth auth = new Auth((string)Settings.GetSetting("auth_addr"), (int)Settings.GetSetting("auth_port"));

        Thread AuthThread = new Thread(() => { auth.Start(gameserver.OnJoin); });
        AuthThread.Start();

        Console.Read();
    }
}