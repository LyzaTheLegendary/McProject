using AuroraEngine.Utils;
using Common.Network;
using ClientExtensions.Game;
using AuroraEngine.Storage;
using Common.Helpers;
using AuroraEngine.Scripting;
using AuroraEngine.GameObjects.Creatures;
using Common.Setting;
using AuroraEngine.Worlds;

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

            //(int count, string worldInfo) = World.GetWorldInfo();
            Dimension[] dimensions = World.GetAllDimensions();

             

            msg.WriteInt(dimensions.Length); // overworld, the end, the nether.
            foreach(Dimension dimension in dimensions)
                msg.WriteString(dimension.GetDimensionIdentifier());  // A list of compound tags like, "minecraft:overworld" , "minecraft:end" , "minecraft:the_nether"

            //registery codec idk kys

            Dimension overworld = (from dimension in dimensions where dimension._dimensionType == DimensionTypes.OVERWORLD select dimension).First();
            // send type minecraft:flat_world????
            msg.WriteString(overworld.GetDimensionIdentifier());
            //msg.WriteString(worldInfo); //  dimension list (FKED NBT) // should be next one!




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
