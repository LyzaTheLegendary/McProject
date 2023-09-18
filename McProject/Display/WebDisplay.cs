using Common.Helpers;
using Common.Threading;
using Figgle;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Common.Console
{
    public class WebDisplay : DisplayInterface
    {
        private class Session : IDisposable
        {
            private WebSocket _socket;
            public bool isLogged;
            Action<string, Session> _onMessage;
            CancellationTokenSource receiveToken = new CancellationTokenSource();
            public Session(WebSocket socket, Action<string,Session> onMessage)
            {
                _onMessage = onMessage;
                _socket = socket;
                isLogged = false;
                Listen();
            }
            ~Session() { receiveToken.Cancel(); }
            public void Dispose() { }

            public void Send(string text)
            {
                lock(_socket)
                    _socket.SendAsync(Encoding.UTF8.GetBytes(text), WebSocketMessageType.Text, true, new CancellationToken());
            }
            public void CloseWithError(Exception e)
            {
                _socket.CloseAsync(WebSocketCloseStatus.InternalServerError, e.Message, new CancellationToken());

            }
            public async void Listen()
            {
                byte[] buff = new byte[1024];
                try
                {
                    WebSocketReceiveResult result = await _socket.ReceiveAsync(buff, receiveToken.Token);
                    if (result.CloseStatus != null)
                    {
                        Dispose();
                        return;
                    }
                }
                catch (Exception e) { Display.WriteError(ErrorHelper.GetErrorMsg(e)); } // probably disconnected
                if (receiveToken.IsCancellationRequested)
                    return;

                _onMessage(Encoding.UTF8.GetString(buff).TrimEnd('\0'), this);
            }
        }
        HttpListener listener;
        TaskPool pool = new TaskPool(3);
        BlockingCollection<string> backLog = new BlockingCollection<string>();
        List<Session> sessions = new List<Session>();
        readonly string stringAddr;
        readonly string _username;
        readonly string _password;
        public WebDisplay(string address, int port, string username, string password) { 
            listener = new HttpListener();
            stringAddr = $"://{address}:{port}/";
            listener.Prefixes.Add("http" + stringAddr);
            _username = username;
            _password = password;
            pool.EnqueueTask(Start);
        }

        public void Write(string text)
        {
            int maxSize = 100;
            if(backLog.Count > maxSize)
            {
                string[] data = backLog.TakeLast(maxSize).ToArray();
                backLog = new BlockingCollection<string>();
                backLog.Concat(data);
            }
            backLog.Add(text); // should make a max size!
            foreach(Session session in sessions)
                try
                {
                    if(session.isLogged)
                        session.Send(text);
                } catch(Exception e)
                {
                    Display.WriteText("Remote Session closed");
                    sessions.Remove(session);
                    session.CloseWithError(e);
                }
        }
        public void Text(string text)
        {
            Write($"[[;white;]{TimeHelper.GetHMS()}:" + text + ']');
        }
        public void Info(string text)
        {
            Write($"[[;cyan;]{TimeHelper.GetHMS()}:(Info) " + text + ']');
        }

        public void Warn(string text)
        {
            Write($"[[;orange;]{TimeHelper.GetHMS()}:(Warning) " + text + ']');
        }

        public void Error(string text)
        {
            Write($"[[;red;]{TimeHelper.GetHMS()}:(Error) " + text + ']');
        }

        public void Fatal(string text)
        {
            Write($"[[g;red;]{TimeHelper.GetHMS()}:(Fatal) " + text + ']');
        }
        public void Link(string text)
        {
            Write($"[[!;blue;]{TimeHelper.GetHMS()}:(url) " + text + ']');
        }
        private void Start()
        {
            listener.Start();
            while (listener.IsListening)
            {
                HttpListenerContext context = listener.GetContext();
                pool.EnqueueTask(() => { HandleRequest(context); });
            }
        }
        private void HandleRequest(HttpListenerContext context)
        {
            if (context.Request.IsWebSocketRequest) { AcceptWebSocket(context); return; }

            Stream outStream = context.Response.OutputStream;

            string htmlDoc = File.ReadAllText("Resource/Terminal.html");
            htmlDoc = htmlDoc.Replace("{#@WEB_ADDR@#}",  "ws" + stringAddr);


            outStream.Write(Encoding.UTF8.GetBytes(htmlDoc));
            outStream.Close();

        }
        private async void AcceptWebSocket(HttpListenerContext context)
        {
            HttpListenerWebSocketContext webContext = (await context.AcceptWebSocketAsync(subProtocol: null));
            Session session = new Session(webContext.WebSocket, OnMessage);
            session.Send("Please login to use the terminal!");
            //_ = socket.SendAsync(Encoding.UTF8.GetBytes(FiggleFonts.Ogre.Render("Hello world")),WebSocketMessageType.Text,true,new CancellationToken());

            sessions.Add(session);
        }
        private void OnMessage(string msg, Session session)
        {
            if (msg == string.Empty)
                return;
            string[] msgInfo = msg.Split('|');
            if (msgInfo[0] == "LOGIN")
            {
                if (msgInfo[1] == _username && msgInfo[2] == _password)
                {
                    if (session.isLogged)
                        session.Send("Already logged in!");
                    else
                    {
                        session.isLogged = true;
                        session.Send($"[[;green;']{FiggleFonts.Ogre.Render("Welcome admin.")}]");
                        foreach (string text in backLog)
                            session.Send(text);
                    }
                }
                else
                    session.Send("[[;red;]Incorrect password!]");
            }

            if (msgInfo[0] == "LOGOUT")
            {
                session.isLogged = false;
                session.Send("[[;green;]Logged out succesfully!]");
            }

            session.Listen();
        }


    }
}