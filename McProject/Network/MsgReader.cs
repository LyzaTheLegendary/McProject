using System.Diagnostics;
using System.Text;

namespace Common.Network
{
    public class MsgReader
    {
        byte[] _buff;
        ushort pos = 0;
        public MsgReader(byte[] buff)
        {
#if DEBUG
            Debug.WriteLine($"Received packet len: {buff.Length} hex: {BitConverter.ToString(buff)}");
#endif
            _buff = buff;
        }
        public byte[] GetBuff() => _buff;
        public int ReadInt()
        {
            int value = 0, position = 0;
            byte currentByte = 0;

            while ((pos < _buff.Length) && ((currentByte = _buff[pos++]) & 128) != 0)
            {
                if (currentByte < 0) throw new EndOfStreamException();
                value |= (currentByte & 127) << position;
                position += 7;
            }

            return value |= (currentByte & 127) << position;
        }
        public ushort ReadUInt16()
        {
            byte[] bytes = _buff.Skip(pos).Take(sizeof(ushort)).ToArray();

            pos += sizeof(ushort);

            return (ushort)(bytes[1] | bytes[0] << 8); // we BIT OR them which reverts them, we shove 1 entire byte back by shoving 8 bits back
        }
        public byte ReadByte()
        {
            pos++;
            return _buff[pos];
        }
        public byte[] ReadBytes(ushort count)
        {
            pos++;
            byte[] bytes = new byte[count];
            Buffer.BlockCopy(_buff, pos, bytes, 0, count);
            pos += count;
            return bytes;
        }
        public string ReadString()
        {
            int size = ReadInt();
            string str = Encoding.UTF8.GetString(_buff.Skip(pos).Take(size).ToArray());
            pos += (ushort)size;
            return str;
        }

        public void Dispose()
        {
        }
    }
}
