using System;
using System.Collections.Generic;
using Cassandra;

namespace ROIDForumServer
{
    public class ChatController(ServerController serverController)
    {
        // Todo: We can use cassandra, redis, or nats to properly show
        // active users and allow subscribing them to events.
        // This will allow multi-instance scaling of the server.
        private readonly List<ConnectedUser> _users = new List<ConnectedUser>();
        private ISession DatabaseSession { get; } = serverController.Database.GetSession();
        private Networking Networking { get; } = serverController.Networking;
        public void AddUser(ConnectedUser user)
        {
            _users.Add(user);
            SendListUpdateToAll();
            SendAllChats(user);
        }
        public void RemoveUser(ConnectedUser user)
        {
            _users.Remove(user);
            SendListUpdateToAll();
        }
        public void SendListUpdateToAll()
        {
            String list = $"Online({_users.Count}): ";
            int guests = 0;
            bool addComma = false;
            // Todo: Fix bulk account name lookups, either cache names
            // or make an active online table in Cassandra, or
            // make a bunch of async parallel queries to be distributed
            // among cassandra nodes.
    foreach (ConnectedUser user in _users)
            {
                if (user.AccountId != null)
                {
                    if (addComma)
                    {
                        list += ", ";
                    }
                    else
                    {
                        addComma = true;
                    }
                    list += DatabaseAccount.GetAccountName(DatabaseSession, (Guid)user.AccountId);
                }
                else
                {
                    guests += 1;
                }
            }
            if (addComma)
            {
                list += $" and {guests} Guests";
            }
            else
            {
                list += $"{guests} Guests";
            }

            byte[] message = ServerMessages.ChatOnlineList(list);
            foreach (ConnectedUser user in _users)
            {
                user.Send(message);
            }
            Console.WriteLine(list);
        }
        private void AddChat(ConnectedUser user, String chat)
        {
            if (user.AccountId == null)
            {
                return;
            }
            chat = $"{DatabaseAccount.GetAccountName(DatabaseSession, (Guid)user.AccountId)}: " + chat;
            DatabaseChat.SubmitChat(DatabaseSession, (Guid)user.AccountId, chat);
            //Send chat to all connected websockets
            // Todo: Switch to a NATS subscription model
            byte[] chatMsg = ServerMessages.ChatMessage(chat);
            foreach (var user2 in Networking.Users)
            {
                user2.Send(chatMsg);
            }
        }
        private void SendAllChats(ConnectedUser user)
        {
            foreach (var (_, _, content, _) in DatabaseChat.GetRecentChats(DatabaseSession))
            {
                user.Send(ServerMessages.ChatMessage(content));
            }
        }
        public void OnMessage(ConnectedUser user, Dictionary<string, object> message)
        {
            if ((string)message["Title"] == "Msg" && user.AccountId != null)
            {
                AddChat(user, (string)message["Data"]);
            }
        }
    }
}