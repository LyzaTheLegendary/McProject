using AuroraEngine.Utils;
using Common.Network;
using ClientExtensions.Game;
using AuroraEngine.Storage;
using Common.Helpers;
using AuroraEngine.Scripting;
using AuroraEngine.GameObjects.Creatures;
using Common.Setting;

namespace Gs
{ 
    public class Gameserver
    {
        public readonly byte id;
        public readonly string name;
        private List<Client> clients = new List<Client>();
        public Gameserver(byte _id, string _name)
        {
            id = _id;
            name = _name;
        }

        public void OnJoin(Client client)
        {
            // validate uid information in the database and check if there is a new name
            string? username;
            bool? isBanned;
            if (!PlayerIndex.GetPlayerIndex(client._uid, out username, out isBanned))
            {
                LuaEngine.ExecuteScript("join_first");
                PlayerIndex.CreatePlayerIndex(client._uid, client.username);
            }
            else
                LuaEngine.ExecuteScript("join_login");

            if(client.username != username && username != null)
                LuaEngine.ExecuteScript("join_name_change");

            if (isBanned == true)
            {
                client.Kick("You are banned!");
                return;
            }

            client.AuObjectId = ArUid.GetNewUID();

            MsgWriter msg = new MsgWriter(PacketId.LOGINSUCCESS);
            msg.WriteBytes(MarshalHelper.StructureToBytes(client._uid));
            msg.WriteString(client.username);
            msg.WriteInt(0);
            
            client.Send((byte[])msg);
            

            Human playerObj = new Human(client.AuObjectId, client.username);


            msg = new MsgWriter(PacketId.JOIN_INFO);
            msg.WriteUid(playerObj.GetID());
            msg.WriteByte((byte)Settings.GetSetting("hard_core"));
            msg.WriteByte((byte)Settings.GetSetting("game_mode"));
            msg.WriteByte(0); // previous game mode
            msg.WriteInt(3); // overworld, the end, the nether.
            // next up is dimension list


            // send world / player information



            client.Listen(OnMessage);
        }

        private void OnMessage(MsgReader msg, Client client)
        {

        }

        private void OnDisconnect(Client client)
        {

        }
    }
}
