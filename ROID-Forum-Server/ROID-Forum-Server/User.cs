using System;
using Fleck;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace ROIDForumServer
{
    public class User
    {
        public IWebSocketConnection connection;
        public Account account;
        public String viewingSection = "";
        public String inGame = "";
        public User(IWebSocketConnection c) {
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
