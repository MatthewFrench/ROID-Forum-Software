using System;
using Fleck;
namespace ROIDForumServer
{
    public class ConnectedUser(IWebSocketConnection connection)
    {
        private IWebSocketConnection Connection { get; } = connection;
        public Guid ConnectionId { get; } = Guid.NewGuid();
        public Guid? AccountId { get; set; }
        public Guid? ViewingSectionId { get; set; }
        public Guid? ViewingThreadId { get; set; }

        public void Send(byte[] data)
        {
            // Sending to a closed connection will error
            if (Connection.IsAvailable)
            {
                Connection.Send(data);
            }
        }
    }
}
