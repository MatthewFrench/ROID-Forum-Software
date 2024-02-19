using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace ROIDForumServer
{
    public class ChatController
    {
        List<String> chats = new List<string>();
        ServerController server;
        List<ConnectedUser> users = new List<ConnectedUser>();
        ChatIOController chatIOController;
        public ChatController(ServerController s)
        {
            chatIOController = new ChatIOController(this);
            server = s;
            chatIOController.loadAllChats();
        }
        public void addUser(ConnectedUser u)
        {
            users.Add(u);
            sendListUpdateToAll();
            sendAllChats(u);
        }
        public void removeUser(ConnectedUser u)
        {
            users.Remove(u);
            sendListUpdateToAll();
        }
        public void sendListUpdateToAll()
        {
            String list = $"Online({users.Count}): ";
            int guests = 0;
            bool addComma = false;
    foreach (ConnectedUser u in users)
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

            byte[] message = ServerMessages.ChatOnlineList(list);
            foreach (ConnectedUser u in users)
            {
                u.sendBinary(message);
            }
            Console.WriteLine(list);
        }
        public void logic()
        {
            chatIOController.logic();
        }
        public void addChat(ConnectedUser u, String chat)
        {
            chat = $"{u.account.name}: " + chat;
            chats.Add(chat);
            //Send chat to all connected websockets
            byte[] chatMsg = ServerMessages.ChatMessage(chat);
            for (int i = 0; i < server.GetNetworking().users.Count; i++)
            {
                ConnectedUser u2 = server.GetNetworking().users[i];
                u2.sendBinary(chatMsg);
            }
        }
        public void sendAllChats(ConnectedUser u)
        { //Send only last 20
            int start = 0;
            // if (chats.length > 20) {
            //   start = chats.length - 20;
            // }
            for (int i = start; i < chats.Count; i++)
            {
                u.sendBinary(ServerMessages.ChatMessage(chats[i]));
            }
        }
        public void onMessage(ConnectedUser u, Dictionary<string, object> message)
        {
            if ((string)message["Title"] == "Msg" && u.account != null)
            {
                addChat(u, (string)message["Data"]);
            }
        }
    }
}