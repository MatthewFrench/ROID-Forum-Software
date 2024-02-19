﻿using System;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class ServerController
    {
        Database database;
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
            database = new Database(this);
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

            var exitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, eventArgs) => {
                    eventArgs.Cancel = true;
                    exitEvent.Set();
                };
            exitEvent.WaitOne();
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

        public void accountLoggedIn(ConnectedUser u)
        {
            if (u.account != null) {
                Console.WriteLine($"Account logged in {u.account.name}");
            }
            chatController.sendListUpdateToAll();
        }
        public void accountLoggedOut(ConnectedUser u)
        {
            if (u.account != null) {
                Console.WriteLine($"Account logged out {u.account.name}");
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