namespace ROIDForumServer;

public static class ChatSendMessages
{
    private enum ChatMsg : byte
    {
        Message = 0,
        OnlineList = 1,
        OnlineListAddUser = 2,
        OnlineListRemoveUser = 3,
        DisplayNameUpdate = 4,
    }
    
    public static byte[] ChatMessage(string chat) {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMsg.Message);
        message.AddString(chat);
        return message.ToBuffer();
    }

    public static byte[] ChatOnlineList(string list)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Chat);
        message.AddUint8((byte)ChatMsg.OnlineList);
        message.AddString(list);
        return message.ToBuffer();
    }
}