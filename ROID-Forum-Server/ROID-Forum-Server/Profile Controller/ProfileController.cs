﻿using System;

namespace ROIDForumServer
{
    public static class ProfileController
    {
        public static void OnMessage(ServerState serverState, ConnectedUser user, MessageReader message)
        {
            if (!message.HasUint8()) return;

            byte messageId = message.GetUint8();
            if ((byte)ProfileReceiveMessages.SetAvatar == messageId)
            {
                if (!message.HasString()) return;
                if (user.AccountId == null) return;
                DatabaseAccount.SetAvatarUrl(serverState.Database.GetSession(), (Guid)user.AccountId, message.GetString());
                ServerController.OnUserAvatarChanged(serverState, user);
            } else if ((byte)ProfileReceiveMessages.UpdateDisplayName == messageId)
            {
                if (!message.HasString()) return;
                if (user.AccountId == null) return;
                DatabaseAccount.SetDisplayName(serverState.Database.GetSession(), (Guid)user.AccountId, message.GetString());
                ServerController.OnUserDisplayNameChanged(serverState, user);
            }
            else if ((byte)ProfileReceiveMessages.GetAvatar == messageId)
            {
                if (user.AccountId == null) return;
                user.Send(ProfileSendMessages.ReturnAvatarMessage(
                    DatabaseAccount.GetAvatarUrl(serverState.Database.GetSession(), (Guid)user.AccountId)));
            }
            else if ((byte)ProfileReceiveMessages.GetDisplayName == messageId)
            {
                if (user.AccountId == null) return;
                user.Send(ProfileSendMessages.ReturnDisplayNameMessage(
                    DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(), (Guid)user.AccountId)));
            }
            else if ((byte)ProfileReceiveMessages.Login == messageId)
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
                    Guid? accountId = DatabaseAccount.GetAccountIdForCredentials(serverState.Database.GetSession(), username, password);
                    if (accountId == null)
                    {
                        user.Send(ProfileSendMessages.LoginFailedMessage());
                    }
                    else
                    {
                        user.AccountId = accountId;
                        user.Send(ProfileSendMessages.LoggedInMessage(
                            DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(), (Guid)user.AccountId), user.AccountId.ToString()));
                        ServerController.OnUserLoggedIn(serverState, user);
                    }
                }
            }
            else if ((byte)ProfileReceiveMessages.Logout == messageId)
            {
                if (user.AccountId == null) return;
                user.AccountId = null;
                user.Send(ProfileSendMessages.LoggedOutMessage());
                ServerController.OnUserLoggedOut(serverState, user);
            }
            else if ((byte)ProfileReceiveMessages.Register == messageId)
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
                    DatabaseAccount.CreateAccount(serverState.Database.GetSession(), username, password, email);
                if (createAccountStatus == DatabaseAccount.CreateAccountStatus.AlreadyExists || accountId == null)
                {
                    user.Send(ProfileSendMessages.RegisterFailedMessage());
                    return;
                }

                // Set the account ID to log them in
                user.AccountId = accountId;
                //Send login notification
                user.Send(ProfileSendMessages.LoggedInMessage(
                    DatabaseAccount.GetAccountDisplayName(serverState.Database.GetSession(), (Guid)user.AccountId), user.AccountId.ToString()));
                ServerController.OnUserLoggedIn(serverState, user);
            }
        }
    }
}