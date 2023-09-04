using Common.Console;
using Common.Cryptography;
using Common.Setting;
using Common.Threading;
using System.Net.Sockets;

namespace Common.Network
{
    public enum ConnState
    {
        Status = 1,
        Login = 2,
        ConnectedToGS = 3,
        Disconnected =4,
        Unknown = 99,
    }
    public struct mAddr
    {
        public readonly string _address;
        public readonly ushort _port;
        public mAddr(string address, ushort port)
        {
            _address = address;
            _port = port;
        }
        public override string ToString() => $"{_address}:{_port}";
    }
    public class Client : IDisposable
    {
        public ConnState _State = ConnState.Unknown;
        private Action<Msg, Client> _onMessage;
        private Socket _socket;
        private McStreamReader _stream;
        private IEncryption _encryption = new DummyEncryption();
        private mAddr _addr;
        private TaskPool _pool = new TaskPool(3);
        public Client(Socket socket, Action<Msg,Client> onMessage)
        {
            _socket = socket;
            int timeout = (int)Settings.GetSetting("max_ping");
            _socket.ReceiveTimeout = timeout;
            _socket.SendTimeout = timeout;
            _onMessage = onMessage;
            string[] addrInfo = _socket.RemoteEndPoint.ToString().Split(':');
            _addr = new mAddr(addrInfo[0], ushort.Parse(addrInfo[1]));
            _stream = new McStreamReader(new NetworkStream(_socket));
            
            try { HandShake(); } catch(Exception) { Disconnect(); return; }

            _pool.EnqueueTask(Listen);
        }
        public void HandShake()
        {
            _ = _stream.ReadInt(); // size
            _ = _stream.ReadInt(); // id
            int ver = _stream.ReadInt();

            if (ver != (int)Settings.GetSetting("protocol_ver"))
            {
                Display.WriteInfo($"Pinged with invalid Version. Client ver:{ver}, Server ver:{(int)Settings.GetSetting("protocol_ver")}");
                //TODO: do kick message 
                return;
            }

            string addr = _stream.ReadString();
            ushort port = _stream.ReadUInt16();
            _State = (ConnState)_stream.ReadByte();

            if (_State == ConnState.Status)
                Display.WriteInfo($"Status request from {GetAddr()} version: {ver} to {addr}:{port} ");

            if (_State == ConnState.Login)
                Display.WriteInfo($"Login request from {GetAddr()} version: {ver} to {addr}:{port} ");

             // status?

            if (_State == ConnState.Status)
            {
                _ = _stream.ReadBytes(2);
                using (Msg msg = new Msg(PacketId.STATUS))
                {

                    msg.WriteString($"{{\"version\":{{\"name\":\"SharpCraft\",\"protocol\":{Settings.GetSetting("protocol_ver")}}},\"enforcesSecureChat\":true,\"description\":{{\"text\":\"A Minecraft Server\"}},\"players\":{{\"max\":69,\"online\":0}},\"preventsChatReports\":true}}");
                    Send((byte[])msg);
                }
                Send(_stream.ReadBytes(10));
            }
            
            return;
        }
        public void HandleLogin()
        {
            _ = _stream.ReadInt(); // size;
            _ = _stream.ReadInt(); // id;
            string username = _stream.ReadString();
            byte hasUid = _stream.ReadByte();

            byte[]? uid = null;
            if (hasUid == 1)
                uid = _stream.ReadBytes(16);

            Rsa rsa = new Rsa();

            Msg msg = new Msg(PacketId.ENCRYPTIONREQUEST);
            msg.WriteString("Lyza C# Server");
            //write Prefixed bytes breaks everything for some reason!
            msg.WritePrefixedBytes(rsa.Get509Cert());
            byte[] ranBuff = new byte[4];
            new Random().NextBytes(ranBuff);
            msg.WritePrefixedBytes(ranBuff);

            Send((byte[])msg);

            byte[] key = rsa.Decrypt(_stream.ReadPrefixedBytes()); // bytes we received are invalid somehow?
            lock (_encryption)
                _encryption = new AesEncryption(key);

            byte[] validationToken = rsa.Decrypt(_stream.ReadPrefixedBytes());

        }
        public void Send(byte[] buff)
            => _pool.EnqueueTask(() => { _socket.Send(_encryption.Encrypt(buff)); }); //TODO: fix can be disposed bc async something that shouold be fixed!
        
        public void Listen()
        {
            while (_socket.Connected)
            {
                byte[] buff = new byte[2]; // smallest packet possible techincally, Size + Id
                try
                {

                    if (_socket.Receive(buff, SocketFlags.Peek) == 0)
                    {
                        Disconnect();
                        return;
                    }

                    //int value = 0, position = 0;
                    //byte currentByte = buff[position / 7]; // should use the offset in the socket receive instead
                    //do
                    //{
                    //    if (currentByte < 0) throw new Exception("End of buffer!");
                    //    value |= (currentByte & 127) << position;
                    //    position += 7;
                    //}
                    //while (((currentByte = buff[position / 7]) & 128) != 0);

                    //byte[] msgBuff = new byte[value + position / 7];
                    //if (_socket.Receive(msgBuff, 0,value, SocketFlags.None) == 0)
                    //{
                    //    Disconnect();
                    //    return;
                    //}

                    //Buffer.BlockCopy(msgBuff, position / 7, msgBuff, 0, value - position / 7);
                    //_pool.EnqueueTask(() => { try { 
                    //        using(Msg msg = new Msg(msgBuff))
                    //            _onMessage(msg, this); 
                    //    } catch (Exception e) { Display.WriteError(ErrorHelper.GetErrorMsg(e)); Disconnect(); } });
                }
                catch(Exception e) {
                    //Display.WriteError(ErrorHelper.GetErrorMsg(e)); 
                }

                
            }
        }
        public void Disconnect()
        {
            if(_State == ConnState.Disconnected) return;

            _State = ConnState.Disconnected;

            Display.WriteInfo($"{_addr} has been closed");

            _pool.Stop();
            _socket.Close();
        }
        public mAddr GetAddr() => _addr;
        public void Dispose()
        {
            Disconnect();
        }
    }
}
