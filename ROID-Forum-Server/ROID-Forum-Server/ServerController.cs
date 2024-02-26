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
            Console.WriteLine($"Account logged in {user.AccountId}");
            ChatController.UserLoggedIn(serverState, user);
        }

        public static void OnUserLoggedOut(ServerState serverState, ConnectedUser user)
        {
            ChatController.UserLoggedOut(serverState, user);
        }

        public static void OnUserConnected(ServerState serverState, ConnectedUser user)
        {
            ChatController.UserConnected(serverState, user);
        }

        public static void OnUserDisconnected(ServerState serverState, ConnectedUser user)
        {
            if (user.ViewingSectionId != null)
            {
                SectionController.RemoveUser(serverState, user, (Guid)user.ViewingSectionId);
            }

            ChatController.UserDisconnected(serverState, user);
        }

        public static void OnUserDisplayNameChanged(ServerState serverState, ConnectedUser user)
        {
        }

        public static void OnMessage(ServerState serverState, ConnectedUser user, MessageReader message)
        {
            if (!message.HasUint8())
            {
                return;
            }

            var messageController = message.GetUint8();
            if (ServerReceiveControllers.Chat.Equals(messageController))
            {
                ChatController.OnMessage(serverState, user, message);
            }
            else if (ServerReceiveControllers.Profile.Equals(messageController))
            {
                ProfileController.OnMessage(serverState, user, message);
            }
            else if (ServerReceiveControllers.Section.Equals(messageController))
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
        }
    }
}