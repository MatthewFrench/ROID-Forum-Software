using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace ROIDForumServer
{
    public class ChatController
    {
        List<String> chats = new List<string>();
        ServerController server;
        List<User> users = new List<User>();
        ChatIOController chatIOController;
        public ChatController(ServerController s)
        {
            chatIOController = new ChatIOController(this);
            server = s;
            chatIOController.loadAllChats();
        }
        public void addUser(User u)
        {
            users.Add(u);
            sendListUpdateToAll();
            sendAllChats(u);
        }
        public void removeUser(User u)
        {
            users.Remove(u);
            sendListUpdateToAll();
        }
        public void sendListUpdateToAll()
        {
            String list = $"Online({users.Count}): ";
            int guests = 0;
            bool addComma = false;
    foreach (User u in users)
            {
                if (u.account != null)
                {
                    if (addComma)
                    {
                        list += ", ";
                    }
                    else
                    {
                        addComma = true;
                    }
                    list += u.account.name;
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

            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Controller"] = "Chat";
            m["Title"] = "Online List";
            m["Data"] = list;
    foreach (User u in users)
            {
                u.sendMap(m);
            }
            Console.WriteLine(list);
        }
        public void logic()
        {
            chatIOController.logic();
        }
        public void addChat(User u, String chat)
        {
            chat = $"{u.account.name}: " + chat;
            chats.Add(chat);
            //Send chat to all connected websockets
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Controller"] = "Chat";
            m["Title"] = "Msg";
            m["Data"] = chat;
            String message = JsonConvert.SerializeObject(m);
            for (int i = 0; i < server.GetNetworking().users.Count; i++)
            {
                User u2 = server.GetNetworking().users[i];
                u2.sendString(message);
            }
        }
        public void sendAllChats(User u)
        { //Send only last 20
            int start = 0;
            // if (chats.length > 20) {
            //   start = chats.length - 20;
            // }
            for (int i = start; i < chats.Count; i++)
            {
                Dictionary<string, object> m = new Dictionary<string, object>();
                m["Controller"] = "Chat";
                m["Title"] = "Msg";
                m["Data"] = chats[i];
                u.sendMap(m);
            }
        }
        public void onMessage(User u, Dictionary<string, object> message)
        {
            if ((string)message["Title"] == "Msg" && u.account != null)
            {
                addChat(u, (string)message["Data"]);
            }
        }
    }
}