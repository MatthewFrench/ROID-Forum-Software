using System;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class ServerController
    {
        Database database;
        Networking networking;
        LoginController loginController;
        ChatController chatController;
        Dictionary<Guid, SectionController> sections;
        public ServerController()
        {
            database = new Database(this);
            networking = new Networking(this);

            loginController = new LoginController(this);
            chatController = new ChatController(this);
            CreateOrLoadSection("Coding Section");
            CreateOrLoadSection("Game Section");
            CreateOrLoadSection("Graphics Section");
            CreateOrLoadSection("Other Section");

            networking.Start();

            var exitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, eventArgs) => {
                    eventArgs.Cancel = true;
                    exitEvent.Set();
                };
            exitEvent.WaitOne();
            networking.Stop();
			Thread.Sleep(1000);
        }

        private void CreateOrLoadSection(String sectionName)
        {
            Guid sectionID = database.LoadSectionID(sectionName);
            sections.Add(sectionID, new SectionController(this, sectionName, sectionID));
        }

        public void accountLoggedIn(ConnectedUser u)
        {
            if (u.accountID != null) {
                Console.WriteLine($"Account logged in {u.accountID}");
            }
            chatController.sendListUpdateToAll();
        }
        public void accountLoggedOut(ConnectedUser u)
        {
            if (u.accountID != null) {
                Console.WriteLine($"Account logged out {u.accountID}");
            }
            chatController.sendListUpdateToAll();
        }
        public void onOpen(ConnectedUser u)
        {
            chatController.addUser(u);
        }
        public void onMessage(ConnectedUser u, Object message)
        {
            if (message is String)
            {
                //Console.WriteLine("Message: " + message);
                Dictionary<string, object> m = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)message);
                if ((string)m["Controller"] == "Chat")
                {
                    chatController.onMessage(u, m);
                }
                if ((string)m["Controller"] == "Login")
                {
                    loginController.onMessage(u, m);
                }
                if ((string)m["Controller"] == "Server" && (string)m["Title"] == "Viewing")
                {
                    disengageFromSection(u.viewingSection, u);
                    engageToSection((string)m["Section"], u);
                }
                Guid sectionID = Guid.Parse((string)m["Controller"]);
                if (sections.ContainsKey(sectionID))
                {
                    sections[sectionID].onMessage(u, m);
                }
            }
            /*
            else if (message is Uint8List)
            {
                gameController.onBinaryMessage(u, message);
            }
            */
        }

        public void onClose(ConnectedUser u)
        {
            disengageFromSection(u.viewingSection, u);
            chatController.removeUser(u);
        }
        public void engageToSection(String section, ConnectedUser u)
        {
            u.viewingSection = section;
            switch (section)
            {
                case "Coding Section":
                    {
                        codingSection.addUser(u);
                    }
                    break;
                case "All Section":
                    {
                        //allSection.addUser(u);
                    }
                    break;
                case "Other Section":
                    {
                        otherSection.addUser(u);
                    }
                    break;
                case "Graphics Section":
                    {
                        graphicsSection.addUser(u);
                    }
                    break;
                case "Game Section":
                    {
                        gameSection.addUser(u);
                    }
                    break;
            }
        }
        public void disengageFromSection(String section, ConnectedUser u)
        {
            switch (section)
            {
                case "All Section":
                    {
                        //allSection.removeUser(u);
                    }
                    break;
                case "Coding Section":
                    {
                        codingSection.removeUser(u);
                    }
                    break;
                case "Game Section":
                    {
                        gameSection.removeUser(u);
                    }
                    break;
                case "Graphics Section":
                    {
                        graphicsSection.removeUser(u);
                    }
                    break;
                case "Other Section":
                    {
                        otherSection.removeUser(u);
                    }
                    break;
            }
            u.viewingSection = "";
        }

		public Networking GetNetworking() {
			return this.networking;
		}
        public Database GetDatabase() {
            return this.database;
        }
    }
}