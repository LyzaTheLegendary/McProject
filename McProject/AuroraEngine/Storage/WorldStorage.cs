using AuroraEngine.Worlds;
using Common.Console;

namespace Common.AuroraEngine.Storage
{
    /* TODO IMPLEMENT THIS 
When using memory-mapped files in C#, the behavior depends on whether you are using persisted or non-persisted memory-mapped files1.

Persisted memory-mapped files are associated with a source file on a disk. When the last process finishes working with the file, the data is saved to the source file on the disk1. Therefore, if the system crashes due to power loss, the data will be saved to the file on disk.

On the other hand, non-persisted memory-mapped files are not associated with a file on a disk. When the last process finishes working with the file, the data is lost and the file is reclaimed by garbage collection1. In this case, if the system crashes due to power loss, any unsaved data will be lost.

It’s important to choose the appropriate type of memory-mapped file based on your requirements and ensure that your data is safely persisted when necessary1.
     */
    public class WorldStorage
    {
        private string worldRoot;
        private Dimension[] worlds; // instead of an array it could be a dict with dimension name and dimension array?
        public WorldStorage(string worldName) // this should be a method to add worlds to it!
        {
            worldRoot = Path.Combine("Resource", "Worlds");
            if (!Directory.Exists(worldRoot + "/" + worldName))
            {
                Directory.CreateDirectory(worldRoot + "/" + worldName);
                Directory.CreateDirectory($"{worldRoot}/{worldName}/players");
                worldRoot = Path.Combine(worldRoot, worldName);

                CreateWorld(DimensionTypes.OVERWORLD);
                CreateWorld(DimensionTypes.THE_NETHER);
                CreateWorld(DimensionTypes.THE_END);
            }
            else
                worldRoot = Path.Combine(worldRoot, worldName);
            Readworlds();
        }
        public void Readworlds() {
            string[] filePaths = Directory.GetFiles(worldRoot);
            worlds = new Dimension[filePaths.Length];
            int i = 0;

            foreach (string filePath in filePaths) {
                if (!filePath.EndsWith(".dim"))
                    continue;
                // Later on I probably want to give the world their own FileWriter so they can actively edit / save shit 
                BinaryReader reader = new(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Inheritable));

                byte fileVer = reader.ReadByte();
                Dimension dimension = new Dimension(reader);
                worlds[i++] = dimension;
            
            }

            string worldFormat = string.Empty;
            foreach(Dimension dimension in worlds) 
                worldFormat += dimension._dimensionType.ToString() + ", ";
            
            Display.WriteInfo($"Loaded {i} worlds: {worldFormat.TrimEnd(new char[] { ',', ' ' })}");
        }
        public void CreateWorld(DimensionTypes dimensionType)
        { 
            string filename = dimensionType.ToString();
            
            Display.WriteInfo($"Creating {filename}");

            using (BinaryWriter writer = new(File.Create(worldRoot + $"/{filename}.dim"))) {
                writer.Write((byte)1); // version!
                writer.Write((byte)dimensionType);
                writer.Write((byte)WorldTypes.DEFAULT);
            }

            }
        public Dimension[] GetWorlds() => worlds;
    }
}
