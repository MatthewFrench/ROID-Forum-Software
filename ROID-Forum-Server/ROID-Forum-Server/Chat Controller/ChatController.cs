using System;
using System.Linq;

namespace ROIDForumServer
{
    public static class ChatController
    {
        public static void UserConnected(ServerState serverState, ConnectedUser user)
        {
            // Send all chats to the newly connected user
            user.Send(
                ChatSendMessages.AllMessages(
                    DatabaseChat.GetRecentChats(serverState.Database.GetSession())
                        .Select(data => (
                            creatorAccountId: data.CreatorAccountId,
                            creatorDisplayName: DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(),
                                data.CreatorAccountId),
                            chatId: data.ChatId,
                            createdTime: data.CreatedTime,
                            ContentDisposition: data.Content
                        ))
                        .ToList()
                )
            );
            user.Send(ChatSendMessages.AllOnlineList(
                serverState.Networking.Users.Select(connectedUser =>
                    (
                        connectionId: connectedUser.ConnectionId,
                        accountId: connectedUser.AccountId,
                        displayName: connectedUser.AccountId != null
                            ? DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(),
                                (Guid)connectedUser.AccountId)
                            : ""
                    )
                ).ToList()
            ));
            // Tell everyone else that the user is connected
            var addUserMessage = ChatSendMessages.AddUser(user.ConnectionId);
            foreach (var otherUser in serverState.Networking.Users)
            {
                if (user == otherUser) return;
                otherUser.Send(addUserMessage);
            }
        }

        public static void UserDisconnected(ServerState serverState, ConnectedUser user)
        {
            var removeUserMessage = ChatSendMessages.RemoveUser(user.ConnectionId);
            foreach (var otherUser in serverState.Networking.Users)
            {
                if (user == otherUser) return;
                otherUser.Send(removeUserMessage);
            }
        }

        public static void UserLoggedIn(ServerState serverState, ConnectedUser user)
        {
            var userLoggedInMessage = ChatSendMessages.LoggedInUser(
                user.ConnectionId,
                (Guid)user.AccountId,
                DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(), (Guid)user.AccountId)
            );
            foreach (var otherUser in serverState.Networking.Users)
            {
                otherUser.Send(userLoggedInMessage);
            }
        }

        public static void UserLoggedOut(ServerState serverState, ConnectedUser user)
        {
            var userLoggedOutMessage = ChatSendMessages.LoggedOutUser(user.ConnectionId);
            foreach (var otherUser in serverState.Networking.Users)
            {
                otherUser.Send(userLoggedOutMessage);
            }
        }

        public static void UserDisplayNameUpdated(ServerState serverState, ConnectedUser user)
        {
            var userDisplayNameUpdatedMessage = ChatSendMessages.DisplayNameUpdate(
                (Guid)user.AccountId,
                DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(), (Guid)user.AccountId)
            );
            foreach (var otherUser in serverState.Networking.Users)
            {
                otherUser.Send(userDisplayNameUpdatedMessage);
            }
        }

        private static void AddChat(ServerState serverState, ConnectedUser user, string chat)
        {
            if (user.AccountId == null)
            {
                return;
            }

            var chatData = DatabaseChat.SubmitChat(serverState.Database.GetSession(), (Guid)user.AccountId, chat);
            // Send chat to all connected websockets
            // Todo: Switch to a NATS subscription model
            byte[] chatMsg = ChatSendMessages.NewMessage(
                (creatorAccountId: chatData.creatorAccountId,
                    creatorDisplayName: DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(),
                        chatData.creatorAccountId),
                    chatId: chatData.chatId,
                    createdTime: chatData.createdTime,
                    content: chatData.content)
            );
            foreach (var user2 in serverState.Networking.Users)
            {
                user2.Send(chatMsg);
            }
        }

        public static void OnMessage(ServerState serverState, ConnectedUser user, MessageReader message)
        {
            if (!message.HasUint8())
            {
                return;
            }

            byte messageId = message.GetUint8();
            if ((byte)ChatReceiveMessages.Message == messageId && user.AccountId != null)
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