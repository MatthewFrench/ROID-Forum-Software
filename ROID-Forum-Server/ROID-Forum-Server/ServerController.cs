using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ROIDForumServer
{
    public class ServerController
    {
        public Database Database { get; }
        public Networking Networking { get; }
        public LoginController LoginController { get; }
        public ChatController ChatController { get; }
        public Dictionary<Guid, SectionController> Sections { get; }

        public ServerController()
        {
            Database = new Database();
            Networking = new Networking(this);
            LoginController = new LoginController(this);
            ChatController = new ChatController(this);
            Sections = new Dictionary<Guid, SectionController>();
            CreateOrLoadSection("Coding Section");
            CreateOrLoadSection("Game Section");
            CreateOrLoadSection("Graphics Section");
            CreateOrLoadSection("Other Section");
            Networking.Start();
        }

        private void CreateOrLoadSection(String sectionName)
        {
            var sectionId = DatabaseSection.LoadSectionId(Database.GetSession(), sectionName);
            Sections.Add(sectionId, new SectionController(this, sectionName, sectionId));
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

        public void OnMessage(ConnectedUser user, Object rawMessage)
        {
            if ((string)rawMessage == null) return;
            //Console.WriteLine("Message: " + message);
            Dictionary<string, object> message = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)rawMessage);
            if ((string)message["Controller"] == "Chat")
            {
                ChatController.OnMessage(user, message);
            }

            if ((string)message["Controller"] == "Login")
            {
                LoginController.OnMessage(user, message);
            }

            if ((string)message["Controller"] == "Server" && (string)message["Title"] == "Viewing")
            {
                if (user.ViewingSectionId != null)
                {
                    DisengageFromSection((Guid) user.ViewingSectionId, user);   
                }
                EngageToSection(Guid.Parse((string)message["Section ID"]), user);
            }

            Guid sectionId = Guid.Parse((string)message["Controller"]);
            if (!Sections.TryGetValue(sectionId, out var section)) return;
            section.OnMessage(user, message);
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
            if (!Sections.TryGetValue(sectionId, out var section)) return;
            section.AddUser(user);
            user.ViewingSectionId = sectionId;
        }

        private void DisengageFromSection(Guid sectionId, ConnectedUser user)
        {
            if (!Sections.TryGetValue(sectionId, out var section)) return;
            section.RemoveUser(user);
            user.ViewingSectionId = null;
        }
    }
}