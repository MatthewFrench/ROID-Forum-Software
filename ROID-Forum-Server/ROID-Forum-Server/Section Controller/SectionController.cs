using System;

namespace ROIDForumServer
{
    public static class SectionController
    {
        public static void UserConnected(ServerState serverState, ConnectedUser user)
        {
            // Send all section names/IDs to the newly connected user
        }
        public static void UserLoggedIn(ServerState serverState, ConnectedUser user, Guid sectionId)
        {
            // Update everyone viewing this section that this user logged in
        }
        public static void UserLoggedOut(ServerState serverState, ConnectedUser user, Guid sectionId)
        {
            // Update everyone viewing this section that this user logged out
        }
        public static void AddUserToViewing(ServerState serverState, ConnectedUser user, Guid sectionId)
        {
            // Send all threads in the section to the user
            
            // Update everyone in this section that there is a new viewer
            user.ViewingSectionId = sectionId;
            SectionMessageSender.SendAllThreadsToUser(serverState, user, sectionId);
        }

        public static void RemoveUserFromViewing(ServerState serverState, ConnectedUser user, Guid sectionId)
        {
            // Update everyone viewing this section, that this user is no longer viewing
            if (user.ViewingSectionId == sectionId)
            {
                user.ViewingSectionId = null;
            }
        }
        
        public static void UserDisplayNameUpdated(ServerState serverState, ConnectedUser user)
        {
            // Update everyone connected that the display name was updated
        }
        
        public static void UserAvatarUpdated(ServerState serverState, ConnectedUser user)
        {
            // Update everyone connected that the avatar was updated
        }

        public static void OnMessage(ServerState serverState, ConnectedUser user, Guid sectionId, MessageReader message)
        {
            /*
             * if (ProfileReceiveMessages.ViewingSection.Equals(messageId))
               {
                   if (!message.HasString()) return;
                   if (user.ViewingSectionId != null)
                   {
                       SectionController.RemoveUser(serverState, user, (Guid)user.ViewingSectionId);
                   }
                   
                   Guid viewingSectionId = Guid.Parse(message.GetString());
                   if (DatabaseSection.SectionIdExists(serverState.Database.GetSession(), viewingSectionId))
                   {
                       SectionController.AddUser(serverState, user, viewingSectionId);
                   }
               }
               else 
             */
            if (!message.HasUint8())
            {
                return;
            }

            byte messageId = message.GetUint8();

            if (user.AccountId == null)
            {
                return;
            }
            if (SectionReceiveMessages.NewPost.Equals(messageId))
            {
                if (!message.HasString()) return;
                String postTitle = message.GetString();
                if (!message.HasString()) return;
                String postDescription = message.GetString();
                ThreadController.AddThread(serverState, user, sectionId, postTitle, postDescription);
            } else if (SectionReceiveMessages.EditPost.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid threadId = Guid.Parse(message.GetString());
                if (!message.HasString()) return;
                String title = message.GetString();
                if (!message.HasString()) return;
                String description = message.GetString();
                ThreadController.EditThread(serverState, user, sectionId, threadId, title, description);
            } else if (SectionReceiveMessages.DeletePost.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid threadId = Guid.Parse(message.GetString());
                ThreadController.DeleteThread(serverState, user, sectionId, threadId);
            } else if (SectionReceiveMessages.AddComment.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid threadId = Guid.Parse(message.GetString());
                if (!message.HasString()) return;
                String text = message.GetString();
                ThreadController.AddComment(serverState, user, threadId, sectionId, text);
            } else if (SectionReceiveMessages.EditComment.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid commentId = Guid.Parse(message.GetString());
                if (!message.HasString()) return;
                String description = message.GetString();
                ThreadController.EditComment(serverState, user, commentId, description);
            } else if (SectionReceiveMessages.DeleteComment.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid commentId = Guid.Parse(message.GetString());
                ThreadController.DeleteComment(serverState, user, commentId);
            }
        }
        
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
           * /
         */
    }
}