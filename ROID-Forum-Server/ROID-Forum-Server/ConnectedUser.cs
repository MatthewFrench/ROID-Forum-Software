using System;
using Fleck;
namespace ROIDForumServer
{
    public class ConnectedUser(IWebSocketConnection connection)
    {
        private IWebSocketConnection Connection { get; } = connection;
        public Guid? AccountId { get; set; }
        public Guid? ViewingSectionId { get; set; }
        public Guid? ViewingThreadId { get; set; }

        public void Send(byte[] data)
        {
            Connection.Send(data);
        }
    }
}
