using AuroraEngine.Utils;

namespace AuroraEngine.GameObjects
{
    public abstract class AuObject
    {
        protected ArUid _id;
        public ArUid GetID() => _id;
        public AuObject(ArUid id) => _id = id;
        public AuObject() => _id = ArUid.GetNewUID();
        

    }
}
