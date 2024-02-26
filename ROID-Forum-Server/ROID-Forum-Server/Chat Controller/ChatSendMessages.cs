using System;
using System.Collections.Generic;
using Cassandra;

namespace ROIDForumServer;

public static class ChatSendMessages
{
    private enum ChatMsg : byte
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
        List<(Guid creatorAccountId, Guid chatId, TimeUuid createdTime, string content)> chats)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMsg.AllMessages);
        message.AddUint32((uint)chats.Count);
        foreach ((Guid creatorAccountId, Guid chatId, TimeUuid createdTime, string content) in chats)
        {
            message.AddString(chatId.ToString());
            message.AddString(creatorAccountId.ToString());
            message.AddString(createdTime.ToString());
            message.AddString(content);
        }

        return message.ToBuffer();
    }

    public static byte[] NewMessage((Guid creatorAccountId, Guid chatId, TimeUuid createdTime, string content) chat)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMsg.NewMessage);
        message.AddString(chat.chatId.ToString());
        message.AddString(chat.creatorAccountId.ToString());
        message.AddString(chat.createdTime.ToString());
        message.AddString(chat.content);
        return message.ToBuffer();
    }

    public static byte[] AllOnlineList(List<(Guid connectionId, Guid? accountId, string displayName)> onlineAccounts)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMsg.AllOnlineList);
        foreach ((Guid connectionId, Guid? accountId, string displayName) in onlineAccounts)
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
        message.AddUint8((byte)ChatMsg.OnlineListAddUser);
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] RemoveUser(Guid connectionId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMsg.OnlineListRemoveUser);
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] LoggedInUser(Guid connectionId, Guid accountId, string displayName)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMsg.OnlineListLoggedInUser);
        message.AddString(connectionId.ToString());
        message.AddString(accountId.ToString());
        message.AddString(displayName);
        return message.ToBuffer();
    }

    public static byte[] LoggedOutUser(Guid connectionId)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMsg.OnlineListLoggedOutUser);
        message.AddString(connectionId.ToString());
        return message.ToBuffer();
    }

    public static byte[] DisplayNameUpdate(Guid accountId, string displayName)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMsg.DisplayNameUpdate);
        message.AddString(accountId.ToString());
        message.AddString(displayName);
        return message.ToBuffer();
    }
}