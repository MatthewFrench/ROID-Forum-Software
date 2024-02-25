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

        public static void AccountLoggedIn(ServerState serverState, ConnectedUser user)
        {
            if (user.AccountId != null)
            {
                Console.WriteLine($"Account logged in {user.AccountId}");
            }

            ChatController.SendListUpdateToAll(serverState);
        }

        public static void AccountLoggedOut(ServerState serverState, ConnectedUser user)
        {
            if (user.AccountId != null)
            {
                Console.WriteLine($"Account logged out {user.AccountId}");
            }

            ChatController.SendListUpdateToAll(serverState);
        }

        public static void OnOpen(ServerState serverState, ConnectedUser user)
        {
            ChatController.AddUser(serverState, user);
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
            } else if (ServerReceiveControllers.Profile.Equals(messageController))
            {
                ProfileController.OnMessage(serverState, user, message);
            } else if (ServerReceiveControllers.Section.Equals(messageController))
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

        public static void OnClose(ServerState serverState, ConnectedUser user)
        {
            if (user.ViewingSectionId != null)
            {
                DisengageFromSection(serverState, (Guid)user.ViewingSectionId, user);
            }

            ChatController.RemoveUser(serverState, user);
        }

        public static void EngageToSection(ServerState serverState, Guid sectionId, ConnectedUser user)
        {
            SectionController.AddUser(serverState, user, sectionId);
            user.ViewingSectionId = sectionId;
        }

        public static void DisengageFromSection(ServerState serverState, Guid sectionId, ConnectedUser user)
        {
            SectionController.RemoveUser(serverState, user);
            user.ViewingSectionId = null;
        }
    }
}