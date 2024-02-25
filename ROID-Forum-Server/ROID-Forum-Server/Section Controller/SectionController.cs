using System;
using System.Collections.Generic;

namespace ROIDForumServer
{
    public class SectionController
    {
        public Guid SectionId { get; }
        public SectionMessageSender MessageSender { get; }
        public ThreadController ThreadController { get; }
        public List<ConnectedUser> UsersViewing { get; } = new List<ConnectedUser>();
        public String SectionName { get; }

        public SectionController(ServerController serverController, String sectionName, Guid sectionId)
        {
            SectionName = sectionName;
            SectionId = sectionId;
            MessageSender = new SectionMessageSender(this, serverController.Database.GetSession());
            ThreadController = new ThreadController(this, serverController.Database.GetSession());
        }

        public void AddUser(ConnectedUser user)
        {
            UsersViewing.Add(user);
            MessageSender.SendAllThreadsToUser(user);
        }

        public void RemoveUser(ConnectedUser user)
        {
            UsersViewing.Remove(user);
        }

        public void OnMessage(ConnectedUser user, Dictionary<string, object> message)
        {
            switch ((string)message["Title"])
            {
                case "New Post":
                {
                    String postTitle = (string)message["Post Title"];
                    String postDescription = (string)message["Post Description"];
                    ThreadController.AddThread(user, postTitle, postDescription);
                }
                    break;
                case "Edit Post":
                {
                    Guid threadId = Guid.Parse((string)message["Thread Id"]);
                    String title = (string)message["Edit Title"];
                    String description = (string)message["Text"];
                    ThreadController.EditThread(user, SectionId, threadId, title, description);
                }
                    break;
                case "Delete Post":
                {
                    Guid threadId = Guid.Parse((string)message["Thread Id"]);
                    ThreadController.DeleteThread(user, SectionId, threadId);
                }
                    break;
                case "Add Comment":
                {
                    Guid threadId = Guid.Parse((string)message["Thread Id"]);
                    String text = (string)message["Text"];
                    ThreadController.AddComment(user, threadId, SectionId, text);
                }
                    break;
                case "Edit Comment":
                {
                    Guid commentId = Guid.Parse((string)message["Comment Id"]);
                    String description = (string)message["Text"];
                    ThreadController.EditComment(user, commentId, description);
                }
                    break;
                case "Delete Comment":
                {
                    Guid commentId = Guid.Parse((string)message["Comment Id"]);
                    ThreadController.DeleteComment(user, commentId);
                }
                    break;
            }
        }
    }
}