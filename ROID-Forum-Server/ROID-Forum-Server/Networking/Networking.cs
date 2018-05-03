using System;
using Fleck;
using System.Collections.Generic;
using System.Text;
namespace ROIDForumServer
{
	public class Networking
	{
		int port = 7779;
		WebSocketServer websocketServer;
		ServerController serverController;
		List<IWebSocketConnection> webSockets = new List<IWebSocketConnection>();
		public Networking(ServerController controller) {
			serverController = controller;
		}
        
		public void ClientConnectedEvent(IWebSocketConnection socket) {
			webSockets.Add(socket);
			Console.WriteLine("Open!");
			Console.WriteLine("Clients connected: " + GetNumberOfConnectedClients());
           
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
		}
        
        public void ClientDisconnectedEvent(IWebSocketConnection socket)
        {
			webSockets.Remove(socket);
            Console.WriteLine("Close!");
        }

		public void ClientMessageEvent(IWebSocketConnection socket, byte[] binary) {
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
		}


        public void Start()
        {
			websocketServer = new WebSocketServer("ws://127.0.0.1:" + port);
            websocketServer.Start(socket =>
            {
                socket.OnOpen = () => { ClientConnectedEvent(socket); };
                socket.OnClose = () => { ClientDisconnectedEvent(socket); };
                socket.OnBinary = (byte[] binary) => { ClientMessageEvent(socket, binary); };
                socket.OnError = (Exception error) => { Console.WriteLine("Client Error: " + error); };
            });
        }

		public void Stop() {
			foreach (var socket in webSockets) {
				socket.Close();
			}
            webSockets.Clear();
			websocketServer.ListenerSocket.Close();
			websocketServer.Dispose();
		}

		public int GetNumberOfConnectedClients() {
			return webSockets.Count;
		}
	}
}