using System;
using System.Collections.Generic;
using Cassandra;
namespace ROIDForumServer
{
    public class ProfileController (ServerController serverController)
    {
        private ServerController Server { get; } = serverController;
        private ISession DatabaseSession { get; } = serverController.Database.GetSession();
        public void OnMessage(ConnectedUser user, MessageReader message)
        {
            /*
             * if ((string)message["Controller"] == "Server" && (string)message["Title"] == "Viewing")
               {
                   if (user.ViewingSectionId != null)
                   {
                       DisengageFromSection((Guid) user.ViewingSectionId, user);   
                   }
                   EngageToSection(Guid.Parse((string)message["Section ID"]), user);
               }
             */
            
            if ((string)message["Title"] == "Set Avatar" && user.AccountId != null)
            {
                DatabaseAccount.SetAvatarUrl(DatabaseSession, (Guid)user.AccountId, (string)message["AvatarURL"]);
            }
            if ((string)message["Title"] == "Get Avatar" && user.AccountId != null)
            {
                user.Send(ProfileSendMessages.GetAvatarMessage(DatabaseAccount.GetAvatarUrl(DatabaseSession, (Guid)user.AccountId)));
            }
            if ((string)message["Title"] == "Login" && user.AccountId == null)
            {
                string username = (string)message["Name"];
                string password = (string)message["Password"];
                if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password))
                {
                    user.Send(ProfileSendMessages.LoginFailedMessage());
                }
                else
                {
                    Guid? accountId = DatabaseAccount.GetAccountIdForCredentials(DatabaseSession, username, password);
                    if (accountId == null)
                    {
                        user.Send(ProfileSendMessages.LoginFailedMessage());
                    }
                    else
                    {
                        user.AccountId = accountId;
                        user.Send(ProfileSendMessages.LoggedInMessage(DatabaseAccount.GetAccountName(DatabaseSession, (Guid)user.AccountId)));
                        Server.AccountLoggedIn(user);
                    }
                }
            }
            if ((string)message["Title"] == "Logout" && user.AccountId != null)
            {
                //Perhaps an account method can be called for saving or other logic
                user.AccountId = null;
                //Send logout notification
                user.Send(ProfileSendMessages.LoggedOutMessage());
                Server.AccountLoggedOut(user);
            }
            if ((string)message["Title"] == "Register" && user.AccountId == null)
            {
                string username = (string)message["Name"];
                string password = (string)message["Password"];
                string email = (string)message["Email"];
                if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(email))
                {
                    user.Send(ProfileSendMessages.RegisterFailedMessage());
                    return;
                }
                var (createAccountStatus, accountId) = DatabaseAccount.CreateAccount(DatabaseSession, username, password, email);
                if (createAccountStatus == DatabaseAccount.CreateAccountStatus.AlreadyExists || accountId == null)
                {
                    user.Send(ProfileSendMessages.RegisterFailedMessage());
                    return;
                }
                // Set the account ID to log them in
                user.AccountId = accountId;
                //Send login notification
                user.Send(ProfileSendMessages.LoggedInMessage(DatabaseAccount.GetAccountName(DatabaseSession, (Guid)user.AccountId)));
                Server.AccountLoggedIn(user);
            }
        }
    }
}
