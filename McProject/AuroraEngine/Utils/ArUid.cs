using Common.Helpers;
using Common;
namespace AuroraEngine.Utils
{
    public class ArUid
    {
        public static Queue<uint> idQueue = new Queue<uint>();
        public static List<uint> activeIds = new List<uint>();
        public static uint highestId = 1;
        private static uint GetNewId()
        {
            uint id;
            lock (idQueue)
                if (idQueue.TryDequeue(out id))
                    return id;
                else
                    highestId++;

            id = highestId;

            lock (activeIds)
                activeIds.Add(id);

            

            return id;
        }

        public static ArUid GetNewUID() => new ArUid();

        protected uint id;
        private ArUid()
        {
            id = GetNewId();
        }
        private ArUid(uint id)
        {
            this.id = id;
        }

        ~ArUid() 
        {
            lock(activeIds)
                activeIds.Remove(id);
            lock (idQueue)
                idQueue.Enqueue(id); 
        }

        //  (original << bits) | (original >> (32 - bits)) https://stackoverflow.com/questions/812022/c-sharp-bitwise-rotate-left-and-rotate-right
        public static explicit operator byte[] (ArUid uid)
        {
            //BigInteger id = (msg.id << 8) | (msg.id >> (16 - 8));
            uid.id = uid.id.RotateRight(8); // I could assign 2 longs here the first one for the bits that should be shifted and 2nd one for the actual number!
            return MarshalHelper.StructureToBytes(uid.id);
        }

        public static implicit operator ArUid (byte[] idBytes)
        {
            if (idBytes.Length != 16)
                throw new mException("Invalid uid was given");

           uint id = MarshalHelper.BytesToStructure<uint>(idBytes).RotateLeft(8);

            if (!activeIds.Contains(id))
                throw new mException("Attempted to call an inactive id!");

            return new ArUid(id);
        }
    }
}
