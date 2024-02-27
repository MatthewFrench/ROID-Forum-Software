using System;
using System.Collections.Generic;
using Cassandra;

namespace ROIDForumServer;

public static class ThreadSendMessages
{
    private enum ThreadMsg
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
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)ThreadMsg.AllComments);
        foreach ((Guid threadId, Guid commentId, Guid creatorAccountId, String text, TimeUuid createdTime) in comments)
        {
            message.AddString(threadId.ToString());
            message.AddString(commentId.ToString());
            message.AddString(creatorAccountId.ToString());
            message.AddString(text);
            message.AddString(createdTime.ToString());
        }

        return message.ToBuffer();
    }

    public static byte[] AllThreadViewers(List<(Guid connectionId, Guid? accountId, string displayName)> viewingUsers,
        Guid threadId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMsg.AllThreadViewers);
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
        message.AddUint8((byte)ThreadMsg.ThreadAddViewer);
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
        message.AddUint8((byte)ThreadMsg.ThreadRemoveViewer);
        message.AddString(threadId.ToString());
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] ThreadLoggedInUser(Guid connectionId, Guid accountId, string displayName, Guid threadId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Thread);
        message.AddUint8((byte)ThreadMsg.ThreadLoggedInViewer);
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
        message.AddUint8((byte)ThreadMsg.ThreadLoggedOutViewer);
        message.AddString(threadId.ToString());
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] DisplayNameUpdate(Guid accountId, string displayName)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ThreadMsg.DisplayNameUpdate);
        message.AddString(accountId.ToString());
        message.AddString(displayName);
        return message.ToBuffer();
    }

    public static byte[] AvatarUpdate(Guid accountId, string avatarUrl)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ThreadMsg.AvatarUpdate);
        message.AddString(accountId.ToString());
        message.AddString(avatarUrl);
        return message.ToBuffer();
    }
}