using System;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class ServerController
    {
        Networking networking;
        public AccountController accountController;
        LoginController loginController;
        ChatController chatController;
        HighResolutionTimer logicTimer;
        //SectionController allSection;
        SectionController codingSection;
        SectionController gameSection;
        SectionController graphicsSection;
        SectionController otherSection;
        public ServerController()
        {         
            networking = new Networking(this);
            accountController = new AccountController(this);

            loginController = new LoginController(this);
            chatController = new ChatController(this);
            logicTimer = new HighResolutionTimer(8, logic); //120 fps logic timer
            codingSection = new SectionController(this, "Coding Section");
            //allSection = new SectionController(this);
            gameSection = new SectionController(this, "Game Section");
            graphicsSection = new SectionController(this, "Graphics Section");
            otherSection = new SectionController(this, "Other Section");

            networking.Start();
            Console.ReadKey(true);
            networking.Stop();
			Thread.Sleep(1000);
        }

        public void logic(float delta)
        {
            chatController.logic();
            accountController.logic();

            //allSection.logic();
            codingSection.logic();
            gameSection.logic();
            graphicsSection.logic();
            otherSection.logic();
        }

        public void accountLoggedIn(User u)
        {
            if (u.account != null) {
                Console.WriteLine($"Account logged in {u.account.name}");
            }
            chatController.sendListUpdateToAll();
        }
        public void accountLoggedOut(User u)
        {
            if (u.account != null) {
                Console.WriteLine($"Account logged out {u.account.name}");
            }
            chatController.sendListUpdateToAll();
        }
        public void onOpen(User u)
        {
            chatController.addUser(u);
        }
        public void onMessage(User u, Object message)
        {
            if (message is String)
            {
                Dictionary<string, object> m = JsonConvert.DeserializeObject<Dictionary<string, object>>((string)message);
                if ((string)m["Controller"] == "Chat")
                {
                    chatController.onMessage(u, m);
                }
                if ((string)m["Controller"] == "Login")
                {
                    loginController.onMessage(u, m);
                }
                if ((string)m["Controller"] == codingSection.name) codingSection.onMessage(u, m);
                //if ((string)m["Controller"] == allSection.name) allSection.onMessage(u, m);
                if ((string)m["Controller"] == graphicsSection.name) graphicsSection.onMessage(u, m);
                if ((string)m["Controller"] == gameSection.name) gameSection.onMessage(u, m);
                if ((string)m["Controller"] == otherSection.name) otherSection.onMessage(u, m);
                if ((string)m["Controller"] == "Server" && (string)m["Title"] == "Viewing")
                {
                    disengageFromSection(u.viewingSection, u);
                    engageToSection((string)m["Section"], u);
                }
            }
            /*
            else if (message is Uint8List)
            {
                gameController.onBinaryMessage(u, message);
            }
            */
        }

        public void onClose(User u)
        {
            disengageFromSection(u.viewingSection, u);
            chatController.removeUser(u);
        }
        public void engageToSection(String section, User u)
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
        public void disengageFromSection(String section, User u)
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
    }
}