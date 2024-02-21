using System;
using System.Collections.Generic;
namespace ROIDForumServer
{
    public class ThreadController
    {
        public SectionController controller;
        private Database database;
        public ThreadController(SectionController c, Database database)
        {
            controller = c;
            this.database = database;
        }
        public void addThread(ConnectedUser p, String title, String description)
        {
            if (p.accountID == null)
            {
                return;
            }

            Guid threadID = database.CreateThread((Guid)p.accountID, controller.sectionID, title);
            database.CreateComment((Guid)p.accountID, threadID, controller.sectionID, description);
            controller.messageSender.sendAddThreadToAll(threadID, (Guid)p.accountID, title);

            //Send a message to the All Controller
            //controller.server.allSection.threadController.addThread(controller.name, t.id, t.title, t.comments.length, t.owner);

            moveThreadToTop(threadID);
        }
        public void deleteThread(ConnectedUser p, Guid sectionID, Guid threadID)
        {
            database.DeleteThread((Guid) p.accountID, sectionID, threadID);
            controller.messageSender.sendRemoveThreadToAll(threadID);
            //Send a message to the All Controller
            //controller.server.allSection.threadController.deleteThread(controller.name, t.id);
        }
        public void editThread(ConnectedUser p, Guid sectionID, Guid threadID, String title, String description)
        {
            database.UpdateThreadTitle((Guid) p.accountID, sectionID, threadID, title);
            var (commentID, commentOwnerAccountID) = database.GetThreadFirstComment(threadID);
            if (commentOwnerAccountID == p.accountID)
            {
                database.UpdateComment((Guid) p.accountID, commentID, description);
            }
            controller.messageSender.sendUpdateThreadToAll(threadID, title);
            //Send a message to the All Controller
            //controller.server.allSection.threadController.editThread(controller.name, t.id, t.title);
        }
        public void moveThreadToTop(Guid threadID)
        {
            controller.messageSender.sendMoveThreadToTopToAll(threadID);
            //Send a message to the All Controller
            //controller.server.allSection.threadController.moveThreadToTop(controller.name, t.id);
        }
        public void addComment(ConnectedUser u, Guid threadID, Guid sectionID, String text)
        {
            database.CreateComment((Guid) u.accountID, threadID, sectionID, text);
            // Todo: Only send comment to those viewing the thread
            //controller.messageSender.sendAddCommentToAll(c);

            //Send a message to the All Controller
            //controller.server.allSection.threadController.updateCommentCount(controller.name, t.id, t.comments.length);

            moveThreadToTop(threadID);
        }
        public void deleteComment(ConnectedUser p, Guid commentID)
        {
            database.DeleteComment((Guid) p.accountID, commentID);
            // Todo: Only send comment delete to those viewing the thread
            //controller.messageSender.sendDeleteCommentToAll(c);

            //Send a message to the All Controller
            //controller.server.allSection.threadController.updateCommentCount(controller.name, t.id, t.comments.length);
        }
        public void editComment(ConnectedUser p, Guid commentID, String description)
        {
            database.UpdateComment((Guid) p.accountID, commentID, description);
            // Todo: Only send comment update to those viewing the thread
            //controller.messageSender.sendUpdateCommentToAll(c);
        }
    }
}
