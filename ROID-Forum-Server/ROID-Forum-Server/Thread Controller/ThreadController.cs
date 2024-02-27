using System;
using System.Linq;

namespace ROIDForumServer
{
    public static class ThreadController
    {
        public static void UserLoggedIn(ServerState serverState, ConnectedUser user, Guid threadId)
        {
            // Update everyone viewing this thread that this user logged in
            var userLoggedInMessage = ThreadSendMessages.ThreadLoggedInUser(
                user.ConnectionId,
                (Guid)user.AccountId,
                DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(), (Guid)user.AccountId),
                threadId
            );
            foreach (var otherUser in serverState.Networking.Users.Where(connectedUser =>
                         connectedUser.ViewingThreadId == threadId))
            {
                otherUser.Send(userLoggedInMessage);
            }
        }

        public static void UserLoggedOut(ServerState serverState, ConnectedUser user, Guid threadId)
        {
            // Update everyone viewing this thread that this user logged out
            var userLoggedOutMessage = ThreadSendMessages.ThreadLoggedOutUser(
                user.ConnectionId,
                threadId
            );
            foreach (var otherUser in serverState.Networking.Users.Where(connectedUser =>
                         connectedUser.ViewingThreadId == threadId))
            {
                otherUser.Send(userLoggedOutMessage);
            }
        }

        public static void AddUserToViewing(ServerState serverState, ConnectedUser user, Guid threadId)
        {
            // Send all comments in the thread to the user
            var comments = DatabaseComment.GetThreadComments(serverState.Database.GetSession(), threadId);
            user.Send(ThreadSendMessages.AllComments(comments));
            // Send all viewers to the new user
            user.Send(ThreadSendMessages.AllThreadViewers(serverState.Networking.Users
                .Where(connectedUser => connectedUser.ViewingThreadId == threadId).Select(connectedUser =>
                    (
                        connectionId: connectedUser.ConnectionId,
                        accountId: connectedUser.AccountId,
                        displayName: connectedUser.AccountId != null
                            ? DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(),
                                (Guid)connectedUser.AccountId)
                            : ""
                    )
                ).ToList(), threadId));
            // Update everyone in this thread that there is a new viewer
            var viewingUserMessage = ThreadSendMessages.ThreadAddViewer(
                user.ConnectionId,
                user.AccountId,
                user.AccountId != null
                    ? DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(),
                        (Guid)user.AccountId)
                    : "",
                threadId
            );
            foreach (var user2 in serverState.Networking.Users.Where(connectedUser =>
                         connectedUser.ViewingThreadId == threadId))
            {
                user2.Send(viewingUserMessage);
            }

            user.ViewingThreadId = threadId;
        }

        public static void RemoveUserFromViewing(ServerState serverState, ConnectedUser user, Guid threadId)
        {
            // Update everyone viewing this thread, that this user is no longer viewing
            if (user.ViewingThreadId == threadId)
            {
                user.ViewingThreadId = null;
            }

            // Update everyone viewing this thread, that this user is no longer viewing
            var removeMessage = ThreadSendMessages.ThreadRemoveViewer(user.ConnectionId, threadId);
            foreach (var user2 in serverState.Networking.Users.Where(connectedUser =>
                         connectedUser.ViewingThreadId == threadId))
            {
                user2.Send(removeMessage);
            }
        }

        public static void UserDisplayNameUpdated(ServerState serverState, ConnectedUser user)
        {
            // Update everyone connected that the display name was updated
            var userDisplayNameUpdatedMessage = ThreadSendMessages.DisplayNameUpdate(
                (Guid)user.AccountId,
                DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(), (Guid)user.AccountId)
            );
            foreach (var otherUser in serverState.Networking.Users)
            {
                otherUser.Send(userDisplayNameUpdatedMessage);
            }
        }

        public static void UserAvatarUpdated(ServerState serverState, ConnectedUser user)
        {
            // Update everyone connected that the avatar was updated
            var avatarUpdatedMessage = ThreadSendMessages.AvatarUpdate(
                (Guid)user.AccountId,
                DatabaseAccount.GetAvatarUrl(serverState.Database.GetSession(), (Guid)user.AccountId)
            );
            foreach (var otherUser in serverState.Networking.Users)
            {
                otherUser.Send(avatarUpdatedMessage);
            }
        }


        public static void OnMessage(ServerState serverState, ConnectedUser user, Guid sectionId, MessageReader message)
        {
            if (!message.HasUint8())
            {
                return;
            }

            byte messageId = message.GetUint8();

            if (ThreadReceiveMessages.BeginViewingThread.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid viewingThreadId = Guid.Parse(message.GetString());
                if (DatabaseThread.ThreadIdExists(serverState.Database.GetSession(), viewingThreadId))
                {
                    AddUserToViewing(serverState, user, viewingThreadId);
                }

                return;
            }

            if (ThreadReceiveMessages.ExitViewingThread.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid viewingThreadId = Guid.Parse(message.GetString());
                RemoveUserFromViewing(serverState, user, viewingThreadId);
                return;
            }

            if (user.AccountId == null)
            {
                return;
            }

            if (SectionReceiveMessages.AddComment.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid threadId = Guid.Parse(message.GetString());
                if (!message.HasString()) return;
                String text = message.GetString();
                ThreadController.AddComment(serverState, user, threadId, sectionId, text);
            }
            else if (SectionReceiveMessages.EditComment.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid commentId = Guid.Parse(message.GetString());
                if (!message.HasString()) return;
                String description = message.GetString();
                ThreadController.EditComment(serverState, user, commentId, description);
            }
            else if (SectionReceiveMessages.DeleteComment.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid commentId = Guid.Parse(message.GetString());
                ThreadController.DeleteComment(serverState, user, commentId);
            }
        }


        public static void AddComment(ServerState serverState, ConnectedUser user, Guid threadId, Guid sectionId,
            String text)
        {
            DatabaseComment.CreateComment(serverState.Database.GetSession(), (Guid)user.AccountId, threadId, sectionId,
                text);
            // Todo: Only send comment to those viewing the thread
            //sectionController.messageSender.sendAddCommentToAll(c);

            MoveThreadToTop(serverState, sectionId, threadId);
        }

        public static void DeleteComment(ServerState serverState, ConnectedUser user, Guid commentId)
        {
            DatabaseComment.DeleteComment(serverState.Database.GetSession(), (Guid)user.AccountId, commentId);
            // Todo: Only send comment delete to those viewing the thread
            //sectionController.messageSender.sendDeleteCommentToAll(c);
        }

        public static void EditComment(ServerState serverState, ConnectedUser user, Guid commentId, string description)
        {
            DatabaseComment.UpdateComment(serverState.Database.GetSession(), (Guid)user.AccountId, commentId,
                description);
            // Todo: Only send comment update to those viewing the thread
            //sectionController.messageSender.sendUpdateCommentToAll(c);
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