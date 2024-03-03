using System;

namespace ROIDForumServer
{
    public class ServerController
    {
        private ServerState ServerState { get; }

        public ServerController()
        {
            ServerState = new ServerState();
            ServerState.Networking.Start();
        }

        public void Stop()
        {
            ServerState.Networking.Stop();
        }

        public static void OnUserLoggedIn(ServerState serverState, ConnectedUser user)
        {
            Console.WriteLine($"Account logged in {user.AccountId}, {DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(), (Guid)user.AccountId)}");
            ChatController.UserLoggedIn(serverState, user);
            if (user.ViewingSectionId != null)
            {
                SectionController.UserLoggedIn(serverState, user, (Guid)user.ViewingSectionId);
            }

            if (user.ViewingThreadId != null)
            {
                ThreadController.UserLoggedIn(serverState, user, (Guid)user.ViewingThreadId);
            }
        }

        public static void OnUserLoggedOut(ServerState serverState, ConnectedUser user)
        {
            ChatController.UserLoggedOut(serverState, user);
            if (user.ViewingSectionId != null)
            {
                SectionController.UserLoggedOut(serverState, user, (Guid)user.ViewingSectionId);
            }

            if (user.ViewingThreadId != null)
            {
                ThreadController.UserLoggedOut(serverState, user, (Guid)user.ViewingThreadId);
            }
        }

        public static void OnUserConnected(ServerState serverState, ConnectedUser user)
        {
            ChatController.UserConnected(serverState, user);
            SectionController.UserConnected(serverState, user);
        }

        public static void OnUserDisconnected(ServerState serverState, ConnectedUser user)
        {
            if (user.ViewingSectionId != null)
            {
                SectionController.RemoveUserFromViewing(serverState, user, (Guid)user.ViewingSectionId);
            }

            if (user.ViewingThreadId != null)
            {
                ThreadController.RemoveUserFromViewing(serverState, user, (Guid)user.ViewingThreadId);
            }

            ChatController.UserDisconnected(serverState, user);
        }

        public static void OnUserDisplayNameChanged(ServerState serverState, ConnectedUser user)
        {
            ChatController.UserDisplayNameUpdated(serverState, user);
            SectionController.UserDisplayNameUpdated(serverState, user);
            ThreadController.UserDisplayNameUpdated(serverState, user);
        }

        public static void OnUserAvatarChanged(ServerState serverState, ConnectedUser user)
        {
            SectionController.UserAvatarUpdated(serverState, user);
            ThreadController.UserAvatarUpdated(serverState, user);
        }

        public static void OnMessage(ServerState serverState, ConnectedUser user, MessageReader message)
        {
            if (!message.HasUint8())
            {
                return;
            }

            var messageController = message.GetUint8();
            if ((byte)ServerReceiveControllers.Chat == messageController)
            {
                ChatController.OnMessage(serverState, user, message);
            }
            else if ((byte)ServerReceiveControllers.Profile == messageController)
            {
                ProfileController.OnMessage(serverState, user, message);
            }
            else if ((byte)ServerReceiveControllers.Section == messageController)
            {
                if (!message.HasString())
                {
                    return;
                }

                var sectionId = Guid.Parse(message.GetString());
                if (DatabaseSection.SectionIdExists(serverState.Database.GetSession(), sectionId))
                {
                    SectionController.OnMessage(serverState, user, sectionId, message);
                }
            }
            else if ((byte)ServerReceiveControllers.Thread == messageController)
            {
                if (!message.HasString())
                {
                    return;
                }

                var sectionId = Guid.Parse(message.GetString());
                var threadId = Guid.Parse(message.GetString());
                if (DatabaseSection.SectionIdExists(serverState.Database.GetSession(), sectionId) &&
                    DatabaseThread.ThreadIdExists(serverState.Database.GetSession(), sectionId))
                {
                    ThreadController.OnMessage(serverState, user, sectionId, threadId, message);
                }
            }
        }
    }
}