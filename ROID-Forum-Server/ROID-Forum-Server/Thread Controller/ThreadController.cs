﻿using System;
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


        public static void OnMessage(ServerState serverState, ConnectedUser user, Guid sectionId, Guid threadId, MessageReader message)
        {
            if (!message.HasUint8())
            {
                return;
            }

            byte messageId = message.GetUint8();

            if (ThreadReceiveMessages.BeginViewingThread.Equals(messageId))
            {
                AddUserToViewing(serverState, user, threadId);
                return;
            }

            if (ThreadReceiveMessages.ExitViewingThread.Equals(messageId))
            {
                if (!message.HasString()) return;
                RemoveUserFromViewing(serverState, user, threadId);
                return;
            }

            if (user.AccountId == null)
            {
                return;
            }

            if (ThreadReceiveMessages.AddComment.Equals(messageId))
            {
                if (!message.HasString()) return;
                String text = message.GetString();
                if (!DatabaseThread.ThreadIdExists(serverState.Database.GetSession(), threadId))
                {
                    return;
                }

                Guid commentId = DatabaseComment.CreateComment(serverState.Database.GetSession(), (Guid)user.AccountId,
                    sectionId, threadId, text);
                // Update thread ordering since a comment was added
                var updatedTime =
                    DatabaseSection.UpdateSectionThreadOrdering(serverState.Database.GetSession(), sectionId, threadId);
                // Send updated comment count to all section viewers
                var updateThreadCommentCountAndUpdatedTimeMessage =
                    SectionSendMessages.UpdateThreadCommentCountAndUpdatedTime(sectionId, threadId,
                        DatabaseComment.GetCommentCount(serverState.Database.GetSession(), threadId), updatedTime);
                foreach (var otherUser in serverState.Networking.Users.Where(connectedUser =>
                             connectedUser.ViewingSectionId == sectionId))
                {
                    otherUser.Send(updateThreadCommentCountAndUpdatedTimeMessage);
                }

                // Send add comment message to all users viewing the thread
                var addCommentMessage =
                    ThreadSendMessages.AddComment(DatabaseComment.GetComment(serverState.Database.GetSession(),
                        commentId));
                foreach (var otherUser in serverState.Networking.Users.Where(connectedUser =>
                             connectedUser.ViewingThreadId == threadId))
                {
                    otherUser.Send(addCommentMessage);
                }

                // Send successfully created comment message to user
                user.Send(ThreadSendMessages.CommentSuccessfullyCreated(sectionId, threadId, commentId));
            }
            else if (ThreadReceiveMessages.EditComment.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid commentId = Guid.Parse(message.GetString());
                if (!DatabaseComment.CommentIdExists(serverState.Database.GetSession(), commentId))
                {
                    return;
                }

                if (user.AccountId != DatabaseComment.GetCommentOwner(serverState.Database.GetSession(), commentId))
                {
                    return;
                }
                if (!message.HasString()) return;
                String description = message.GetString();
                if (String.IsNullOrWhiteSpace(description))
                {
                    return;
                }
                DatabaseComment.UpdateComment(serverState.Database.GetSession(), commentId, description);
                
                // Send update comment message to all users viewing the thread
                var updateCommentMessage =
                    ThreadSendMessages.UpdateComment(sectionId, threadId, commentId, description);
                foreach (var otherUser in serverState.Networking.Users.Where(connectedUser =>
                             connectedUser.ViewingThreadId == threadId))
                {
                    otherUser.Send(updateCommentMessage);
                }
            }
            else if (ThreadReceiveMessages.DeleteComment.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid commentId = Guid.Parse(message.GetString());
                if (!DatabaseComment.CommentIdExists(serverState.Database.GetSession(), commentId))
                {
                    return;
                }

                if (user.AccountId != DatabaseComment.GetCommentOwner(serverState.Database.GetSession(), commentId))
                {
                    return;
                }

                DatabaseComment.DeleteComment(serverState.Database.GetSession(), commentId);

                // Send remove comment message to all users viewing the thread
                var removeCommentMessage =
                    ThreadSendMessages.RemoveComment(sectionId, threadId, commentId);
                foreach (var otherUser in serverState.Networking.Users.Where(connectedUser =>
                             connectedUser.ViewingThreadId == threadId))
                {
                    otherUser.Send(removeCommentMessage);
                }

                // Send updated comment count to all section viewers
                var updateThreadCommentCountMessage =
                    SectionSendMessages.UpdateThreadCommentCount(sectionId, threadId,
                        DatabaseComment.GetCommentCount(serverState.Database.GetSession(), threadId));
                foreach (var otherUser in serverState.Networking.Users.Where(connectedUser =>
                             connectedUser.ViewingSectionId == sectionId))
                {
                    otherUser.Send(updateThreadCommentCountMessage);
                }
            }
        }
    }
}