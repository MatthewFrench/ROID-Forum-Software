using System;
using Fleck;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace ROIDForumServer
{
    public class ConnectedUser
    {
        public IWebSocketConnection connection;
        public Account account;
        public String viewingSection = "";
        public ConnectedUser(IWebSocketConnection c) {
            connection = c;
            account = null;
        }
        public void sendBinary(byte[] data)
        {
            connection.Send(data);
        }
        public void sendMap(Dictionary<string, object> m)
        {
            connection.Send(JsonConvert.SerializeObject(m));
        }
        public void sendString(String o)
        {
            connection.Send(o);
        }
    }
}
