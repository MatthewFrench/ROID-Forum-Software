using System;
using System.Collections.Generic;
using Cassandra;

namespace ROIDForumServer;

public static class SectionSendMessages
{
    private enum SectionMsg
    {
        AllSectionViewers = 0,
        SectionAddViewer = 1,
        SectionRemoveViewer = 2,
        SectionLoggedInViewer = 3,
        SectionLoggedOutViewer = 4,
        AllSectionHeaders = 5,
        AllThreadHeaders = 6,
        ThreadSuccessfullyCreated = 7,
        AddThread = 8,
        RemoveThread = 9,
        UpdateThreadTitle = 10,
        UpdateThreadDescription = 11,
        MoveThreadToTop = 12,
        AvatarUpdate = 13,
        DisplayNameUpdate = 14
    }

    public static byte[] AllSectionHeaders(List<(Guid sectionId, string name)> sections)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMsg.AllSectionHeaders);
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
        message.AddUint8((byte)SectionMsg.AllThreadHeaders);
        foreach ((Guid sectionId, Guid threadId, Guid creatorAccountId, string title, string description, TimeUuid createdTime, TimeUuid updatedTime, uint commentCount, string creatorDisplayName, string creatorAvatarUrl) in threadHeaders)
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

    public static byte[] AllSectionViewers(List<(Guid connectionId, Guid? accountId, string displayName)> viewingUsers,
        Guid sectionId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMsg.AllSectionViewers);
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
        message.AddUint8((byte)SectionMsg.SectionAddViewer);
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
        message.AddUint8((byte)SectionMsg.SectionRemoveViewer);
        message.AddString(sectionId.ToString());
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] SectionLoggedInUser(Guid connectionId, Guid accountId, string displayName, Guid sectionId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Section);
        message.AddUint8((byte)SectionMsg.SectionLoggedInViewer);
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
        message.AddUint8((byte)SectionMsg.SectionLoggedOutViewer);
        message.AddString(sectionId.ToString());
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] DisplayNameUpdate(Guid accountId, string displayName)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)SectionMsg.DisplayNameUpdate);
        message.AddString(accountId.ToString());
        message.AddString(displayName);
        return message.ToBuffer();
    }

    public static byte[] AvatarUpdate(Guid accountId, string avatarUrl)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)SectionMsg.AvatarUpdate);
        message.AddString(accountId.ToString());
        message.AddString(avatarUrl);
        return message.ToBuffer();
    }
}