using System;
using Cassandra;

namespace ROIDForumServer
{
    public static class SectionMessageSender
    {
        public static void SendAllThreadsToUser(ServerState serverState, ConnectedUser user, Guid sectionId)
        {
            user.Send(SectionSendMessages.AllThreadsMessage(
                DatabaseThread.GetThreadsInSection(serverState.Database.GetSession(), sectionId))
            );
        }

        public static void SendAddThreadToAll(ServerState serverState, Guid sectionId, Guid threadId, Guid creatorAccountId, String title)
        {
            byte[] message = SectionSendMessages.AddThreadMessage(creatorAccountId, threadId, title);
            foreach (ConnectedUser user in serverState.Networking.GetUsersViewingSection(sectionId))
            {
                user.Send(message);
            }
        }

        public static void SendRemoveThreadToAll(ServerState serverState, Guid sectionId, Guid threadId)
        {
            byte[] message = SectionSendMessages.RemoveThreadMessage(threadId);
            foreach (ConnectedUser user in serverState.Networking.GetUsersViewingSection(sectionId))
            {
                user.Send(message);
            }
        }

        public static void SendUpdateThreadToAll(ServerState serverState, Guid sectionId, Guid threadId, String title)
        {
            byte[] message = SectionSendMessages.UpdateThreadMessage(threadId, title);
            foreach (ConnectedUser user in serverState.Networking.GetUsersViewingSection(sectionId))
            {
                user.Send(message);
            }
        }

        public static void SendMoveThreadToTopToAll(ServerState serverState, Guid sectionId, Guid threadId)
        {
            byte[] message = SectionSendMessages.MoveToTopThreadMessage(threadId);
            foreach (ConnectedUser user in serverState.Networking.GetUsersViewingSection(sectionId))
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