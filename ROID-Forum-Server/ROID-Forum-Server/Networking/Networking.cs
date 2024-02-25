using System;
using Fleck;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
namespace ROIDForumServer
{
	public class Networking(ServerController serverController)
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
            serverController.OnOpen(connectedUser);
           
            /*
            var message = new MessageWriter();
            message.AddUint8(1);
            message.AddInt8(-1);
            message.AddUint16(2);
            message.AddInt16(-2);
            message.AddUint32(3);
            message.AddInt32(-3);
            message.AddFloat32(3.3f);
            message.AddFloat64(4.4);
            message.AddString("This is a test string");
            var message2 = new MessageWriter();
            message2.AddString("Inner Binary");
            message.AddBinary(message2.ToBuffer());
			socket.Send(message.ToBuffer());
			*/
		}
        
        private void ClientDisconnectedEvent(IWebSocketConnection socket)
        {
			_webSockets.Remove(socket);
            Console.WriteLine("Close!");
            var user = _userMap.GetValueOrDefault(socket);
            Users.Remove(user);
            serverController.OnClose(user);
            _userMap.Remove(socket);
        }

		private void ClientBinaryMessageEvent(IWebSocketConnection socket, byte[] binary) {
            try
            {
                //serverController.onMessage(userMap.GetValueOrDefault(socket), binary);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }

            /*
			var sb = new StringBuilder();
			foreach (var b in binary)
            {
				if (sb.Length > 0) {
					sb.Append(", ");
				}
                sb.Append(b);
            }
			Console.WriteLine("Bytes: " + binary.Length);
			Console.WriteLine("Got Message! : new byte[] { " + sb.ToString() + " } = " + Encoding.UTF8.GetString(binary));
			Console.WriteLine("Parsed message: ");
			var message = new MessageReader(binary);
			Console.WriteLine("GetUint8: " + message.GetUint8());
			Console.WriteLine("GetInt8: " + message.GetInt8());
			Console.WriteLine("GetUint16: " + message.GetUint16());
			Console.WriteLine("GetInt16: " + message.GetInt16());
			Console.WriteLine("GetUint32: " + message.GetUint32());
			Console.WriteLine("GetInt32: " + message.GetInt32());
			Console.WriteLine("GetFloat32: " + message.GetFloat32());
			Console.WriteLine("GetFloat64: " + message.GetFloat64());
			Console.WriteLine("GetString: " + message.GetString());
			var message2 = new MessageReader(message.GetBinary());
			Console.WriteLine("GetBinary GetString: " + message2.GetString());
			if (message.IsAtEndOfData()) {
				Console.WriteLine("End of Message");
			}
			*/
		}

        private void ClientMessageEvent(IWebSocketConnection socket, string message)
        {
            try
            {
                serverController.OnMessage(_userMap.GetValueOrDefault(socket), message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Errored Network Message: " + message);
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
		        // Note: This should not be done if running the project locally
		        _websocketServer.Certificate = new X509Certificate2("../certs/cert.pfx", "password");
		        _websocketServer.EnabledSslProtocols = SslProtocols.Tls12;
	        }
            _websocketServer.Start(socket =>
            {
                socket.OnOpen = () => { ClientConnectedEvent(socket); };
                socket.OnClose = () => { ClientDisconnectedEvent(socket); };
                socket.OnBinary = (binary) => { ClientBinaryMessageEvent(socket, binary); };
                socket.OnMessage = (message) => { ClientMessageEvent(socket, message); };
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
	}
}