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
        public void sendAllThreadsToUser(ConnectedUser u)
        {
            u.sendBinary(ServerMessages.AllThreadsMessage(controller));
        }
        public void sendAddThreadToAll(ThreadInfo t)
        {
            byte[] message = ServerMessages.AddThreadMessage(controller, t);
            foreach (ConnectedUser u in controller.usersViewing)
            {
                u.sendBinary(message);
            }
        }
        public void sendRemoveThreadToAll(ThreadInfo t)
        {
            byte[] message = ServerMessages.RemoveThreadMessage(controller, t);
            foreach (ConnectedUser u in controller.usersViewing)
            {
                u.sendBinary(message);
            }
        }
        public void sendUpdateThreadToAll(ThreadInfo t)
        {
            byte[] message = ServerMessages.UpdateThreadMessage(controller, t);
            foreach (ConnectedUser u in controller.usersViewing)
            {
                u.sendBinary(message);
            }
        }
        public void sendMoveThreadToTopToAll(ThreadInfo t)
        {
            byte[] message = ServerMessages.MoveToTopThreadMessage(controller, t);
            foreach (ConnectedUser u in controller.usersViewing)
            {
                u.sendBinary(message);
            }
        }
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
    }
}
