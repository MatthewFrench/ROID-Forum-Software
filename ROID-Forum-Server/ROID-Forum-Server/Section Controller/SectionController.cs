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
                if (!message.HasString()) return;
                Guid viewingSectionId = Guid.Parse(message.GetString());
                if (DatabaseSection.SectionIdExists(serverState.Database.GetSession(), viewingSectionId))
                {
                    AddUserToViewing(serverState, user, viewingSectionId);
                }

                return;
            }
            if (SectionReceiveMessages.ExitViewingSection.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid viewingSectionId = Guid.Parse(message.GetString());
                RemoveUserFromViewing(serverState, user, viewingSectionId);
                return;
            }
            
            if (user.AccountId == null)
            {
                return;
            }

            if (SectionReceiveMessages.NewThread.Equals(messageId))
            {
                if (!message.HasString()) return;
                String postTitle = message.GetString();
                if (!message.HasString()) return;
                String postDescription = message.GetString();
                ThreadController.AddThread(serverState, user, sectionId, postTitle, postDescription);
            }
            else if (SectionReceiveMessages.EditThread.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid threadId = Guid.Parse(message.GetString());
                if (!message.HasString()) return;
                String title = message.GetString();
                if (!message.HasString()) return;
                String description = message.GetString();
                ThreadController.EditThread(serverState, user, sectionId, threadId, title, description);
            }
            else if (SectionReceiveMessages.DeleteThread.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid threadId = Guid.Parse(message.GetString());
                ThreadController.DeleteThread(serverState, user, sectionId, threadId);
            }
        }

        
        
        
        
        /*
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
        */
        
        /*
         *
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
         */
    }
}