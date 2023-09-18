using System.Numerics;

namespace Common.Helpers
{
    public static class BitHelper
    {
        public static uint RotateLeft(this uint value, int count)
        {
            return (value << count) | (value >> (128 - count));
        }

        public static uint RotateRight(this uint value, int count)
            => (value >> count) | (value << (128 - count));
        
    }
}
