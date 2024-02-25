using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ROIDForumServer
{
    public class ServerController
    {
        public Database Database { get; }
        public Networking Networking { get; }
        private ProfileController ProfileController { get; }
        private ChatController ChatController { get; }
        private Dictionary<Guid, SectionController> SectionControllers { get; }

        public ServerController()
        {
            Database = new Database();
            Networking = new Networking(this);
            ProfileController = new ProfileController(this);
            ChatController = new ChatController(this);
            SectionControllers = new Dictionary<Guid, SectionController>();
            CreateOrLoadSection("Coding Section");
            CreateOrLoadSection("Game Section");
            CreateOrLoadSection("Graphics Section");
            CreateOrLoadSection("Other Section");
            Networking.Start();
        }

        private void CreateOrLoadSection(String sectionName)
        {
            var sectionId = DatabaseSection.CreateOrLoadSection(Database.GetSession(), sectionName);
            SectionControllers.Add(sectionId, new SectionController(this, sectionName, sectionId));
        }

        public void AccountLoggedIn(ConnectedUser user)
        {
            if (user.AccountId != null)
            {
                Console.WriteLine($"Account logged in {user.AccountId}");
            }

            ChatController.SendListUpdateToAll();
        }

        public void AccountLoggedOut(ConnectedUser user)
        {
            if (user.AccountId != null)
            {
                Console.WriteLine($"Account logged out {user.AccountId}");
            }

            ChatController.SendListUpdateToAll();
        }

        public void OnOpen(ConnectedUser user)
        {
            ChatController.AddUser(user);
        }

        public void OnMessage(ConnectedUser user, MessageReader message)
        {
            if (!message.HasUint8())
            {
                return;
            }

            var messageController = message.GetUint8();
            if (ServerReceiveControllers.Chat.Equals(messageController))
            {
                ChatController.OnMessage(user, message);
            } else if (ServerReceiveControllers.Profile.Equals(messageController))
            {
                ProfileController.OnMessage(user, message);
            } else if (ServerReceiveControllers.Section.Equals(messageController))
            {
                if (!message.HasString())
                {
                    return;
                }
                var sectionId = Guid.Parse(message.GetString());
                if (!SectionControllers.TryGetValue(sectionId, out var section)) return;
                section.OnMessage(user, message);
            }
        }

        public void OnClose(ConnectedUser user)
        {
            if (user.ViewingSectionId != null)
            {
                DisengageFromSection((Guid)user.ViewingSectionId, user);
            }

            ChatController.RemoveUser(user);
        }

        private void EngageToSection(Guid sectionId, ConnectedUser user)
        {
            if (!SectionControllers.TryGetValue(sectionId, out var section)) return;
            section.AddUser(user);
            user.ViewingSectionId = sectionId;
        }

        private void DisengageFromSection(Guid sectionId, ConnectedUser user)
        {
            if (!SectionControllers.TryGetValue(sectionId, out var section)) return;
            section.RemoveUser(user);
            user.ViewingSectionId = null;
        }
    }
}