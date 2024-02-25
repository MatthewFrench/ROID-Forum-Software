using System;
using System.Collections.Generic;

namespace ROIDForumServer;

public static class SectionSendMessages
{
        private enum SectionMsg {
            AllThreadHeaders = 0,
            AddThread = 1,
            RemoveThread = 2,
            UpdateThreadTitle = 3,
            UpdateThreadDescription = 4,
            MoveThreadToTop = 5,
            ThreadAndComments = 6,
            AddComment = 7,
            RemoveComment = 8,
            UpdateComment = 9,
            SectionViewers = 10,
            SectionAddViewer = 11,
            SectionRemoveViewer = 12,
            ThreadViewers = 13,
            ThreadAddViewer = 14,
            ThreadRemoveViewer = 15,
            AvatarUpdate = 16,
            DisplayNameUpdate = 17
        }

        public static byte[] AllThreadHeaders(List<DatabaseThread.DatabaseThreadHeaderData> threadHeaders)
        {
            var message = new MessageWriter();
            message.AddUint8((byte)ServerSendControllers.Section);
            message.AddUint8((byte)SectionMsg.AllThreadHeaders);
            message.AddUint32((uint)threadHeaders.Count);
            foreach (var threadHeader in threadHeaders)
            {
                message.AddString(threadHeader.sectionId.ToString());
                message.AddString(threadHeader.threadId.ToString());
                message.AddString(threadHeader.creatorAccountId.ToString());
                message.AddString(threadHeader.createdTime.ToString());
                message.AddString(threadHeader.updated_time.ToString());
                message.AddString(threadHeader.title);
            }
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

        public static byte[] UpdateThreadTitleMessage(Guid threadId, String title)
        {
            // Todo: This needs updated on client side
            var message = new MessageWriter();
            message.AddUint8((byte)ServerSendControllers.Section);
            message.AddUint8((byte)SectionMsg.UpdateThreadTitle);
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