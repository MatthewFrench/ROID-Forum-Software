using System;

namespace ROIDForumServer
{
    public static class ChatController
    {
        // Todo: We can use cassandra, redis, or nats to properly show
        // active users and allow subscribing them to events.
        // This will allow multi-instance scaling of the server.
        public static void AddUser(ServerState serverState, ConnectedUser user)
        {
            SendListUpdateToAll(serverState);
            SendAllChats(serverState, user);
        }

        public static void RemoveUser(ServerState serverState, ConnectedUser user)
        {
            SendListUpdateToAll(serverState);
        }

        public static void SendListUpdateToAll(ServerState serverState)
        {
            String list = $"Online({serverState.Networking.Users.Count}): ";
            int guests = 0;
            bool addComma = false;
            // Todo: Fix bulk account name lookups, either cache names
            // or make an active online table in Cassandra, or
            // make a bunch of async parallel queries to be distributed
            // among cassandra nodes.
            foreach (ConnectedUser user in serverState.Networking.Users)
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

                    list += DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(),
                        (Guid)user.AccountId);
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

            byte[] message = ChatSendMessages.ChatOnlineList(list);
            foreach (ConnectedUser user in serverState.Networking.Users)
            {
                user.Send(message);
            }

            Console.WriteLine(list);
        }

        private static void AddChat(ServerState serverState, ConnectedUser user, String chat)
        {
            if (user.AccountId == null)
            {
                return;
            }

            chat =
                $"{DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(), (Guid)user.AccountId)}: " +
                chat;
            DatabaseChat.SubmitChat(serverState.Database.GetSession(), (Guid)user.AccountId, chat);
            // Send chat to all connected websockets
            // Todo: Switch to a NATS subscription model
            byte[] chatMsg = ChatSendMessages.ChatMessage(chat);
            foreach (var user2 in serverState.Networking.Users)
            {
                user2.Send(chatMsg);
            }
        }

        private static void SendAllChats(ServerState serverState, ConnectedUser user)
        {
            foreach (var (_, _, content, _) in DatabaseChat.GetRecentChats(serverState.Database.GetSession()))
            {
                user.Send(ChatSendMessages.ChatMessage(content));
            }
        }

        public static void OnMessage(ServerState serverState, ConnectedUser user, MessageReader message)
        {
            if (!message.HasUint8())
            {
                return;
            }

            byte messageId = message.GetUint8();
            if (ChatReceiveMessages.Message.Equals(messageId) && user.AccountId != null)
            {
                if (!message.HasString())
                {
                    return;
                }

                AddChat(serverState, user, message.GetString());
            }
        }
    }
}