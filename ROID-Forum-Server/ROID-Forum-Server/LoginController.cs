using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace ROIDForumServer
{
    public class LoginController
    {
        ServerController server;
        public LoginController(ServerController s)
        {
            server = s;
        }
        public void logic()
        {
        }
        public void onMessage(User u, Dictionary<string, object> message)
        {
            if ((string)message["Title"] == "Set Avatar" && u.account != null)
            {
                u.account.avatarURL = (string)message["AvatarURL"];
            }
            if ((string)message["Title"] == "Get Avatar" && u.account != null)
            {
                u.sendBinary(ServerMessages.GetAvatarMessage(u.account.avatarURL));
            }
            if ((string)message["Title"] == "Login" && u.account == null)
            {
                if (server.accountController.accountExists((string)message["Name"], (string)message["Password"]))
                {
                    u.account = server.accountController.getAccount((string)message["Name"], (string)message["Password"]);
                    u.sendBinary(ServerMessages.LoggedInMessage(u.account.name, u.account.password));

                    server.accountLoggedIn(u);
                }
                else
                {
                    //Send login failure
                    u.sendBinary(ServerMessages.LoginFailedMessage());
                }
            }
            if ((string)message["Title"] == "Logout" && u.account != null)
            {
                //Perhaps an account method can be called for saving or other logic
                u.account = null;
                //Send logout notification
                u.sendBinary(ServerMessages.LoggedOutMessage());
                server.accountLoggedOut(u);
            }
            if ((string)message["Title"] == "Register" && u.account == null)
            {
                if (server.accountController.accountNameExists((string)message["Name"]))
                {
                    u.sendBinary(ServerMessages.RegisterFailedMessage());
                }
                else
                {
                    u.account = server.accountController.createAccount((string)message["Name"], (string)message["Password"], (string)message["Email"]);
                    //Send login notification
                    u.sendBinary(ServerMessages.LoggedInMessage(u.account.name, u.account.password));
                    server.accountLoggedIn(u);
                }
            }
        }
    }
}
