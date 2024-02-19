using System;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class ChatController
    {
        ServerController server;
        // Todo: We can use cassandra, redis, or nats to properly show
        // active users and allow subscribing them to events.
        // This will allow multi-instance scaling of the server.
        List<ConnectedUser> users = new List<ConnectedUser>();
        public ChatController(ServerController s)
        {
            server = s;
        }
        public void addUser(ConnectedUser u)
        {
            users.Add(u);
            sendListUpdateToAll();
            sendAllChats(u);
        }
        public void removeUser(ConnectedUser u)
        {
            users.Remove(u);
            sendListUpdateToAll();
        }
        public void sendListUpdateToAll()
        {
            String list = $"Online({users.Count}): ";
            int guests = 0;
            bool addComma = false;
            // Todo: Fix bulk account name lookups, either cache names
            // or make an active online table in Cassandra, or
            // make a bunch of async parallel queries to be distributed
            // among cassandra nodes.
    foreach (ConnectedUser u in users)
            {
                if (u.accountID != null)
                {
                    if (addComma)
                    {
                        list += ", ";
                    }
                    else
                    {
                        addComma = true;
                    }
                    list += server.GetDatabase().GetAccountName((Guid)u.accountID);
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
            foreach (ConnectedUser u in users)
            {
                u.sendBinary(message);
            }
            Console.WriteLine(list);
        }
        public void logic()
        {
        }
        public void addChat(ConnectedUser u, String chat)
        {
            if (u.accountID == null)
            {
                return;
            }
            chat = $"{server.GetDatabase().GetAccountName((Guid)u.accountID)}: " + chat;
            server.GetDatabase().SubmitChat((Guid)u.accountID, chat);
            //Send chat to all connected websockets
            // Todo: Switch to a NATS subscription model
            byte[] chatMsg = ServerMessages.ChatMessage(chat);
            for (int i = 0; i < server.GetNetworking().users.Count; i++)
            {
                ConnectedUser u2 = server.GetNetworking().users[i];
                u2.sendBinary(chatMsg);
            }
        }
        public void sendAllChats(ConnectedUser u)
        {
            foreach (var (creator_account_id, chat_id, content, created_time) in server.GetDatabase().GetRecentChats())
            {
                u.sendBinary(ServerMessages.ChatMessage(content));
            }
        }
        public void onMessage(ConnectedUser u, Dictionary<string, object> message)
        {
            if ((string)message["Title"] == "Msg" && u.accountID != null)
            {
                addChat(u, (string)message["Data"]);
            }
        }
    }
}