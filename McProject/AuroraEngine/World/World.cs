using Common;
namespace AuroraEngine.Worlds
{
    public class World
    {
        private static World _instance;
        public static void ConstructInstance(Dimension[] dimensions) => new World(dimensions);
        public static World GetInstance() => _instance ?? throw new mException("World hasn't been set yet, but has been called!");

        private Dimension[] _dimensions;
        private World(Dimension[] dimensions)
        {
            _dimensions = dimensions;
        }
        public static Dimension GetDimension(DimensionTypes type)
            => (from dimension in GetInstance()._dimensions where dimension._dimensionType == type select dimension).First();
        
    }
}
