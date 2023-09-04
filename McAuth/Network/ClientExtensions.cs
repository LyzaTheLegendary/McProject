using Common.Network;
using System.Net.Sockets;

namespace McAuth.Network { 

    public static class ClientExtensions
    {
        public static void Kick(this Client client, string reason)
        {
            Msg msg = new Msg(PacketId.KICK);
            msg.WriteString("{\r\n    \"text\": \"foo\",\r\n    \"bold\": true,\r\n    \"extra\": [\r\n        {\r\n            \"text\": \"bar\"\r\n        },\r\n        {\r\n            \"text\": \"baz\",\r\n            \"bold\": false\r\n        },\r\n        {\r\n            \"text\": \"qux\",\r\n            \"bold\": true\r\n        }\r\n    ]\r\n}");
            client.Send((byte[])msg);
        }
    }
}
