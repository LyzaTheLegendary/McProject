using Common.Console;
using Common.Network;
using Common.Setting;
using Common.Threading;
using System.Net;
using System.Net.Sockets;

namespace McAuth
{
    public class Auth
    {
        private Socket  _socket;
        private TaskPool _pool = new TaskPool(2);
        private bool _isAlive = false;
        public Auth(string address, int port)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(address), port);
            _socket = new Socket(ep.AddressFamily,SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(ep);
            _socket.Listen(10);
        }
        public void Start()
        {
            _pool.EnqueueTask(() =>
            {
                Display.WriteInfo($"Listening for connections on: {_socket.LocalEndPoint!} ");
                _isAlive = true;
                while (_isAlive)
                {
                    Client client = new Client(_socket.Accept(), OnMessage);
                    if (client._State == ConnState.Status)
                        client.Dispose();
                }
            });
        }
        private void OnMessage(Msg msg, Client _client) {


        }

    }
}
