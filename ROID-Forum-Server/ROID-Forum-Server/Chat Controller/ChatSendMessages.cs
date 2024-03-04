using System;
using System.Collections.Generic;
using Cassandra;

namespace ROIDForumServer;

public static class ChatSendMessages
{
    private enum ChatMessage : byte
    {
        // Todo: Add chat pagination
        AllMessages = 0,
        NewMessage = 1,
        AllOnlineList = 2,
        OnlineListAddUser = 3,
        OnlineListRemoveUser = 4,
        OnlineListLoggedInUser = 5,
        OnlineListLoggedOutUser = 6,
        DisplayNameUpdate = 7,
    }

    public static byte[] AllMessages(
        List<(Guid creatorAccountId, string creatorDisplayName, Guid chatId, TimeUuid createdTime, string content)> chats)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMessage.AllMessages);
        message.AddUint32(Convert.ToUInt32(chats.Count));
        foreach ((Guid creatorAccountId, string creatorDisplayName, Guid chatId, TimeUuid createdTime, string content) in chats)
        {
            message.AddString(chatId.ToString());
            message.AddString(creatorAccountId.ToString());
            message.AddString(creatorDisplayName);
            message.AddString(createdTime.ToString());
            message.AddString(content);
        }

        return message.ToBuffer();
    }

    public static byte[] NewMessage((Guid creatorAccountId, string creatorDisplayName, Guid chatId, TimeUuid createdTime, string content) chat)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMessage.NewMessage);
        message.AddString(chat.chatId.ToString());
        message.AddString(chat.creatorAccountId.ToString());
        message.AddString(chat.creatorDisplayName);
        message.AddString(chat.createdTime.ToString());
        message.AddString(chat.content);
        return message.ToBuffer();
    }

    public static byte[] AllOnlineList(List<(Guid connectionId, Guid? accountId, string displayName)> users)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMessage.AllOnlineList);
        message.AddUint32((UInt32)users.Count);
        foreach ((Guid connectionId, Guid? accountId, string displayName) in users)
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

    public static byte[] AddUser(Guid connectionId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMessage.OnlineListAddUser);
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] RemoveUser(Guid connectionId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMessage.OnlineListRemoveUser);
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] LoggedInUser(Guid connectionId, Guid accountId, string displayName)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMessage.OnlineListLoggedInUser);
        message.AddString(connectionId.ToString());
        message.AddString(accountId.ToString());
        message.AddString(displayName);
        return message.ToBuffer();
    }

    public static byte[] LoggedOutUser(Guid connectionId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMessage.OnlineListLoggedOutUser);
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] DisplayNameUpdate(Guid accountId, string displayName)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMessage.DisplayNameUpdate);
        message.AddString(accountId.ToString());
        message.AddString(displayName);
        return message.ToBuffer();
    }
}