namespace AuroraEngine.Worlds
{
    public enum DimensionTypes : byte
    {
        OVERWORLD,
        THE_NETHER,
        THE_END,
    }
    public enum WorldTypes : byte
    {
        DEFAULT,
    }
    public class Dimension
    {
        public readonly DimensionTypes _dimensionType;
        public readonly WorldTypes _worldType;
        private readonly BinaryReader _dimensionStream; // I'm working on the reading part, I don't know yet how I'll do the writing part!

        //worldInfoJson = $"  'minecraft:dimension_type': {{\r\n    type: 'minecraft:dimension_type',\r\n    value: [\r\n      {{\r\n        element: {{\r\n          ambient_light: 0,\r\n          bed_works: 1,\r\n          coordinate_scale: 1,\r\n          effects: 'minecraft:overworld',\r\n          has_ceiling: 0,\r\n          has_raids: 1,\r\n          has_skylight: 1,\r\n          height: 384,\r\n          infiniburn: '#minecraft:infiniburn_overworld',\r\n          logical_height: 384,\r\n          min_y: -64,\r\n          monster_spawn_block_light_limit: 0,\r\n          monster_spawn_light_level: {{\r\n            type: 'minecraft:uniform',\r\n            value: {{\r\n              max_inclusive: 7,\r\n              min_inclusive: 0,\r\n            }},\r\n          }},\r\n          natural: 1,\r\n          piglin_safe: 0,\r\n          respawn_anchor_works: 0,\r\n          ultrawarm: 0,\r\n        }},\r\n        id: 0,\r\n        name: 'minecraft:overworld',\r\n      }}";
        public Dimension(BinaryReader dimensionStream) {
            _dimensionStream = dimensionStream;
            _dimensionType = (DimensionTypes)_dimensionStream.ReadByte();
            _worldType = (WorldTypes)_dimensionStream.ReadByte();
        }
        // should be read from the file!
        public object GetChunk(int x, int y, int z) => throw new NotImplementedException("TODO figure out how to implement chunks!");
        public void SaveWorld() => throw new NotImplementedException();
        ~Dimension() {
            _dimensionStream.Close();
        }
        //public string GetWorldInfo() => worldInfoJson;
    }
}
