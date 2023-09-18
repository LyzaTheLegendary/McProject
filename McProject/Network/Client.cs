using AuroraEngine.Utils;
using Common.Console;
using Common.Cryptography;
using Common.Helpers;
using Common.Setting;
using Common.Threading;
using System.IO.Compression;
using System.Net.Sockets;

namespace Common.Network
{
    public enum ConnState : byte
    {
        Status = 1,
        Login = 2,
        Play = 3,
        Disconnected = 4,
        Kicked = 5,
        InvalidVersion = 6,
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
        public Int128 _uid;
        public ArUid AuObjectId;
        public string username = string.Empty;
        public ConnState _State = ConnState.Unknown;
        public Socket _socket;
        public McStreamReader _stream;
        public IEncryption _encryption = new DummyEncryption();
        private mAddr _addr;
        public TaskPool _pool = new TaskPool(3);
        public Client(Socket socket)
        {
            _socket = socket;
            int timeout = (int)Settings.GetSetting("max_ping");
            _socket.ReceiveTimeout = timeout;
            _socket.SendTimeout = timeout;
            string[] addrInfo = _socket.RemoteEndPoint!.ToString()!.Split(':');
            _addr = new mAddr(addrInfo[0], ushort.Parse(addrInfo[1]));
            _stream = new McStreamReader(new NetworkStream(_socket));
        }

        public void Send(byte[] buff)
            => _pool.EnqueueTask(() => {
                _encryption.Encrypt(buff);
                try { _socket.Send(buff); } catch (Exception) { Disconnect(); }
            });
        public void Listen(Action<MsgReader, Client> onMessage)
        {
            int compressSize = (int)Settings.GetSetting("compression_threshhold");
            byte[] buff;
            _pool.EnqueueTask(() =>
            {
                while (_socket.Connected)
                {
                    try
                    {

                        if (_socket.Receive(new byte[2], SocketFlags.Peek) == 0)
                        {
                            Disconnect();
                            return;
                        }

                        int packetLength = _stream.ReadInt();


                        buff = new byte[packetLength];

                        if (packetLength > compressSize)
                        {
                            int compressedDataSize = _stream.ReadInt();
                            new ZLibStream(_stream.GetStream(), CompressionMode.Decompress).ReadExactly(buff);
                        }
                        else
                            _stream.GetStream().Read(buff, 0, packetLength);


                        _pool.EnqueueTask(() =>
                        {
                            try { onMessage(new MsgReader(buff), this); } catch(Exception e) { Display.WriteError(ErrorHelper.GetErrorMsg(e)); }
                        });
                    }
                    catch (Exception e)
                    {
                        //Display.WriteError(ErrorHelper.GetErrorMsg(e));
                    }


                }
            });
        }
        public void Kick(string reason)
        {
            MsgWriter msg = new MsgWriter(PacketId.KICK);
            msg.WriteString($"{{\r\n    \"text\": \"{reason}\"\r\n}}");
            Send((byte[])msg);
            _State = ConnState.Kicked;
        }
        public void Disconnect()
        {
            if(_State == ConnState.Disconnected) return;

            _State = ConnState.Disconnected;

            //Display.WriteInfo($"{_addr} has been closed");
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception) { }
            finally
            {
                _pool.Stop();
                _socket.Close();
            }
        }
        public mAddr GetAddr() => _addr;
        public void Dispose()
        {
            Disconnect();
        }
    }
}
