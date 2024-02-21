using System;
using System.Collections.Generic;

namespace ROIDForumServer
{
    public class ServerMessages
    {
        public enum Controller
        {
            Chat = 0,
            Login = 1,
            Section = 2
        }

        public enum ChatMsg
        {
            Msg = 0,
            OnlineList = 1
        }

        public enum SectionMsg {
            AllThreads = 0,
            AddThread = 1,
            RemoveThread = 2,
            UpdateThread = 3,
            MoveThreadToTop = 4,
            AddComment = 5,
            RemoveComment = 6,
            UpdateComment = 7
        }

        public enum LoginMsg {
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

        public static byte[] GetAvatarMessage(String avatarURL)
        {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Login);
            message.AddUint8((byte)LoginMsg.GetAvatar);
            message.AddString(avatarURL);
            return message.ToBuffer();
        }

        public static byte[] AllThreadsMessage(SectionController controller, List<Database.DatabaseThreadData> threads)
        {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.AllThreads);
            message.AddString(controller.name);
            message.AddUint32((uint)threads.Count);
            for (int i = 0; i < threads.Count; i++)
            {
                message.AddBinary(InnerThreadMessage(threads[i]));
            }
            return message.ToBuffer();
        }
        
        public static byte[] InnerThreadMessage(Database.DatabaseThreadData threadData) {
            var message = new MessageWriter();
            message.AddString(threadData.creatorAccountID.ToString());
            message.AddString(threadData.threadID.ToString());
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

        public static byte[] AddThreadMessage(SectionController controller, Guid creatorAccountID, Guid threadID, String title) {

            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.AddThread);
            message.AddString(controller.name);
            message.AddString(creatorAccountID.ToString());
            message.AddString(threadID.ToString());
            message.AddString(title);
            return message.ToBuffer();
        }

        public static byte[] RemoveThreadMessage(SectionController controller, Guid threadID)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.RemoveThread);
            message.AddString(controller.name);
            message.AddString(threadID.ToString());
            return message.ToBuffer();
        }

        public static byte[] UpdateThreadMessage(SectionController controller, Guid threadID, String title)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.UpdateThread);
            message.AddString(controller.name);
            message.AddString(threadID.ToString());
            message.AddString(title);
            return message.ToBuffer();
        }

        public static byte[] MoveToTopThreadMessage(SectionController controller, Guid threadID)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.MoveThreadToTop);
            message.AddString(controller.name);
            message.AddString(threadID.ToString());
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

        public static byte[] RemoveCommentMessage(SectionController controller, Guid commentID, Guid threadID)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.RemoveComment);
            message.AddString(controller.name);
            message.AddString(commentID.ToString());
            message.AddString(threadID.ToString());
            return message.ToBuffer();
        }

        public static byte[] UpdateCommentMessage(SectionController controller, Guid commentID, Guid threadID, String text)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Section);
            message.AddUint8((byte)SectionMsg.UpdateComment);
            message.AddString(controller.name);
            message.AddString(commentID.ToString());
            message.AddString(threadID.ToString());
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
