namespace ROIDForumServer;

public static class ProfileSendMessages
{
    private enum LoginMsg
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
        message.AddUint8((byte)LoginMsg.LoggedIn);
        message.AddString(displayName);

        return message.ToBuffer();
    }

    public static byte[] RegisterFailedMessage()
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Profile);
        message.AddUint8((byte)LoginMsg.RegisterFailed);
        return message.ToBuffer();
    }

    public static byte[] LoggedOutMessage()
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Profile);
        message.AddUint8((byte)LoginMsg.LoggedOut);
        return message.ToBuffer();
    }

    public static byte[] LoginFailedMessage()
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Profile);
        message.AddUint8((byte)LoginMsg.LoginFailed);
        return message.ToBuffer();
    }

    public static byte[] ReturnAvatarMessage(string avatarUrl)
    {
        var message = new MessageWriter();
        message.AddUint8((byte)ServerSendControllers.Profile);
        message.AddUint8((byte)LoginMsg.ReturnAvatar);
        message.AddString(avatarUrl);
        return message.ToBuffer();
    }
}