namespace ROIDForumServer;

public class ProfileSendMessages
{
    
        private enum LoginMsg {
            GetAvatar = 0,
            LoginFailed = 1,
            LoggedOut = 2,
            RegisterFailed = 3,
            LoggedIn = 4,
        }

        public static byte[] LoggedInMessage(string username)
        {
            var message = new MessageWriter();
            message.AddUint8((byte)ServerSendControllers.Profile);
            message.AddUint8((byte)LoginMsg.LoggedIn);
            message.AddString(username);

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

        public static byte[] GetAvatarMessage(string avatarUrl)
        {
            var message = new MessageWriter();
            message.AddUint8((byte)ServerSendControllers.Profile);
            message.AddUint8((byte)LoginMsg.GetAvatar);
            message.AddString(avatarUrl);
            return message.ToBuffer();
        }
}