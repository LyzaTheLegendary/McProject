using Common.Console;
using Common.Cryptography;
using Common.Helpers;
using Common.Network;
using Common.Setting;
using System.Net.Sockets;

namespace ClientExtensions.Auth { 

    public static class AuthClientExtensions
    {
        public static void HandShake(this Client client)
        {
            try
            {
                _ = client._stream.ReadInt(); // size
                _ = client._stream.ReadInt(); // id
                int ver = client._stream.ReadInt();



                string addr = client._stream.ReadString(); // later proxy?
                ushort port = client._stream.ReadUInt16();
                client._State = (ConnState)client._stream.ReadByte();

                if (client._State == ConnState.Status)
                {
                    _ = client._stream.ReadBytes(2);
                    using (MsgWriter msg = new MsgWriter(PacketId.STATUS))
                    {
                        msg.WriteString($"{{\"version\":{{\"name\":\"SharpCraft\",\"protocol\":{Settings.GetSetting("protocol_ver")}}},\"enforcesSecureChat\":false,\"description\":{{\"text\":\"A Minecraft Server\"}},\"players\":{{\"max\":{Settings.GetSetting("max_player")},\"online\":0}},\"preventsChatReports\":true}}");
                        client.Send((byte[])msg);
                    }
                   
                    client.Send(client._stream.ReadBytes(10));
                }
                if (ver != (int)Settings.GetSetting("protocol_ver"))
                {
                    if (client._State == ConnState.Login)
                    {
                        client.Kick($"Client is running protocol version: {ver} whilst server is on {Settings.GetSetting("protocol_ver")}");
                        client._State = ConnState.Disconnected;
                    }
                    return;
                }
            }
            catch (Exception)
            {
                client.Disconnect();
                client._State = ConnState.Unknown;
            }
        }
        public static void HandleLogin(this Client client)
        { // TODO: I have a dark feeling something dies in here sometimes, and sometimes works not sure what yet though! but encryption goes wrong writing somehow
            try
            {
                _ = client._stream.ReadInt();
                _ = client._stream.ReadInt();
                client.username = client._stream.ReadString();

                if (client._stream.ReadByte() == 1)
                    client._uid = MarshalHelper.BytesToStructure<Int128>(client._stream.ReadBytes(16));
                else
                    throw new NotImplementedException("Haven't found a way to generate a unique number!");

                Rsa rsa = new Rsa();
                MsgWriter msg = new MsgWriter(PacketId.ENCRYPTIONREQUEST);

                byte[] ranBuff = new byte[4];
                new Random().NextBytes(ranBuff);

                msg.WriteString("Lyza SharpCraft");
                msg.WritePrefixedBytes(rsa.Get509Cert());
                msg.WritePrefixedBytes(ranBuff);

                client.Send((byte[])msg);
                _ = client._stream.ReadInt();
                _ = client._stream.ReadInt();

                lock (client._encryption)
                    client._encryption = new AesEncryption(rsa.Decrypt(client._stream.ReadPrefixedBytes()));
                byte[] validationToken = rsa.Decrypt(client._stream.ReadPrefixedBytes());
                client._stream = new McStreamReader(client._encryption.GetStream(new NetworkStream(client._socket)));

                //msg = new MsgWriter(PacketId.COMPRESSIONREQUEST);
                //msg.WriteInt((int)Settings.GetSetting("compression_threshhold"));
                //client.Send((byte[])msg);
            }
            catch (Exception)
            {
                client.Disconnect();
                client._State = ConnState.Unknown;
            }

        }
    }
}
