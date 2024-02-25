using System;
using Cassandra;

namespace ROIDForumServer
{
    public class SectionMessageSender(SectionController sectionController, ISession databaseSession)
    {
        private ISession DatabaseSession { get; } = databaseSession;

        public void SendAllThreadsToUser(ConnectedUser user)
        {
            user.Send(ServerMessages.AllThreadsMessage(sectionController,
                DatabaseThread.GetThreadsInSection(DatabaseSession, sectionController.SectionId)));
        }

        public void SendAddThreadToAll(Guid threadId, Guid creatorAccountId, String title)
        {
            byte[] message = ServerMessages.AddThreadMessage(sectionController, creatorAccountId, threadId, title);
            foreach (ConnectedUser user in sectionController.UsersViewing)
            {
                user.Send(message);
            }
        }

        public void SendRemoveThreadToAll(Guid threadId)
        {
            byte[] message = ServerMessages.RemoveThreadMessage(sectionController, threadId);
            foreach (ConnectedUser user in sectionController.UsersViewing)
            {
                user.Send(message);
            }
        }

        public void SendUpdateThreadToAll(Guid threadId, String title)
        {
            byte[] message = ServerMessages.UpdateThreadMessage(sectionController, threadId, title);
            foreach (ConnectedUser user in sectionController.UsersViewing)
            {
                user.Send(message);
            }
        }

        public void SendMoveThreadToTopToAll(Guid threadId)
        {
            byte[] message = ServerMessages.MoveToTopThreadMessage(sectionController, threadId);
            foreach (ConnectedUser user in sectionController.UsersViewing)
            {
                user.Send(message);
            }
        }
        /*
        public void sendAddCommentToAll(CommentInfo c)
        {
            byte[] message = ServerMessages.AddCommentMessage(controller, c);
            foreach (ConnectedUser user in controller.usersViewing)
            {
                user.sendBinary(message);
            }
        }
        public void sendDeleteCommentToAll(CommentInfo c)
        {
            byte[] message = ServerMessages.RemoveCommentMessage(controller, c);
            foreach (ConnectedUser user in controller.usersViewing)
            {
                user.sendBinary(message);
            }
        }
        public void sendUpdateCommentToAll(CommentInfo c)
        {
            byte[] message = ServerMessages.UpdateCommentMessage(controller, c);
            foreach (ConnectedUser user in controller.usersViewing)
            {
                user.sendBinary(message);
            }
        }
        */
    }
}