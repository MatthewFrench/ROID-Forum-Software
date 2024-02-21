using System;
namespace ROIDForumServer
{
    public class SectionMessageSender
    {
        private Database database;
        SectionController controller;
        public SectionMessageSender(SectionController c)
        {
            controller = c;
            this.database = c.server.GetDatabase();
        }
        public void sendAllThreadsToUser(ConnectedUser u)
        {
            u.sendBinary(ServerMessages.AllThreadsMessage(controller, database.GetThreadsInSection(controller.sectionID)));
        }
        public void sendAddThreadToAll(Guid threadID, Guid creatorAccountID, String title)
        {
            byte[] message = ServerMessages.AddThreadMessage(controller, creatorAccountID, threadID, title);
            foreach (ConnectedUser u in controller.usersViewing)
            {
                u.sendBinary(message);
            }
        }
        public void sendRemoveThreadToAll(Guid threadID)
        {
            byte[] message = ServerMessages.RemoveThreadMessage(controller, threadID);
            foreach (ConnectedUser u in controller.usersViewing)
            {
                u.sendBinary(message);
            }
        }
        public void sendUpdateThreadToAll(Guid threadID, String title)
        {
            byte[] message = ServerMessages.UpdateThreadMessage(controller, threadID, title);
            foreach (ConnectedUser u in controller.usersViewing)
            {
                u.sendBinary(message);
            }
        }
        public void sendMoveThreadToTopToAll(Guid threadID)
        {
            byte[] message = ServerMessages.MoveToTopThreadMessage(controller, threadID);
            foreach (ConnectedUser u in controller.usersViewing)
            {
                u.sendBinary(message);
            }
        }
        /*
        public void sendAddCommentToAll(CommentInfo c)
        {
            byte[] message = ServerMessages.AddCommentMessage(controller, c);
            foreach (ConnectedUser u in controller.usersViewing)
            {
                u.sendBinary(message);
            }
        }
        public void sendDeleteCommentToAll(CommentInfo c)
        {
            byte[] message = ServerMessages.RemoveCommentMessage(controller, c);
            foreach (ConnectedUser u in controller.usersViewing)
            {
                u.sendBinary(message);
            }
        }
        public void sendUpdateCommentToAll(CommentInfo c)
        {
            byte[] message = ServerMessages.UpdateCommentMessage(controller, c);
            foreach (ConnectedUser u in controller.usersViewing)
            {
                u.sendBinary(message);
            }
        }
        */
    }
}
