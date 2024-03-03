using System;
using System.Collections.Generic;
using Cassandra;

namespace ROIDForumServer;

public static class ThreadSendMessages
{
    private enum ThreadMessage
    {
        AllThreadViewers = 0,
        ThreadAddViewer = 1,
        ThreadRemoveViewer = 2,
        ThreadLoggedInViewer = 3,
        ThreadLoggedOutViewer = 4,
        AllComments = 5,
        CommentSuccessfullyCreated = 6,
        AddComment = 7,
        RemoveComment = 8,
        UpdateComment = 9,
        AvatarUpdate = 10,
        DisplayNameUpdate = 11
    }

    public static byte[] AllComments(List<DatabaseComment.DatabaseCommentData> comments)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMessage.AllComments);
        var count = (UInt32)comments.Count;
        message.AddUint32(count);
        foreach ((Guid sectionId, Guid threadId, TimeUuid commentIdAndCreatedTime, Guid creatorAccountId, String text, string creatorDisplayName, string creatorAvatarUrl) in comments)
        {
            message.AddString(sectionId.ToString());
            message.AddString(threadId.ToString());
            message.AddString(commentIdAndCreatedTime.ToString());
            message.AddString(creatorAccountId.ToString());
            message.AddString(text);
            message.AddString(creatorDisplayName);
            message.AddString(creatorAvatarUrl);
        }

        return message.ToBuffer();
    }

    public static byte[] CommentSuccessfullyCreated(Guid sectionId, Guid threadId, Guid commentId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMessage.CommentSuccessfullyCreated);
        message.AddString(sectionId.ToString());
        message.AddString(threadId.ToString());
        message.AddString(commentId.ToString());
        return message.ToBuffer();
    }

    public static byte[] AddComment(DatabaseComment.DatabaseCommentData comment)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMessage.AddComment);
        message.AddString(comment.sectionId.ToString());
        message.AddString(comment.threadId.ToString());
        message.AddString(comment.commentIdAndCreatedTime.ToString());
        message.AddString(comment.creatorAccountId.ToString());
        message.AddString(comment.text);
        message.AddString(comment.creatorDisplayName);
        message.AddString(comment.creatorAvatarUrl);

        return message.ToBuffer();
    }

    public static byte[] UpdateComment(Guid sectionId, Guid threadId, Guid commentId, string text)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMessage.UpdateComment);
        message.AddString(sectionId.ToString());
        message.AddString(threadId.ToString());
        message.AddString(commentId.ToString());
        message.AddString(text);

        return message.ToBuffer();
    }

    public static byte[] RemoveComment(Guid sectionId, Guid threadId, Guid commentId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMessage.RemoveComment);
        message.AddString(sectionId.ToString());
        message.AddString(threadId.ToString());
        message.AddString(commentId.ToString());

        return message.ToBuffer();
    }

    public static byte[] AllThreadViewers(List<(Guid connectionId, Guid? accountId, string displayName)> viewingUsers,
        Guid threadId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMessage.AllThreadViewers);
        message.AddString(threadId.ToString());
        foreach ((Guid connectionId, Guid? accountId, string displayName) in viewingUsers)
        {
            message.AddString(connectionId.ToString());
            message.AddUint8(accountId == null ? (byte)0 : (byte)1);
            if (accountId != null)
            {
                message.AddString(accountId.ToString());
                message.AddString(displayName);
            }
        }

        return message.ToBuffer();
    }

    public static byte[] ThreadAddViewer(Guid connectionId, Guid? accountId, string displayName, Guid threadId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMessage.ThreadAddViewer);
        message.AddString(threadId.ToString());
        message.AddString(connectionId.ToString());
        if (accountId != null)
        {
            message.AddString(accountId.ToString());
            message.AddString(displayName);
        }

        return message.ToBuffer();
    }

    public static byte[] ThreadRemoveViewer(Guid connectionId, Guid threadId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMessage.ThreadRemoveViewer);
        message.AddString(threadId.ToString());
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] ThreadLoggedInUser(Guid connectionId, Guid accountId, string displayName, Guid threadId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMessage.ThreadLoggedInViewer);
        message.AddString(threadId.ToString());
        message.AddString(connectionId.ToString());
        message.AddString(accountId.ToString());
        message.AddString(displayName);
        return message.ToBuffer();
    }

    public static byte[] ThreadLoggedOutUser(Guid connectionId, Guid threadId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMessage.ThreadLoggedOutViewer);
        message.AddString(threadId.ToString());
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] DisplayNameUpdate(Guid accountId, string displayName)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMessage.DisplayNameUpdate);
        message.AddString(accountId.ToString());
        message.AddString(displayName);
        return message.ToBuffer();
    }

    public static byte[] AvatarUpdate(Guid accountId, string avatarUrl)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMessage.AvatarUpdate);
        message.AddString(accountId.ToString());
        message.AddString(avatarUrl);
        return message.ToBuffer();
    }
}