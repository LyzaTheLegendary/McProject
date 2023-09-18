using Common;
using Common.Console;
using NLua;

namespace AuroraEngine.Scripting
{
    
    public class LuaEngine
    {
        public static void ConstructInstance(string path) => _instance = new(path);
        public static LuaEngine GetInstance() => _instance ?? throw new Exception("Lua engine was called before being set");
        private static LuaEngine _instance;



        private Dictionary<string,string> _luaScripts = new();
        private LuaEngine(string scriptDirectory) {
            string[] luaScripts = Directory.GetFiles(scriptDirectory);
            foreach( string script  in luaScripts )
            {
                string[] scriptInfo = script.Split('\\');
                _luaScripts.Add(scriptInfo[scriptInfo.Length - 1].Split('.')[0], File.ReadAllText(script));
            }
            Display.WriteInfo($"Loaded {_luaScripts.Count} scripts into memory");
        }
        public static void ExecuteScript(string scriptName)
        {
            string? function;
            GetInstance()._luaScripts.TryGetValue(scriptName, out function);

            if (function == null)
                throw new mException("Script name given does not exist!");
            Lua lua = new();

            lua.RegisterFunction("WriteInfo", null, typeof(Display).GetMethod("WriteInfo"));
            lua.RegisterFunction("WriteError", null, typeof(Display).GetMethod("WriteError"));
            lua.RegisterFunction("WriteWarning", null, typeof(Display).GetMethod("WriteWarning"));

            lua.DoString(function);
        }
    }
}
