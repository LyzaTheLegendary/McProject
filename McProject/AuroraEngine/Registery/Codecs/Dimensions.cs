using System.Text.Json.Serialization;

namespace AuroraEngine.Registery.Codecs
{
    public record struct DimensionElement
    {
        public DimensionElement() { }

        //  How much light the dimension has. When set to 0, it completely follows the light level; when set to 1, there is no ambient lighting. Precise effects need testing
        [JsonPropertyName("ambient_light")]
        public float ambientLight { get; set; } = 0;

        //  Wether a bed can be used in the dimension ( not sure why the client would need to know this! )
        [JsonPropertyName("bed_works")]
        public byte bedWorks { get; set; } = 1;

        //  The multiplier applied to coordinates when leaving the dimension. Value between 0.00001 and 30000000.0 (both inclusive)
        [JsonPropertyName("coordinate_scale")]
        public double coordinateScale { get; set;} = 1;

        //   (optional, defaults to minecraft:overworld) Can be "minecraft:overworld", "minecraft:the_nether" and "minecraft:the_end".
        //   Determines the dimension effect used for this dimension. Setting to overworld makes the dimension have clouds, sun, stars and moon.
        //   Setting to the nether makes the dimension have thick fog blocking that sight, similar to the nether.
        //   Setting to the end makes the dimension have dark spotted sky similar to the end, ignoring the sky and fog color.
        [JsonPropertyName("effects")]
        public string effects { get; set; } = "minecraft:overworld";

        //  Whether the dimension has a bedrock ceiling.
        //  Note that this is only a logical ceiling.
        //  It is unrelated with whether the dimension really has a block ceiling.
        [JsonPropertyName("has_ceiling")]
        public byte hasCeiling { get; set; } = 0;

        //  If it is possible to start raids within villages
        [JsonPropertyName("has_raids")]
        public byte hasRaids { get; set; } = 1;

        // Whether the dimension has skylight or not.
        [JsonPropertyName("has_skylight")]
        public byte hasSkyLight { get; set; } = 1;

        //  The total height in which blocks can exist within this dimension.
        //  Must be between 16 and 4064 and be a multiple of 16.
        //  The maximum building height = min_y + height - 1, which cannot be greater than 2031.
        [JsonPropertyName("height")]
        public short height { get; set; } = 384;

        //  The maximum height to which chorus fruits and nether portals can bring players within this dimension.
        //  This excludes portals that were already built above the limit as they still connect normally.
        //  Cannot be greater than  height.
        [JsonPropertyName("logical_height")]
        public short logicalHeight { get; set; } = 384;

        //  A block tag with #. Fires on these blocks burns infinitely.
        [JsonPropertyName("infiniburn")]
        public string inifiburn { get; set; } = "#minecraft:infiniburn_overworld";

        //  The minimum height in which blocks can exist within this dimension.
        //  Must be between -2032 and 2031 and be a multiple of 16 (effectively making 2016 the maximum).
        [JsonPropertyName("min_y")]
        public short minY { get; set; } = -64;

        //  Value between 0 and 15 (both inclusive). Maximum block light required when the monster spawns.
        [JsonPropertyName("monster_spawn_block_light_limit")]
        public byte monsterSpawnBlockLightLimit { get; set; } = 0;

        //  Value between 0 and 15 (both inclusive).
        //  Maximum light required when the monster spawns.
        //  The formula of this light is: max( skyLight - 10, blockLight ) during thunderstorms, and max( internalSkyLight, blockLight ) during other weather.
        [JsonPropertyName("monster_spawn_light_leve")]
        public MonsterSpawnLightLevel monsterSpawnLightLevel { get; set; } = new MonsterSpawnLightLevel() { Type= "minecraft:uniform", Value = new LightValues() };

        //  When false, compasses spin randomly, and using a bed to set the respawn point or sleep, is disabled. When true, nether portals can spawn zombified piglins.
        [JsonPropertyName("natural")]
        public byte natural { get; set; } = 1;

        //  Whether Piglin and hoglin shake and transform to zombified entities.
        [JsonPropertyName("piglin_safe")]
        public byte piglingSafe { get; set; } = 1;

        //  When false, the respawn anchor blows up when trying to set spawn point.
        [JsonPropertyName("respawn_anchor_works")]
        public byte respawnAnchorWorks { get; set; } = 1;

        //  Whether the dimensions behaves like the nether (water evaporates and sponges dry) or not.
        //  Also lets stalactites drip lava and causes lava to spread faster and thinner.
        [JsonPropertyName("ultrawarm")]
        public byte ultrawarm { get; set; } = 1;
    }
}
public struct worldIndexInfo
{
    [JsonPropertyName("id")]
    public byte id;
    [JsonPropertyName("name")]
    public string name;
}
public struct MonsterSpawnLightLevel
{
    [JsonPropertyName("type")]
    public string type { get; set; }

    [JsonPropertyName("value")]
    public LightValues values { get; set; }
}

public struct LightValues
{
    public LightValues() {}

    [JsonPropertyName("max_inclusive")]
    public int maxInclusive { get; set; } = 7;

    [JsonPropertyName("min_inclusive")]
    public int minInclusive { get; set; } = 0;
}