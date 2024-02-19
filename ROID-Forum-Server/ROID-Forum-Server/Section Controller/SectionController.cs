using System;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class SectionController
    {
        public ServerController server;
        public Guid sectionID;
        public SectionMessageSender messageSender;
        public ThreadController threadController;
        public List<ConnectedUser> usersViewing = new List<ConnectedUser>();
        public String name = "";
        public SectionIOController ioController;
        int saveTimer = -1;
        public SectionController(ServerController s, String name)
        {
            server = s;
            this.name = name;
            sectionID = server.GetDatabase().LoadSectionID(this.name);
            messageSender = new SectionMessageSender(this);
            threadController = new ThreadController(this);
            ioController = new SectionIOController(this);
            ioController.loadAllData();
        }

        public void addUser(ConnectedUser u)
        {
            usersViewing.Add(u);
            messageSender.sendAllThreadsToUser(u);
        }
        public void removeUser(ConnectedUser u)
        {
            usersViewing.Remove(u);
        }
        public void logic()
        {
            saveTimer += 1;
            if (saveTimer >= 60 * 60 * 2)
            { //Every 2 minutes
                saveTimer = -1;
                ioController.saveAllData();
            }
        }
        public void onMessage(ConnectedUser p, Dictionary<string, object> message)
        {
            switch ((string)message["Title"])
            {
                case "New Post":
                    {
                        String postTitle = (string)message["Post Title"];
                        String postDescription = (string)message["Post Description"];
                        threadController.addThread(p, postTitle, postDescription);
                    }
                    break;
                case "Edit Post":
                    {
                        int threadID = Convert.ToInt32(message["Thread ID"]);
                        String title = (string)message["Edit Title"];
                        String description = (string)message["Text"];
                        threadController.editThread(p, threadID, title, description);
                    }
                    break;
                case "Delete Post":
                    {
                        int threadID = Convert.ToInt32(message["Thread ID"]);
                        threadController.deleteThread(p, threadID);
                    }
                    break;
                case "Add Comment":
                    {
                        int id = Convert.ToInt32(message["ID"]);
                        String text = (string)message["Text"];
                        threadController.addComment(p, id, text);
                    }
                    break;
                case "Edit Comment":
                    {
                        int threadID = Convert.ToInt32(message["Thread ID"]);
                        int commentID = Convert.ToInt32(message["Comment ID"]);
                        String description = (string)message["Text"];
                        threadController.editComment(p, threadID, commentID, description);
                    }
                    break;
                case "Delete Comment":
                    {
                        int threadID = Convert.ToInt32(message["Thread ID"]);
                        int commentID = Convert.ToInt32(message["Comment ID"]);
                        threadController.deleteComment(p, threadID, commentID);
                    }
                    break;
            }

        }
    }
}