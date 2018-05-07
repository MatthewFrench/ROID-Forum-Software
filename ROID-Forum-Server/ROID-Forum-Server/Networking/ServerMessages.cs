using System;
namespace ROIDForumServer
{
    public class ServerMessages
    {
        public enum Controller
        {
            Chat = 0,
            Login = 1,
            Server = 2,
            Section = 3
        }

        public enum ChatMsg
        {
            Msg = 0,
            OnlineList = 1
        }
        
        public static byte[] SendChatMessage(String chat) {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Chat);
            message.AddUint8((byte)ChatMsg.Msg);
            message.AddString(chat);
            return message.ToBuffer();
        }

        public static byte[] SendChatOnlineList(String list)
        {
            var message = new MessageWriter();
            message.AddUint8((byte)Controller.Chat);
            message.AddUint8((byte)ChatMsg.OnlineList);
            message.AddString(list);
            return message.ToBuffer();
        }
    }
}
