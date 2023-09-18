using Common.Network;

namespace ClientExtensions.Game
{
    public static class GameClientExtensions
    {
        public static void SendTick(this Client client)
            => client.Send((byte[])new MsgWriter(PacketId.DOTICK));
        
    }
}
