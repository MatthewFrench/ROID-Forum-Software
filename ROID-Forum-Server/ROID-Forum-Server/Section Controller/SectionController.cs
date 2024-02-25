using System;

namespace ROIDForumServer
{
    public static class SectionController
    {
        public static void AddUser(ServerState serverState, ConnectedUser user, Guid sectionId)
        {
            user.ViewingSectionId = sectionId;
            SectionMessageSender.SendAllThreadsToUser(serverState, user, sectionId);
        }

        public static void RemoveUser(ServerState serverState, ConnectedUser user, Guid sectionId)
        {
            if (user.ViewingSectionId == sectionId)
            {
                user.ViewingSectionId = null;
            }
        }

        public static void OnMessage(ServerState serverState, ConnectedUser user, Guid sectionId, MessageReader message)
        {
            if (user.AccountId == null)
            {
                return;
            }
            if (!message.HasUint8())
            {
                return;
            }

            byte messageId = message.GetUint8();

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
    }
}