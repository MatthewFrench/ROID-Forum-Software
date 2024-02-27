using System;
using System.Linq;

namespace ROIDForumServer
{
    public static class SectionController
    {
        public static void UserConnected(ServerState serverState, ConnectedUser user)
        {
            // Send all section names/IDs to the newly connected user
            var sections = DatabaseSection.GetSections(serverState.Database.GetSession());
            user.Send(SectionSendMessages.AllSectionHeaders(sections));
        }

        public static void UserLoggedIn(ServerState serverState, ConnectedUser user, Guid sectionId)
        {
            // Update everyone viewing this section that this user logged in
            var userLoggedInMessage = SectionSendMessages.SectionLoggedInUser(
                user.ConnectionId,
                (Guid)user.AccountId,
                DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(), (Guid)user.AccountId),
                sectionId
            );
            foreach (var otherUser in serverState.Networking.Users.Where(connectedUser => connectedUser.ViewingSectionId == sectionId))
            {
                otherUser.Send(userLoggedInMessage);
            }
        }

        public static void UserLoggedOut(ServerState serverState, ConnectedUser user, Guid sectionId)
        {
            // Update everyone viewing this section that this user logged out
            var userLoggedOutMessage = SectionSendMessages.SectionLoggedOutUser(
                user.ConnectionId,
                sectionId
            );
            foreach (var otherUser in serverState.Networking.Users.Where(connectedUser => connectedUser.ViewingSectionId == sectionId))
            {
                otherUser.Send(userLoggedOutMessage);
            }
        }

        public static void AddUserToViewing(ServerState serverState, ConnectedUser user, Guid sectionId)
        {
            // Send all thread headers in the section to the user
            var threadHeaders = DatabaseThread.GetThreadHeadersInSection(serverState.Database.GetSession(), sectionId);
            user.Send(SectionSendMessages.AllThreadHeaders(threadHeaders));
            // Send all viewers to the new user
            user.Send(SectionSendMessages.AllSectionViewers(serverState.Networking.Users
                .Where(connectedUser => connectedUser.ViewingSectionId == sectionId).Select(connectedUser =>
                    (
                        connectionId: connectedUser.ConnectionId,
                        accountId: connectedUser.AccountId,
                        displayName: connectedUser.AccountId != null
                            ? DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(),
                                (Guid)connectedUser.AccountId)
                            : ""
                    )
                ).ToList(), sectionId));
            // Update everyone in this section that there is a new viewer
            var viewingUserMessage = SectionSendMessages.SectionAddViewer(
                user.ConnectionId,
                user.AccountId,
                user.AccountId != null
                    ? DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(),
                        (Guid)user.AccountId)
                    : "",
                sectionId
            );
            foreach (var user2 in serverState.Networking.Users.Where(connectedUser =>
                         connectedUser.ViewingSectionId == sectionId))
            {
                user2.Send(viewingUserMessage);
            }
            user.ViewingSectionId = sectionId;
        }

        public static void RemoveUserFromViewing(ServerState serverState, ConnectedUser user, Guid sectionId)
        {
            if (user.ViewingSectionId == sectionId)
            {
                user.ViewingSectionId = null;
            }

            // Update everyone viewing this section, that this user is no longer viewing
            var removeMessage = SectionSendMessages.SectionRemoveViewer(user.ConnectionId, sectionId);
            foreach (var user2 in serverState.Networking.Users.Where(connectedUser =>
                         connectedUser.ViewingSectionId == sectionId))
            {
                user2.Send(removeMessage);
            }
        }

        public static void UserDisplayNameUpdated(ServerState serverState, ConnectedUser user)
        {
            // Update everyone connected that the display name was updated
            var userDisplayNameUpdatedMessage = SectionSendMessages.DisplayNameUpdate(
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
            var avatarUpdatedMessage = SectionSendMessages.AvatarUpdate(
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

            if (SectionReceiveMessages.BeginViewingSection.Equals(messageId))
            {
                AddUserToViewing(serverState, user, sectionId);
                return;
            }
            if (SectionReceiveMessages.ExitViewingSection.Equals(messageId))
            {
                RemoveUserFromViewing(serverState, user, sectionId);
                return;
            }
            
            if (user.AccountId == null)
            {
                return;
            }

            if (SectionReceiveMessages.NewThread.Equals(messageId))
            {
                if (!message.HasString()) return;
                String title = message.GetString();
                if (!message.HasString()) return;
                String description = message.GetString();
                
                Guid threadId = DatabaseThread.CreateThread(serverState.Database.GetSession(), (Guid)user.AccountId, sectionId, title, description);
                // Send add thread message to all users viewing the section
                var addThreadMessage = SectionSendMessages.AddThreadHeader(DatabaseThread.GetThreadHeader(serverState.Database.GetSession(), threadId));
                foreach (var otherUser in serverState.Networking.Users.Where(connectedUser => connectedUser.ViewingSectionId == sectionId))
                {
                    otherUser.Send(addThreadMessage);
                }
                // Send successfully created thread message to user
                user.Send(SectionSendMessages.ThreadSuccessfullyCreated(sectionId, threadId));
            }
            else if (SectionReceiveMessages.EditThreadTitleAndDescription.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid threadId = Guid.Parse(message.GetString());
                if (!DatabaseThread.ThreadIdExists(serverState.Database.GetSession(), threadId))
                {
                    return;
                }
                if (user.AccountId != DatabaseThread.GetThreadOwner(serverState.Database.GetSession(), threadId))
                {
                    return;
                }
                if (!message.HasString()) return;
                String title = message.GetString();
                if (!message.HasString()) return;
                String description = message.GetString();
                if (String.IsNullOrWhiteSpace(title) || String.IsNullOrWhiteSpace(description))
                {
                    return;
                }
                DatabaseThread.UpdateThreadTitleAndDescription(serverState.Database.GetSession(), threadId, title, description);
                var updateThreadMessage = SectionSendMessages.UpdateThreadTitleAndDescription(sectionId, threadId, title, description);
                foreach (var otherUser in serverState.Networking.Users.Where(connectedUser => connectedUser.ViewingSectionId == sectionId || connectedUser.ViewingThreadId == threadId))
                {
                    otherUser.Send(updateThreadMessage);
                }
            }
            else if (SectionReceiveMessages.DeleteThread.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid threadId = Guid.Parse(message.GetString());
                if (!DatabaseThread.ThreadIdExists(serverState.Database.GetSession(), threadId))
                {
                    return;
                }
                if (user.AccountId != DatabaseThread.GetThreadOwner(serverState.Database.GetSession(), threadId))
                {
                    return;
                }
                DatabaseThread.DeleteThreadAndComments(serverState.Database.GetSession(), (Guid) user.AccountId, sectionId, threadId);
                // Update all users that the thread was deleted
                var deleteThreadMessage = SectionSendMessages.RemoveThreadHeader(sectionId, threadId);
                foreach (var otherUser in serverState.Networking.Users.Where(connectedUser => connectedUser.ViewingSectionId == sectionId || connectedUser.ViewingThreadId == threadId))
                {
                    otherUser.Send(deleteThreadMessage);
                }
                // Kick out any users viewing the delete thread
                foreach (var otherUser in serverState.Networking.Users)
                {
                    otherUser.ViewingThreadId = null;
                }
            }
        }
    }
}