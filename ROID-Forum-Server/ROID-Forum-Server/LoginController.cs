using System;
using System.Collections.Generic;
using Cassandra;
namespace ROIDForumServer
{
    public class LoginController (ServerController serverController)
    {
        private ServerController Server { get; } = serverController;
        private ISession DatabaseSession { get; } = serverController.Database.GetSession();
        public void OnMessage(ConnectedUser user, Dictionary<string, object> message)
        {
            if ((string)message["Title"] == "Set Avatar" && user.AccountId != null)
            {
                DatabaseAccount.SetAvatarUrl(DatabaseSession, (Guid)user.AccountId, (string)message["AvatarURL"]);
            }
            if ((string)message["Title"] == "Get Avatar" && user.AccountId != null)
            {
                user.Send(ServerMessages.GetAvatarMessage(DatabaseAccount.GetAvatarUrl(DatabaseSession, (Guid)user.AccountId)));
            }
            if ((string)message["Title"] == "Login" && user.AccountId == null)
            {
                string username = (string)message["Name"];
                string password = (string)message["Password"];
                if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
                {
                    user.Send(ServerMessages.LoginFailedMessage());
                }
                else
                {
                    Guid? accountId = DatabaseAccount.GetAccountIdForCredentials(DatabaseSession, username, password);
                    if (accountId == null)
                    {
                        user.Send(ServerMessages.LoginFailedMessage());
                    }
                    else
                    {
                        user.AccountId = accountId;
                        user.Send(ServerMessages.LoggedInMessage(DatabaseAccount.GetAccountName(DatabaseSession, (Guid)user.AccountId)));
                        Server.AccountLoggedIn(user);
                    }
                }
            }
            if ((string)message["Title"] == "Logout" && user.AccountId != null)
            {
                //Perhaps an account method can be called for saving or other logic
                user.AccountId = null;
                //Send logout notification
                user.Send(ServerMessages.LoggedOutMessage());
                Server.AccountLoggedOut(user);
            }
            if ((string)message["Title"] == "Register" && user.AccountId == null)
            {
                string username = (string)message["Name"];
                string password = (string)message["Password"];
                string email = (string)message["Email"];
                if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(email))
                {
                    user.Send(ServerMessages.RegisterFailedMessage());
                    return;
                }
                var (createAccountStatus, accountId) = DatabaseAccount.CreateAccount(DatabaseSession, username, password, email);
                if (createAccountStatus == DatabaseAccount.CreateAccountStatus.AlreadyExists || accountId == null)
                {
                    user.Send(ServerMessages.RegisterFailedMessage());
                    return;
                }
                // Set the account ID to log them in
                user.AccountId = accountId;
                //Send login notification
                user.Send(ServerMessages.LoggedInMessage(DatabaseAccount.GetAccountName(DatabaseSession, (Guid)user.AccountId)));
                Server.AccountLoggedIn(user);
            }
        }
    }
}
