using System;
using Fleck;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
namespace ROIDForumServer
{
	public class Networking(ServerState serverState)
    {
	    private WebSocketServer _websocketServer;
	    private readonly List<IWebSocketConnection> _webSockets = new List<IWebSocketConnection>();
	    private readonly Dictionary<IWebSocketConnection, ConnectedUser> _userMap = new Dictionary<IWebSocketConnection, ConnectedUser>();

	    public List<ConnectedUser> Users { get; } = new List<ConnectedUser>();

	    private void ClientConnectedEvent(IWebSocketConnection socket) {
			_webSockets.Add(socket);
			Console.WriteLine("Open!");
			Console.WriteLine("Clients connected: " + GetNumberOfConnectedClients());

            ConnectedUser connectedUser = new ConnectedUser(socket);
            Users.Add(connectedUser);
            _userMap.Add(socket, connectedUser);
            ServerController.OnUserConnected(serverState, connectedUser);
		}
        
        private void ClientDisconnectedEvent(IWebSocketConnection socket)
        {
			_webSockets.Remove(socket);
            Console.WriteLine("Close!");
            var user = _userMap.GetValueOrDefault(socket);
            Users.Remove(user);
            _userMap.Remove(socket);
            ServerController.OnUserDisconnected(serverState, user);
        }

		private void ClientBinaryMessageEvent(IWebSocketConnection socket, byte[] binary) {
            try
            {
                ServerController.OnMessage(serverState, _userMap.GetValueOrDefault(socket), new MessageReader(binary));
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
		}

        public void Start()
        {
	        // Todo: Move this to configuration or environment variables
	        var ip = "0.0.0.0";
	        var port = 7779;
	        if (Environment.IsRunningLocally())
	        {
		        _websocketServer = new WebSocketServer("ws://"+ip+":" + port);
	        }
	        else
	        {
		        _websocketServer = new WebSocketServer("wss://"+ip+":" + port);
		        _websocketServer.Certificate = new X509Certificate2("../certs/cert.pfx", "password");
		        _websocketServer.EnabledSslProtocols = SslProtocols.Tls12;
	        }
            _websocketServer.Start(socket =>
            {
                socket.OnOpen = () => { ClientConnectedEvent(socket); };
                socket.OnClose = () => { ClientDisconnectedEvent(socket); };
                socket.OnBinary = (binary) => { ClientBinaryMessageEvent(socket, binary); };
                socket.OnError = (error) => { Console.WriteLine("Client Error: " + error); };
            });
        }

		public void Stop() {
			foreach (var socket in _webSockets) {
				socket.Close();
			}
            _webSockets.Clear();
			_websocketServer.ListenerSocket.Close();
			_websocketServer.Dispose();
		}

		private int GetNumberOfConnectedClients() {
			return _webSockets.Count;
		}

		public List<ConnectedUser> GetUsersViewingSection(Guid sectionId)
		{
			return Users.FindAll(user => user.ViewingSectionId == sectionId);
		}

		public List<ConnectedUser> GetUsersViewingThread(Guid threadId)
		{
			return Users.FindAll(user => user.ViewingThreadId == threadId);
		}
	}
}