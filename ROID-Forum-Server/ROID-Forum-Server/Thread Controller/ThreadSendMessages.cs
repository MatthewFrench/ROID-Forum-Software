namespace ROIDForumServer;


public static class ThreadSendMessages
{
        private enum ThreadMsg {
            AllThreadViewers = 0,
            ThreadAddViewer = 1,
            ThreadRemoveViewer = 2,
            ThreadLoggedInViewer = 3,
            ThreadLoggedOutViewer = 4,
            ThreadAndAllComments = 5,
            AddComment = 6,
            RemoveComment = 7,
            UpdateComment = 8,
            AvatarUpdate = 9,
            DisplayNameUpdate = 10
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