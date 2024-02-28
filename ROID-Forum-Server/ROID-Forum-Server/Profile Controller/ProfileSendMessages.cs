namespace ROIDForumServer;

public static class ProfileSendMessages
{
    private enum ProfileMessage
    {
        ReturnAvatar = 0,
        LoginFailed = 1,
        LoggedOut = 2,
        RegisterFailed = 3,
        LoggedIn = 4,
        ReturnDisplayName = 5,
    }

    public static byte[] LoggedInMessage(string displayName)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Profile);
        message.AddUint8((byte)ProfileMessage.LoggedIn);
        message.AddString(displayName);

        return message.ToBuffer();
    }

    public static byte[] RegisterFailedMessage()
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Profile);
        message.AddUint8((byte)ProfileMessage.RegisterFailed);
        return message.ToBuffer();
    }

    public static byte[] LoggedOutMessage()
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Profile);
        message.AddUint8((byte)ProfileMessage.LoggedOut);
        return message.ToBuffer();
    }

    public static byte[] LoginFailedMessage()
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Profile);
        message.AddUint8((byte)ProfileMessage.LoginFailed);
        return message.ToBuffer();
    }

    public static byte[] ReturnAvatarMessage(string avatarUrl)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Profile);
        message.AddUint8((byte)ProfileMessage.ReturnAvatar);
        message.AddString(avatarUrl);
        return message.ToBuffer();
    }

    public static byte[] ReturnDisplayNameMessage(string displayName)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Profile);
        message.AddUint8((byte)ProfileMessage.ReturnDisplayName);
        message.AddString(displayName);
        return message.ToBuffer();
    }
}