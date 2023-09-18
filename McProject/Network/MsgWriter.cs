using AuroraEngine.Utils;
using System.Text;

namespace Common.Network
{
    public class MsgWriter : IDisposable
    {
        private List<byte> _buff = new();

        public MsgWriter(PacketId id)
            => WriteInt((int)id);
        public byte[] GetBuff() => _buff.ToArray();
        public void WriteUid(ArUid uid)
            => _buff.AddRange((byte[])uid);
        
        public void WriteInt(int integer)
        {
            do
            {
                byte currentByte = (byte)(integer & 0x7F);  //LSB, 127
                integer >>= 7;                              //VLQ, 7

                if (integer > 0)
                    currentByte |= 0x80;                    //MSB, 128

                _buff.Add(currentByte);
            } while (integer > 0);
        }

        public void WriteUShort(ushort value)
        {
            byte[] data = BitConverter.GetBytes(value);
            _buff = _buff.Concat(BitConverter.GetBytes((ushort)(data[1] | data[0] << 8))).ToList();
        }

        public void WriteByte(byte value) 
            => _buff.Add(value);
        public void WriteBytes(byte[] buff) 
            => _buff.AddRange(buff);
        public void WriteString(string text)
        {
            WriteInt(text.Length);
            _buff.AddRange(Encoding.UTF8.GetBytes(text));
        }

        public void WritePrefixedBytes(byte[] buff)
        {
            WriteInt(buff.Length);
            _buff.AddRange(buff); //= _buff.Concat(_buff).ToList();
        }

        public static explicit operator byte[](MsgWriter msg)
        {
            List<byte> list = new List<byte>(msg._buff);
            msg._buff.Clear();
            msg.WriteInt(list.Count);

            return msg._buff.Concat(list).ToArray();
        }

        public void Dispose() { }
    }
}
