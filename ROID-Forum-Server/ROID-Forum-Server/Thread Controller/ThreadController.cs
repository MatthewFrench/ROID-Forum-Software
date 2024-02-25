using System;

namespace ROIDForumServer
{
    public static class ThreadController
    {
        public static void AddThread(ServerState serverState, ConnectedUser user, Guid sectionId, string title, string description)
        {
            if (user.AccountId == null)
            {
                return;
            }

            Guid threadId = DatabaseThread.CreateThread(serverState.Database.GetSession(), (Guid)user.AccountId, sectionId, title);
            DatabaseComment.CreateComment(serverState.Database.GetSession(), (Guid)user.AccountId, threadId, sectionId, description);
            SectionMessageSender.SendAddThreadToAll(serverState, sectionId, threadId, (Guid)user.AccountId, title);

            MoveThreadToTop(serverState, sectionId, threadId);
        }
        public static void DeleteThread(ServerState serverState, ConnectedUser user, Guid sectionId, Guid threadId)
        {
            DatabaseThread.DeleteThread(serverState.Database.GetSession(), (Guid) user.AccountId, sectionId, threadId);
            SectionMessageSender.SendRemoveThreadToAll(serverState, sectionId, threadId);
        }
        public static void EditThread(ServerState serverState, ConnectedUser user, Guid sectionId, Guid threadId, String title, String description)
        {
            DatabaseThread.UpdateThreadTitle(serverState.Database.GetSession(), (Guid) user.AccountId, sectionId, threadId, title);
            var (commentId, commentOwnerAccountId) = DatabaseComment.GetThreadFirstComment(serverState.Database.GetSession(), threadId);
            if (commentOwnerAccountId == user.AccountId)
            {
                DatabaseComment.UpdateComment(serverState.Database.GetSession(), (Guid) user.AccountId, commentId, description);
            }
            SectionMessageSender.SendUpdateThreadToAll(serverState, sectionId, threadId, title);
            //Send a message to the All Controller
            //sectionController.server.allSection.threadController.editThread(sectionController.name, t.Id, t.title);
        }
        private static void MoveThreadToTop(ServerState serverState, Guid sectionId, Guid threadId)
        {
            SectionMessageSender.SendMoveThreadToTopToAll(serverState, sectionId, threadId);
            //Send a message to the All Controller
            //sectionController.server.allSection.threadController.moveThreadToTop(sectionController.name, t.Id);
        }
        public static void AddComment(ServerState serverState, ConnectedUser user, Guid threadId, Guid sectionId, String text)
        {
            DatabaseComment.CreateComment(serverState.Database.GetSession(), (Guid) user.AccountId, threadId, sectionId, text);
            // Todo: Only send comment to those viewing the thread
            //sectionController.messageSender.sendAddCommentToAll(c);

            MoveThreadToTop(serverState, sectionId, threadId);
        }
        public static void DeleteComment(ServerState serverState, ConnectedUser user, Guid commentId)
        {
            DatabaseComment.DeleteComment(serverState.Database.GetSession(), (Guid) user.AccountId, commentId);
            // Todo: Only send comment delete to those viewing the thread
            //sectionController.messageSender.sendDeleteCommentToAll(c);
        }
        public static void EditComment(ServerState serverState, ConnectedUser user, Guid commentId, string description)
        {
            DatabaseComment.UpdateComment(serverState.Database.GetSession(), (Guid) user.AccountId, commentId, description);
            // Todo: Only send comment update to those viewing the thread
            //sectionController.messageSender.sendUpdateCommentToAll(c);
        }
    }
}
