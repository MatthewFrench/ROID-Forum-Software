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
        public void onMessage(ConnectedUser u, Dictionary<string, object> message)
        {
            if ((string)message["Title"] == "Set Avatar" && u.accountID != null)
            {
                server.GetDatabase().SetAvatarUrl((Guid)u.accountID, (string)message["AvatarURL"]);
            }
            if ((string)message["Title"] == "Get Avatar" && u.accountID != null)
            {
                u.sendBinary(ServerMessages.GetAvatarMessage(server.GetDatabase().GetAvatarUrl((Guid)u.accountID)));
            }
            if ((string)message["Title"] == "Login" && u.accountID == null)
            {
                string username = (string)message["Name"];
                string password = (string)message["Password"];
                if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
                {
                    u.sendBinary(ServerMessages.LoginFailedMessage());
                }
                else
                {
                    Guid? accountID = server.GetDatabase().GetAccountIDForCredentials(username, password);
                    if (accountID == null)
                    {
                        u.sendBinary(ServerMessages.LoginFailedMessage());
                    }
                    else
                    {
                        u.accountID = accountID;
                        u.sendBinary(ServerMessages.LoggedInMessage(server.GetDatabase().GetAccountName((Guid)u.accountID)));
                        server.accountLoggedIn(u);
                    }
                }
            }
            if ((string)message["Title"] == "Logout" && u.accountID != null)
            {
                //Perhaps an account method can be called for saving or other logic
                u.accountID = null;
                //Send logout notification
                u.sendBinary(ServerMessages.LoggedOutMessage());
                server.accountLoggedOut(u);
            }
            if ((string)message["Title"] == "Register" && u.accountID == null)
            {
                string username = (string)message["Name"];
                string password = (string)message["Password"];
                string email = (string)message["Email"];
                if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(email))
                {
                    u.sendBinary(ServerMessages.RegisterFailedMessage());
                    return;
                }
                var (createAccountStatus, accountID) = server.GetDatabase().CreateAccount(username, password, email);
                if (createAccountStatus == Database.CreateAccountStatus.AlreadyExists || accountID == null)
                {
                    u.sendBinary(ServerMessages.RegisterFailedMessage());
                    return;
                }
                // Set the account ID to log them in
                u.accountID = accountID;
                //Send login notification
                u.sendBinary(ServerMessages.LoggedInMessage(server.GetDatabase().GetAccountName((Guid)u.accountID)));
                server.accountLoggedIn(u);
            }
        }
    }
}
