using System.Runtime.InteropServices;

namespace Common.Helpers;

public static class MarshalHelper
{
    public static T BytesToStructure<T>(byte[] bytes)
    {
        int size = Marshal.SizeOf(typeof(T));
        if (bytes.Length < size)
            throw new Exception("Invalid parameter");

        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.Copy(bytes, 0, ptr, size);
            return (T)Marshal.PtrToStructure(ptr, typeof(T))!;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
    public static byte[] StructureToBytes<T>(T structure)
    {
        int structSize = Marshal.SizeOf(structure);
        byte[] bytes = new byte[structSize];

        IntPtr ptr = Marshal.AllocHGlobal(structSize);
        try
        {
            Marshal.StructureToPtr(structure!, ptr, false);
            Marshal.Copy(ptr, bytes, 0, structSize);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        return bytes;
    }
}
