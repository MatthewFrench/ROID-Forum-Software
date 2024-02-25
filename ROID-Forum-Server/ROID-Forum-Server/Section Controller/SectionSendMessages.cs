using System;
using System.Collections.Generic;

namespace ROIDForumServer;

public static class SectionSendMessages
{
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

        public static byte[] AllThreadsMessage(List<DatabaseThread.DatabaseThreadData> threads)
        {
            var message = new MessageWriter();
            message.AddUint8((byte)ServerSendControllers.Section);
            message.AddUint8((byte)SectionMsg.AllThreads);
            //message.AddString(controller.SectionName);
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

        public static byte[] AddThreadMessage(Guid creatorAccountId, Guid threadId, String title) {

            var message = new MessageWriter();
            message.AddUint8((byte)ServerSendControllers.Section);
            message.AddUint8((byte)SectionMsg.AddThread);
            //message.AddString(controller.SectionName);
            message.AddString(creatorAccountId.ToString());
            message.AddString(threadId.ToString());
            message.AddString(title);
            return message.ToBuffer();
        }

        public static byte[] RemoveThreadMessage(Guid threadId)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)ServerSendControllers.Section);
            message.AddUint8((byte)SectionMsg.RemoveThread);
            //message.AddString(controller.SectionName);
            message.AddString(threadId.ToString());
            return message.ToBuffer();
        }

        public static byte[] UpdateThreadMessage(Guid threadId, String title)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)ServerSendControllers.Section);
            message.AddUint8((byte)SectionMsg.UpdateThread);
            //message.AddString(controller.SectionName);
            message.AddString(threadId.ToString());
            message.AddString(title);
            return message.ToBuffer();
        }

        public static byte[] MoveToTopThreadMessage(Guid threadId)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)ServerSendControllers.Section);
            message.AddUint8((byte)SectionMsg.MoveThreadToTop);
            //message.AddString(controller.SectionName);
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
}