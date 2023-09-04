using Common.Network;
using System.Net.Sockets;

namespace McAuth.Network { 

    public struct UserInfo
    {
        public string name;
        public string uuid;
        public string base64SkinBlob;
    }
    public class AuthClient : Client
    {
        
        public UserInfo? _userInfo = null;
        public AuthClient(Socket socket, Action<Msg, Client> onMessage) : base(socket, onMessage)
        {
        }

        public void Authenticate(UserInfo userInfo) => _userInfo = userInfo;
        public bool IsAuthenticated() => (_userInfo == null) ? true : false;
    }
}
