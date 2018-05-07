using System;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class ThreadController
    {
        public List<ThreadInfo> threads;
        public SectionController controller;
        public int threadIDs = 0;
        public ThreadController(SectionController c)
        {
            controller = c;
            threads = new List<ThreadInfo>();
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
        public void addThread(User p, String title, String description)
        {
            ThreadInfo t = new ThreadInfo(p.account.name, threadIDs++, title, description, p.account.avatarURL);
            threads.Add(t);
            controller.messageSender.sendAddThreadToAll(t);

            //Send a message to the All Controller
            //controller.server.allSection.threadController.addThread(controller.name, t.id, t.title, t.comments.length, t.owner);

            moveThreadToTop(t.id);
        }
        public void deleteThread(User p, int threadID)
        {
            ThreadInfo t = getThreadForID(threadID);
            if (t != null)
            {
                if (t.owner == p.account.name)
                {
                    threads.Remove(t);
                    controller.messageSender.sendRemoveThreadToAll(t);
                    //Send a message to the All Controller
                    //controller.server.allSection.threadController.deleteThread(controller.name, t.id);
                }
            }
        }
        public void editThread(User p, int id, String title, String description)
        {
            ThreadInfo t = getThreadForID(id);
            if (t != null)
            {
                if (t.owner == p.account.name)
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
        public void addComment(User u, int threadID, String text)
        {
            ThreadInfo t = getThreadForID(threadID);
            if (t != null)
            {
                CommentInfo c = new CommentInfo(t.id, t.commentIDs++, text, u.account.name, u.account.avatarURL);
                t.comments.Add(c);
                controller.messageSender.sendAddCommentToAll(c);

                //Send a message to the All Controller
                //controller.server.allSection.threadController.updateCommentCount(controller.name, t.id, t.comments.length);

                moveThreadToTop(threadID);
            }
        }
        public void deleteComment(User p, int threadID, int commentID)
        {
            ThreadInfo t = getThreadForID(threadID);
            if (t != null)
            {
                CommentInfo c = t.getCommentForID(commentID);
                if (c != null)
                {
                    if (p.account.name == c.owner)
                    {
                        t.comments.Remove(c);
                        controller.messageSender.sendDeleteCommentToAll(c);

                        //Send a message to the All Controller
                        //controller.server.allSection.threadController.updateCommentCount(controller.name, t.id, t.comments.length);
                    }
                }
            }
        }
        public void editComment(User p, int threadID, int commentID, String description)
        {
            ThreadInfo t = getThreadForID(threadID);
            if (t != null)
            {
                CommentInfo c = t.getCommentForID(commentID);
                if (c != null)
                {
                    if (p.account.name == c.owner)
                    {
                        c.comment = description;
                        controller.messageSender.sendUpdateCommentToAll(c);
                    }
                }
            }
        }
    }
}
