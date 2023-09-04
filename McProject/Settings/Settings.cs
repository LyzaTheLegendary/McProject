using Common.Console;

namespace Common.Setting
{
    public class Settings
    {
        private static Settings instance;
        public static void ConstructInstance(string path) => instance = new Settings(path);
        public static Settings GetInstance() => instance;
        public static object GetSetting(string key)
        {
            object? value;
            GetInstance().values.TryGetValue(key, out value);
            if (value == null)
                throw new mException("key not found: " + key);
            return value;
        }
        private Dictionary<string, object> values = new Dictionary<string, object>();
        private Settings(string path)
        {
            using (StreamReader stream = File.OpenText(path))
            {
                int lineCount = 0;
                int settingCount = 0;
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine() ?? "//";
                    lineCount++;

                    if (line.StartsWith("//") || line.StartsWith('\n'))
                        continue;

                    string[] lineInfo = line.Split("//")[0].Split(':');
                    if (lineInfo[0] == string.Empty)
                        continue;

                    if (lineInfo.Length != 3)
                        throw new mException($"Invalid line in {path} on line:{lineCount}");

                    if (lineInfo[0]      == "str")
                        values.Add(lineInfo[1], lineInfo[2]);
                    else if (lineInfo[0] == "int")
                        values.Add(lineInfo[1], int.Parse(lineInfo[2]));
                    else if (lineInfo[0] == "float")
                        values.Add(lineInfo[1], float.Parse(lineInfo[2]));
                    else if (lineInfo[0] == "array")
                        values.Add(lineInfo[1], lineInfo[2].Split(','));
                    settingCount++;
                }
                Display.WriteInfo($"Loaded Settings {settingCount}");
            }
        }
    }
}
