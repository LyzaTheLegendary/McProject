using System.IO;
using System.Text;

namespace Common.Network
{
    public class McStreamReader
    {
        private Stream _stream;
        public McStreamReader(Stream stream)
            => _stream = stream;
        public int ReadInt()
        {
            int value = 0, position = 0, currentByte;

            while (((currentByte = _stream.ReadByte()) & 128) != 0)
            {
                if (currentByte < 0) throw new EndOfStreamException();
                value |= (currentByte & 127) << position;
                position += 7;
            }

            return value |= (currentByte & 127) << position;
        }
        public ushort ReadUInt16()
        {
            byte[] uint16Bytes = new byte[sizeof(ushort)];
            _stream.ReadExactly(uint16Bytes, 0, sizeof(ushort));
            return (ushort)(uint16Bytes[1] | uint16Bytes[0] << 8);
        }
        public byte ReadByte() 
            => (byte)_stream.ReadByte();
        public byte[] ReadBytes(int count)
        {
            byte[] bytes = new byte[count];
            _stream.ReadExactly(bytes, 0, count);
            return bytes;
        }
        public string ReadString()
        {
            byte[] stringBytes = new byte[ReadInt()];
            _stream.ReadExactly(stringBytes, 0, stringBytes.Length);
            return Encoding.UTF8.GetString(stringBytes);
        }
    }
}
