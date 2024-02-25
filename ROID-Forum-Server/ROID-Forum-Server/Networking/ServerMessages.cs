using System;
using System.Collections.Generic;

namespace ROIDForumServer
{
    public static class ServerMessages
    {
        private enum Controller
        {
            Chat = 0,
            Login = 1,
            Section = 2
        }

        private enum ChatMsg
        {
            Msg = 0,
            OnlineList = 1
        }

        private enum SectionMsg {
            AllThreads = 0,
            AddThread = 1,
            RemoveThread = 2,
            UpdateThread = 3,
            MoveThreadToTop = 4,
            AddComment = 5,
            RemoveComment = 6,
            UpdateComment = 7
        }

        private enum LoginMsg {
            GetAvatar = 0,
            LoginFailed = 1,
            LoggedOut = 2,
            RegisterFailed = 3,
            LoggedIn = 4,
        }

        public static byte[] LoggedInMessage(String username)
        {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Login);
            message.AddUint8((byte)LoginMsg.LoggedIn);
            message.AddString(username);

            return message.ToBuffer();
        }

        public static byte[] RegisterFailedMessage()
        {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Login);
            message.AddUint8((byte)LoginMsg.RegisterFailed);
            return message.ToBuffer();
        }

        public static byte[] LoggedOutMessage()
        {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Login);
            message.AddUint8((byte)LoginMsg.LoggedOut);
            return message.ToBuffer();
        }

        public static byte[] LoginFailedMessage()
        {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Login);
            message.AddUint8((byte)LoginMsg.LoginFailed);
            return message.ToBuffer();
        }

        public static byte[] GetAvatarMessage(String avatarUrl)
        {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Login);
            message.AddUint8((byte)LoginMsg.GetAvatar);
            message.AddString(avatarUrl);
            return message.ToBuffer();
        }

        public static byte[] AllThreadsMessage(SectionController controller, List<DatabaseThread.DatabaseThreadData> threads)
        {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.AllThreads);
            message.AddString(controller.SectionName);
            message.AddUint32((uint)threads.Count);
            foreach (var thread in threads)
            {
                message.AddBinary(InnerThreadMessage(thread));
            }
            return message.ToBuffer();
        }
        
        private static byte[] InnerThreadMessage(DatabaseThread.DatabaseThreadData threadData) {
            var message = new MessageWriter();
            message.AddString(threadData.creatorAccountId.ToString());
            message.AddString(threadData.threadId.ToString());
            message.AddString(threadData.title);
            /*
            message.AddUint32((uint)comments.Count);
            for (int i = 0; i < comments.Count; i++)
            {
                message.AddBinary(comments[i].toBinary());
            }
            */
            return message.ToBuffer();
        }

        public static byte[] AddThreadMessage(SectionController controller, Guid creatorAccountId, Guid threadId, String title) {

            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.AddThread);
            message.AddString(controller.SectionName);
            message.AddString(creatorAccountId.ToString());
            message.AddString(threadId.ToString());
            message.AddString(title);
            return message.ToBuffer();
        }

        public static byte[] RemoveThreadMessage(SectionController controller, Guid threadId)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.RemoveThread);
            message.AddString(controller.SectionName);
            message.AddString(threadId.ToString());
            return message.ToBuffer();
        }

        public static byte[] UpdateThreadMessage(SectionController controller, Guid threadId, String title)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.UpdateThread);
            message.AddString(controller.SectionName);
            message.AddString(threadId.ToString());
            message.AddString(title);
            return message.ToBuffer();
        }

        public static byte[] MoveToTopThreadMessage(SectionController controller, Guid threadId)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.MoveThreadToTop);
            message.AddString(controller.SectionName);
            message.AddString(threadId.ToString());
            return message.ToBuffer();
        }
/*
        public static byte[] AddCommentMessage(SectionController controller, CommentInfo comment)
        {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.AddComment);
            message.AddString(controller.name);
            message.AddBinary(comment.toBinary());
            return message.ToBuffer();
        }

        public static byte[] RemoveCommentMessage(SectionController controller, Guid commentId, Guid threadId)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.RemoveComment);
            message.AddString(controller.name);
            message.AddString(commentId.ToString());
            message.AddString(threadId.ToString());
            return message.ToBuffer();
        }

        public static byte[] UpdateCommentMessage(SectionController controller, Guid commentId, Guid threadId, String text)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.UpdateComment);
            message.AddString(controller.name);
            message.AddString(commentId.ToString());
            message.AddString(threadId.ToString());
            message.AddString(text);
            return message.ToBuffer();
        }
        */
        
        public static byte[] ChatMessage(String chat) {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Chat);
            message.AddUint8((byte)ChatMsg.Msg);
            message.AddString(chat);
            return message.ToBuffer();
        }

        public static byte[] ChatOnlineList(String list)
        {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Chat);
            message.AddUint8((byte)ChatMsg.OnlineList);
            message.AddString(list);
            return message.ToBuffer();
        }
    }
}
