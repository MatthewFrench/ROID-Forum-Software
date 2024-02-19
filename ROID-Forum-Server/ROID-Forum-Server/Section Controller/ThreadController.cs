using System;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class ThreadController
    {
        public List<ThreadInfo> threads;
        public SectionController controller;
        public int threadIDs = 0;
        private Database database;
        public ThreadController(SectionController c, Database database)
        {
            controller = c;
            threads = new List<ThreadInfo>();
            this.database = database;
        }
        public ThreadInfo getThreadForID(int threadID)
        {
            for (int i = 0; i < threads.Count; i++)
            {
                if (threads[i].id == threadID)
                {
                    return threads[i];
                }
            }
            return null;
        }
        //Thread actions
        public void addThread(ConnectedUser p, String title, String description)
        {
            if (p.accountID == null)
            {
                return;
            }
            ThreadInfo t = new ThreadInfo(database.GetAccountName((Guid)p.accountID), threadIDs++, title, description, database.GetAvatarUrl((Guid)p.accountID));
            threads.Add(t);
            controller.messageSender.sendAddThreadToAll(t);

            //Send a message to the All Controller
            //controller.server.allSection.threadController.addThread(controller.name, t.id, t.title, t.comments.length, t.owner);

            moveThreadToTop(t.id);
        }
        public void deleteThread(ConnectedUser p, int threadID)
        {
            ThreadInfo t = getThreadForID(threadID);
            if (t != null)
            {
                if (t.owner == database.GetAccountName((Guid)p.accountID))
                {
                    threads.Remove(t);
                    controller.messageSender.sendRemoveThreadToAll(t);
                    //Send a message to the All Controller
                    //controller.server.allSection.threadController.deleteThread(controller.name, t.id);
                }
            }
        }
        public void editThread(ConnectedUser p, int id, String title, String description)
        {
            ThreadInfo t = getThreadForID(id);
            if (t != null)
            {
                if (t.owner == database.GetAccountName((Guid)p.accountID))
                {
                    t.title = title;
                    t.description = description;
                    controller.messageSender.sendUpdateThreadToAll(t);
                    //Send a message to the All Controller
                    //controller.server.allSection.threadController.editThread(controller.name, t.id, t.title);
                }
            }
        }
        public void moveThreadToTop(int id)
        {
            ThreadInfo t = getThreadForID(id);
            if (t != null)
            {
                threads.Remove(t);
                threads.Insert(0, t);
                controller.messageSender.sendMoveThreadToTopToAll(t);
                //Send a message to the All Controller
                //controller.server.allSection.threadController.moveThreadToTop(controller.name, t.id);
            }
        }
        public void addComment(ConnectedUser u, int threadID, String text)
        {
            ThreadInfo t = getThreadForID(threadID);
            if (t != null)
            {
                CommentInfo c = new CommentInfo(t.id, t.commentIDs++, text, database.GetAccountName((Guid)u.accountID), database.GetAvatarUrl((Guid)u.accountID));
                t.comments.Add(c);
                controller.messageSender.sendAddCommentToAll(c);

                //Send a message to the All Controller
                //controller.server.allSection.threadController.updateCommentCount(controller.name, t.id, t.comments.length);

                moveThreadToTop(threadID);
            }
        }
        public void deleteComment(ConnectedUser p, int threadID, int commentID)
        {
            ThreadInfo t = getThreadForID(threadID);
            if (t != null)
            {
                CommentInfo c = t.getCommentForID(commentID);
                if (c != null)
                {
                    if (database.GetAccountName((Guid)p.accountID) == c.owner)
                    {
                        t.comments.Remove(c);
                        controller.messageSender.sendDeleteCommentToAll(c);

                        //Send a message to the All Controller
                        //controller.server.allSection.threadController.updateCommentCount(controller.name, t.id, t.comments.length);
                    }
                }
            }
        }
        public void editComment(ConnectedUser p, int threadID, int commentID, String description)
        {
            ThreadInfo t = getThreadForID(threadID);
            if (t != null)
            {
                CommentInfo c = t.getCommentForID(commentID);
                if (c != null)
                {
                    if (database.GetAccountName((Guid)p.accountID) == c.owner)
                    {
                        c.comment = description;
                        controller.messageSender.sendUpdateCommentToAll(c);
                    }
                }
            }
        }
    }
}
