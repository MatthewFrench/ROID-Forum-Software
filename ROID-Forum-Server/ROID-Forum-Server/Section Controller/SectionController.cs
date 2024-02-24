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
        public String name;
        private int saveTimer = -1;
        public SectionController(ServerController s, String name, Guid sectionID)
        {
            server = s;
            this.name = name;
            this.sectionID = sectionID;
            messageSender = new SectionMessageSender(this);
            threadController = new ThreadController(this, server.GetDatabase());
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
                        Guid threadID = Guid.Parse((string)message["Thread ID"]);
                        String title = (string)message["Edit Title"];
                        String description = (string)message["Text"];
                        threadController.editThread(p, sectionID, threadID, title, description);
                    }
                    break;
                case "Delete Post":
                    {
                        Guid threadID = Guid.Parse((string)message["Thread ID"]);
                        threadController.deleteThread(p, sectionID, threadID);
                    }
                    break;
                case "Add Comment":
                    {
                        Guid threadID = Guid.Parse((string)message["Thread ID"]);
                        String text = (string)message["Text"];
                        threadController.addComment(p, threadID, sectionID, text);
                    }
                    break;
                case "Edit Comment":
                    {
                        Guid commentID = Guid.Parse((string)message["Comment ID"]);
                        String description = (string)message["Text"];
                        threadController.editComment(p, commentID, description);
                    }
                    break;
                case "Delete Comment":
                    {
                        Guid commentID = Guid.Parse((string)message["Comment ID"]);
                        threadController.deleteComment(p, commentID);
                    }
                    break;
            }

        }
    }
}