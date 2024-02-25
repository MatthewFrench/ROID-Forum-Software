using System;
using Cassandra;

namespace ROIDForumServer
{
    public class ThreadController(SectionController sectionController, ISession databaseSession)
    {
        private ISession DatabaseSession { get; } = databaseSession;
        public void AddThread(ConnectedUser user, string title, string description)
        {
            if (user.AccountId == null)
            {
                return;
            }

            Guid threadId = DatabaseThread.CreateThread(DatabaseSession, (Guid)user.AccountId, sectionController.SectionId, title);
            DatabaseComment.CreateComment(DatabaseSession, (Guid)user.AccountId, threadId, sectionController.SectionId, description);
            sectionController.MessageSender.SendAddThreadToAll(threadId, (Guid)user.AccountId, title);

            //Send a message to the All Controller
            //sectionController.server.allSection.threadController.addThread(sectionController.name, t.Id, t.title, t.comments.length, t.owner);

            MoveThreadToTop(threadId);
        }
        public void DeleteThread(ConnectedUser user, Guid sectionId, Guid threadId)
        {
            DatabaseThread.DeleteThread(DatabaseSession, (Guid) user.AccountId, sectionId, threadId);
            sectionController.MessageSender.SendRemoveThreadToAll(threadId);
            //Send a message to the All Controller
            //sectionController.server.allSection.threadController.deleteThread(sectionController.name, t.Id);
        }
        public void EditThread(ConnectedUser user, Guid sectionId, Guid threadId, String title, String description)
        {
            DatabaseThread.UpdateThreadTitle(DatabaseSession, (Guid) user.AccountId, sectionId, threadId, title);
            var (commentId, commentOwnerAccountId) = DatabaseComment.GetThreadFirstComment(DatabaseSession, threadId);
            if (commentOwnerAccountId == user.AccountId)
            {
                DatabaseComment.UpdateComment(DatabaseSession, (Guid) user.AccountId, commentId, description);
            }
            sectionController.MessageSender.SendUpdateThreadToAll(threadId, title);
            //Send a message to the All Controller
            //sectionController.server.allSection.threadController.editThread(sectionController.name, t.Id, t.title);
        }
        private void MoveThreadToTop(Guid threadId)
        {
            sectionController.MessageSender.SendMoveThreadToTopToAll(threadId);
            //Send a message to the All Controller
            //sectionController.server.allSection.threadController.moveThreadToTop(sectionController.name, t.Id);
        }
        public void AddComment(ConnectedUser user, Guid threadId, Guid sectionId, String text)
        {
            DatabaseComment.CreateComment(DatabaseSession, (Guid) user.AccountId, threadId, sectionId, text);
            // Todo: Only send comment to those viewing the thread
            //sectionController.messageSender.sendAddCommentToAll(c);

            //Send a message to the All Controller
            //sectionController.server.allSection.threadController.updateCommentCount(sectionController.name, t.Id, t.comments.length);

            MoveThreadToTop(threadId);
        }
        public void DeleteComment(ConnectedUser user, Guid commentId)
        {
            DatabaseComment.DeleteComment(DatabaseSession, (Guid) user.AccountId, commentId);
            // Todo: Only send comment delete to those viewing the thread
            //sectionController.messageSender.sendDeleteCommentToAll(c);

            //Send a message to the All Controller
            //sectionController.server.allSection.threadController.updateCommentCount(sectionController.name, t.Id, t.comments.length);
        }
        public void EditComment(ConnectedUser user, Guid commentId, string description)
        {
            DatabaseComment.UpdateComment(DatabaseSession, (Guid) user.AccountId, commentId, description);
            // Todo: Only send comment update to those viewing the thread
            //sectionController.messageSender.sendUpdateCommentToAll(c);
        }
    }
}
