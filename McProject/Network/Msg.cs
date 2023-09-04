using System.Diagnostics;
using System.Text;

namespace Common.Network
{
    //    public struct Msg : IDisposable
    //    {
    //        byte[] _buff;
    //        ushort pos = 0;
    //        public Msg(byte[] buff) {
    //#if DEBUG
    //            Debug.WriteLine($"Received packet len: {buff.Length} hex: {BitConverter.ToString(buff)}");
    //#endif
    //            _buff = buff; 
    //        }
    //        public byte[] GetBuff() => _buff;
    //        public int ReadInt() 
    //        {
    //            int value = 0, position = 0; 
    //            byte currentByte = 0;

    //            while ( (pos < _buff.Length) && ((currentByte = _buff[pos++]) & 128) != 0)
    //            {
    //                if (currentByte < 0) throw new EndOfStreamException();
    //                value |= (currentByte & 127) << position;
    //                position += 7;
    //            }

    //            return value |= (currentByte & 127) << position;
    //        }
    //        public ushort ReadUInt16()
    //        {
    //            byte[] bytes = _buff.Skip(pos).Take(sizeof(ushort)).ToArray();

    //            pos += sizeof(ushort);

    //            return (ushort)(bytes[1] | bytes[0] << 8); // we BIT OR them which reverts them, we shove 1 entire byte back by shoving 8 bits back
    //        }
    //        public byte ReadByte()
    //        {
    //            pos++;
    //            return _buff[pos];
    //        }
    //        public byte[] ReadBytes(ushort count)
    //        {
    //            pos++;
    //            byte[] bytes = new byte[count];
    //            Buffer.BlockCopy(_buff, pos, bytes, 0,count);
    //            pos += count;
    //            return bytes;
    //        }
    //        public string ReadString() { 
    //            int size = ReadInt();
    //            string str = Encoding.UTF8.GetString(_buff.Skip(pos).Take(size).ToArray());
    //            pos += (ushort)size;
    //            return str;
    //        }

    //        public void Dispose()
    //        {
    //        }
    //    }
    public class Msg : IDisposable // TODO for some reason it duplicates itself many times when sending!
    {
        private List<byte> _buff = new();

        public Msg(PacketId id)
            => WriteInt((int)id);
        
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

        public static explicit operator byte[](Msg msg)
        {
            List<byte> list = new List<byte>(msg._buff);
            msg._buff.Clear();
            msg.WriteInt(list.Count);

            return msg._buff.Concat(list).ToArray();
        }

        public void Dispose() { }
    }
}
