using System;
using System.Collections.Generic;

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
        AllThreadHeaders = 5,
        AddThread = 6,
        RemoveThread = 7,
        UpdateThreadTitle = 8,
        UpdateThreadDescription = 9,
        MoveThreadToTop = 10,
        AvatarUpdate = 11,
        DisplayNameUpdate = 12
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