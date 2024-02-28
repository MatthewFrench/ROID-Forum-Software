using System;
using System.Collections.Generic;
using Cassandra;

namespace ROIDForumServer;

public static class SectionSendMessages
{
    private enum SectionMessage
    {
        AllSectionViewers = 0,
        SectionAddViewer = 1,
        SectionRemoveViewer = 2,
        SectionLoggedInViewer = 3,
        SectionLoggedOutViewer = 4,
        AllSectionHeaders = 5,
        AllThreadHeaders = 6,
        ThreadSuccessfullyCreated = 7,
        AddThreadHeader = 8,
        RemoveThreadHeader = 9,
        UpdateThreadTitleAndDescription = 10,
        UpdateThreadCommentCountAndUpdatedTime = 11,
        UpdateThreadCommentCount = 12,
        AvatarUpdate = 13,
        DisplayNameUpdate = 14
    }

    public static byte[] AllSectionHeaders(List<(Guid sectionId, string name)> sections)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.AllSectionHeaders);
        foreach ((Guid sectionId, string name) in sections)
        {
            message.AddString(sectionId.ToString());
            message.AddString(name);
        }

        return message.ToBuffer();
    }

    public static byte[] AllThreadHeaders(List<DatabaseThread.DatabaseThreadHeaderData> threadHeaders)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.AllThreadHeaders);
        foreach ((Guid sectionId, Guid threadId, Guid creatorAccountId, string title, string description,
                     TimeUuid createdTime, TimeUuid updatedTime, uint commentCount, string creatorDisplayName,
                     string creatorAvatarUrl) in threadHeaders)
        {
            message.AddString(sectionId.ToString());
            message.AddString(threadId.ToString());
            message.AddString(creatorAccountId.ToString());
            message.AddString(title);
            message.AddString(description);
            message.AddString(createdTime.ToString());
            message.AddString(updatedTime.ToString());
            message.AddUint32(commentCount);
            message.AddString(creatorDisplayName);
            message.AddString(creatorAvatarUrl);
        }

        return message.ToBuffer();
    }

    public static byte[] AddThreadHeader(DatabaseThread.DatabaseThreadHeaderData threadHeaderData)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.AddThreadHeader);
        message.AddString(threadHeaderData.sectionId.ToString());
        message.AddString(threadHeaderData.threadId.ToString());
        message.AddString(threadHeaderData.creatorAccountId.ToString());
        message.AddString(threadHeaderData.title);
        message.AddString(threadHeaderData.description);
        message.AddString(threadHeaderData.createdTime.ToString());
        message.AddString(threadHeaderData.updatedTime.ToString());
        message.AddUint32(threadHeaderData.commentCount);
        message.AddString(threadHeaderData.creatorDisplayName);
        message.AddString(threadHeaderData.creatorAvatarUrl);

        return message.ToBuffer();
    }

    public static byte[] RemoveThreadHeader(Guid sectionId, Guid threadId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.RemoveThreadHeader);
        message.AddString(sectionId.ToString());
        message.AddString(threadId.ToString());

        return message.ToBuffer();
    }

    public static byte[] UpdateThreadTitleAndDescription(Guid sectionId, Guid threadId, string title, string description)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.UpdateThreadTitleAndDescription);
        message.AddString(sectionId.ToString());
        message.AddString(threadId.ToString());
        message.AddString(title);
        message.AddString(description);

        return message.ToBuffer();
    }

    public static byte[] UpdateThreadCommentCountAndUpdatedTime(Guid sectionId, Guid threadId, UInt32 commentCount, TimeUuid updatedTime)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.UpdateThreadCommentCountAndUpdatedTime);
        message.AddString(sectionId.ToString());
        message.AddString(threadId.ToString());
        message.AddUint32(commentCount);
        message.AddString(updatedTime.ToString());

        return message.ToBuffer();
    }

    public static byte[] UpdateThreadCommentCount(Guid sectionId, Guid threadId, UInt32 commentCount)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.UpdateThreadCommentCount);
        message.AddString(sectionId.ToString());
        message.AddString(threadId.ToString());
        message.AddUint32(commentCount);

        return message.ToBuffer();
    }

    public static byte[] AllSectionViewers(List<(Guid connectionId, Guid? accountId, string displayName)> viewingUsers,
        Guid sectionId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.AllSectionViewers);
        message.AddString(sectionId.ToString());
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

    public static byte[] SectionAddViewer(Guid connectionId, Guid? accountId, string displayName, Guid sectionId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.SectionAddViewer);
        message.AddString(sectionId.ToString());
        message.AddString(connectionId.ToString());
        if (accountId != null)
        {
            message.AddString(accountId.ToString());
            message.AddString(displayName);
        }

        return message.ToBuffer();
    }

    public static byte[] SectionRemoveViewer(Guid connectionId, Guid sectionId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.SectionRemoveViewer);
        message.AddString(sectionId.ToString());
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] ThreadSuccessfullyCreated(Guid sectionId, Guid threadId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.ThreadSuccessfullyCreated);
        message.AddString(sectionId.ToString());
        message.AddString(threadId.ToString());
        return message.ToBuffer();
    }

    public static byte[] SectionLoggedInUser(Guid connectionId, Guid accountId, string displayName, Guid sectionId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.SectionLoggedInViewer);
        message.AddString(sectionId.ToString());
        message.AddString(connectionId.ToString());
        message.AddString(accountId.ToString());
        message.AddString(displayName);
        return message.ToBuffer();
    }

    public static byte[] SectionLoggedOutUser(Guid connectionId, Guid sectionId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMessage.SectionLoggedOutViewer);
        message.AddString(sectionId.ToString());
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] DisplayNameUpdate(Guid accountId, string displayName)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)SectionMessage.DisplayNameUpdate);
        message.AddString(accountId.ToString());
        message.AddString(displayName);
        return message.ToBuffer();
    }

    public static byte[] AvatarUpdate(Guid accountId, string avatarUrl)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)SectionMessage.AvatarUpdate);
        message.AddString(accountId.ToString());
        message.AddString(avatarUrl);
        return message.ToBuffer();
    }
}