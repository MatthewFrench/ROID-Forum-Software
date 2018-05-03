library UserMsgHandler;
import 'User.dart';
import 'UserMsgCreator.dart';
import '../Networking/MessageReader.dart';
import '../Database/Account.dart';

const int User_Controller_Binary = 0,
    Forum_Controller_Binary = 1;

//Messages from the client
const int MSG_PING = 0;
const int MSG_SET_AVATAR = 1;
const int MSG_GET_AVATAR = 2;
const int MSG_LOGIN = 3;
const int MSG_LOGOUT = 4;
const int MSG_REGISTER = 5;
const int MSG_PING_FINAL = 6;

class UserMsgHandler {
  User user;

  UserMsgHandler(User u) {
    user = u;
  }
  void onBinaryMessage(MessageReader message) {
    int controller = message.getUint8();
    switch (controller) {
      case Forum_Controller_Binary: {
        user.server.forumController.handleUserMessage(user, message);
      } break;
      case User_Controller_Binary:
        {
          int binaryMsg = message.getUint8();
          switch (binaryMsg) {
            case MSG_PING:
              {
                user.pingTracker.gotFirstPing();
              }
              break;
            case MSG_PING_FINAL:
              {
                user.pingTracker.gotFinalPing();
              }
              break;
            case MSG_SET_AVATAR:
              {
                if (user.account == null) return;
                user.account.avatarURL = message.getString();
                user.server.databaseController.updateAccountAvatarURL(user.account.id, user.account.avatarURL);
              }
              break;
            case MSG_GET_AVATAR:
              {
                if (user.account == null) return;
                user.sendBinary(UserMsgCreator.Avatar(user.account.avatarURL), true);
              }
              break;
            case MSG_LOGIN:
              {
                if (user.account != null) return;
                String username = message.getString();
                String password = message.getString();
                user.server.databaseController.accountExists(username, password).then((bool exists){
                  if (exists) {
                    user.server.databaseController.getAccount(username, password).then((Account account){
                      user.account = account;
                      user.sendBinary(UserMsgCreator.LoggedIn(username, password), true);

                      user.eventAccountLoggedIn();
                    });
                  } else {
                    user.sendBinary(UserMsgCreator.LoginFailed(), true);
                  }
                });
              }
              break;
            case MSG_LOGOUT:
              {
                if (user.account == null) return;
                user.account = null;
                user.sendBinary(UserMsgCreator.LoggedOut(), true);
                user.eventAccountLoggedOut();
              }
              break;
            case MSG_REGISTER:
              {
                if (user.account != null) return;
                String username = message.getString();
                String password = message.getString();
                String email = message.getString();
                String displayName = message.getString();
                String avatarURL = message.getString();
                user.server.databaseController.accountNameExistsOrEmailTaken(username, email).then((bool exists) {
                  if (exists) {
                    user.sendBinary(UserMsgCreator.RegisterFailed());
                  } else {

                    user.server.databaseController.createAccount(displayName, username, password, email, avatarURL).then((int id){
                      user.server.forumController.userRegistered(user);
                      user.server.databaseController.getAccountByID(id).then((Account a){
                        user.account = a;
                        user.sendBinary(UserMsgCreator.LoggedIn(username, password), true);
                        user.eventAccountLoggedIn();
                      });
                    });
                  }
                });
              }
              break;
          }
        }
        break;
    }
  }
}