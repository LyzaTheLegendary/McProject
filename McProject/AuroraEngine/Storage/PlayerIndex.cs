
namespace AuroraEngine.Storage
{
    public static class PlayerIndex
    {
        private static object _writeLock = new object();
        public static bool GetPlayerIndex(Int128 uid, out string? username, out bool? isBanned)
        {
            string path = Path.Combine("Resource", "Worlds", "UidIndex",uid.ToString());
            if (!File.Exists(path)) {
                username = null; isBanned = null;
                return false;
            }

            BinaryReader reader = new( File.OpenRead(path) );

            username = reader.ReadString();
            isBanned = reader.ReadBoolean();

            return true;
        }
        public static void CreatePlayerIndex(Int128 uid, string username)
        {
            if (!Directory.Exists(Path.Combine("Resource", "Worlds", "UidIndex")))
                Directory.CreateDirectory(Path.Combine("Resource", "Worlds", "UidIndex"));
            lock (_writeLock) // very slow!
            {
                using (BinaryWriter writer = new(File.Create(Path.Combine("Resource", "Worlds", "UidIndex", uid.ToString()))))
                {
                    writer.Write(username);
                    writer.Write(false); 
                }
            }
        }
    }
}
