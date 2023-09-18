using Common.Console;
using Common.Network;
using Common.Threading;
using ClientExtensions.Auth;
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
            _socket.Listen(2);
        }
        public void Start(Action<Client> onJoin)
        {
            Display.WriteInfo($"Listening for connections on: {_socket.LocalEndPoint!} ");
            _isAlive = true;
                
            while (_isAlive)
            {
                Socket remoteSock = _socket.Accept();
                _pool.EnqueueTask(() =>
                { // need to reuse clients
                    Client client = new Client(remoteSock);
                    client.HandShake();
                        
                    if(client._State != ConnState.Login)
                    {
                        client.Dispose();
                        return;
                    }

                    client.HandleLogin();

                    // if it fails we already disconnect it because we can't communicate to it anymore, AS it is out of sync
                    if (client._State == ConnState.Unknown)
                        return;
                    
                    onJoin(client);
                        
                });
            }
        }
    }
}
