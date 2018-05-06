using System;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class SectionMessageSender
    {

        SectionController controller;
        public SectionMessageSender(SectionController c)
        {
            controller = c;
        }
        public void sendAllThreadsToUser(User u)
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Controller"] = controller.name;
            m["Title"] = "All Threads";
            List<Dictionary<string, object>> threadArray = new List<Dictionary<string, object>>();
            for (int i = 0; i < controller.threadController.threads.Count; i++)
            {
                ThreadInfo t = controller.threadController.threads[i];
                Dictionary<string, object> thread = t.toMap();
                threadArray.Add(thread);
            }
            m["Threads"] = threadArray;
            u.sendMap(m);
        }
        public void sendAddThreadToAll(ThreadInfo t)
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Controller"] = controller.name;
            m["Title"] = "Thread Add";
            Dictionary<string, object> thread = t.toMap();
            m["Thread Map"] = thread;
            foreach (User u in controller.usersViewing)
            {
                u.sendMap(m);
            }
        }
        public void sendRemoveThreadToAll(ThreadInfo t)
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Controller"] = controller.name;
            m["Title"] = "Thread Remove";
            m["ThreadID"] = t.id;
            foreach (User u in controller.usersViewing)
            {
                u.sendMap(m);
            }
        }
        public void sendUpdateThreadToAll(ThreadInfo t)
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Controller"] = controller.name;
            m["Title"] = "Thread Update";
            m["ThreadID"] = t.id;
            m["Thread Title"] = t.title;
            m["Thread Description"] = t.description;
            foreach (User u in controller.usersViewing)
            {
                u.sendMap(m);
            }
        }
        public void sendMoveThreadToTopToAll(ThreadInfo t)
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Controller"] = controller.name;
            m["Title"] = "Thread Move To Top";
            m["ThreadID"] = t.id;
            foreach (User u in controller.usersViewing)
            {
                u.sendMap(m);
            }
        }
        public void sendAddCommentToAll(CommentInfo c)
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Controller"] = controller.name;
            m["Title"] = "Comment Add";
            Dictionary<string, object> commentMap = c.toMap();
            m["Comment"] = commentMap;
            foreach (User u in controller.usersViewing)
            {
                u.sendMap(m);
            }
        }
        public void sendDeleteCommentToAll(CommentInfo c)
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Controller"] = controller.name;
            m["Title"] = "Comment Delete";
            m["ThreadID"] = c.threadID;
            m["CommentID"] = c.commentID;
            foreach (User u in controller.usersViewing)
            {
                u.sendMap(m);
            }
        }
        public void sendUpdateCommentToAll(CommentInfo c)
        {
            Dictionary<string, object> m = new Dictionary<string, object>();
            m["Controller"] = controller.name;
            m["Title"] = "Comment Update";
            m["ThreadID"] = c.threadID;
            m["CommentID"] = c.commentID;
            m["Comment"] = c.comment;
            foreach (User u in controller.usersViewing)
            {
                u.sendMap(m);
            }
        }
    }
}
