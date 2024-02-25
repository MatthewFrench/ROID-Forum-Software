﻿using System;
using Cassandra;

namespace ROIDForumServer
{
    public class ProfileController(ServerController serverController)
    {
        private ServerController Server { get; } = serverController;
        private ISession DatabaseSession { get; } = serverController.Database.GetSession();

        public void OnMessage(ConnectedUser user, MessageReader message)
        {
            if (!message.HasUint8()) return;

            byte messageId = message.GetUint8();
            if (ProfileReceiveMessages.ViewingSection.Equals(messageId))
            {
                if (!message.HasString()) return;
                Guid viewingSectionId = Guid.Parse(message.GetString());
                if (user.ViewingSectionId != null)
                {
                    Server.DisengageFromSection((Guid)user.ViewingSectionId, user);
                }

                Server.EngageToSection(viewingSectionId, user);
            }
            else if (ProfileReceiveMessages.SetAvatar.Equals(messageId))
            {
                if (!message.HasString()) return;
                if (user.AccountId == null) return;
                DatabaseAccount.SetAvatarUrl(DatabaseSession, (Guid)user.AccountId, message.GetString());
            }
            else if (ProfileReceiveMessages.GetAvatar.Equals(messageId))
            {
                if (user.AccountId == null) return;
                user.Send(ProfileSendMessages.GetAvatarMessage(
                    DatabaseAccount.GetAvatarUrl(DatabaseSession, (Guid)user.AccountId)));
            }
            else if (ProfileReceiveMessages.Login.Equals(messageId))
            {
                if (user.AccountId != null) return;
                if (!message.HasString()) return;
                string username = message.GetString();
                if (!message.HasString()) return;
                string password = message.GetString();
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
                        user.Send(ProfileSendMessages.LoggedInMessage(
                            DatabaseAccount.GetAccountDisplayName(DatabaseSession, (Guid)user.AccountId)));
                        Server.AccountLoggedIn(user);
                    }
                }
            }
            else if (ProfileReceiveMessages.Logout.Equals(messageId))
            {
                if (user.AccountId == null) return;
                user.AccountId = null;
                user.Send(ProfileSendMessages.LoggedOutMessage());
                Server.AccountLoggedOut(user);
            }
            else if (ProfileReceiveMessages.Register.Equals(messageId))
            {
                if (user.AccountId != null) return;
                if (!message.HasString()) return;
                string username = message.GetString();
                if (!message.HasString()) return;
                string password = message.GetString();
                if (!message.HasString()) return;
                string email = message.GetString();
                if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(password) ||
                    String.IsNullOrWhiteSpace(email))
                {
                    user.Send(ProfileSendMessages.RegisterFailedMessage());
                    return;
                }

                var (createAccountStatus, accountId) =
                    DatabaseAccount.CreateAccount(DatabaseSession, username, password, email);
                if (createAccountStatus == DatabaseAccount.CreateAccountStatus.AlreadyExists || accountId == null)
                {
                    user.Send(ProfileSendMessages.RegisterFailedMessage());
                    return;
                }

                // Set the account ID to log them in
                user.AccountId = accountId;
                //Send login notification
                user.Send(ProfileSendMessages.LoggedInMessage(
                    DatabaseAccount.GetAccountDisplayName(DatabaseSession, (Guid)user.AccountId)));
                Server.AccountLoggedIn(user);
            }
        }
    }
}