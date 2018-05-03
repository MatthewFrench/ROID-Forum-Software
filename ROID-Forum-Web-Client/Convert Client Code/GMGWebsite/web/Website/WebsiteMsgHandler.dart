library WebsiteMsgHandler;
import 'Website.dart';
import 'Networking/MessageReader.dart';

const int User_Controller_Binary = 0;
const int Forum_Controller_Binary = 1;

//Messages from server
const int MSG_PING = 0;
const int MSG_AVATAR = 1;
const int MSG_LOGGED_IN = 2;
const int MSG_LOGIN_FAILED = 3;
const int MSG_LOGGED_OUT = 4;
const int MSG_REGISTER_FAILED = 5;

class WebsiteMsgHandler {
  Website website;

  WebsiteMsgHandler(Website w) {
    website = w;
  }
  void onBinaryMessage(MessageReader message) {
    int controller = message.getUint8();
    switch (controller) {
      case User_Controller_Binary:
        {
        
          int binaryMsg = message.getUint8();
          switch (binaryMsg) {
            case MSG_PING:
              {
                website.networkingController.pingTracker.gotPing();
              }
              break;
            case MSG_AVATAR:
              {
                website.controlPanel.controlPanelGui.preferencesAvatarInput.value = message.getString();
              }
              break;
            case MSG_LOGGED_IN:
              {
                String name = message.getString();
                String password = message.getString();
                Map m = new Map();
                m['Username'] = name;
                m['Password'] = password;
                website.database.processEvent('Logged In', m);
                website.controlPanel.processEvent('Logged In');
                //if (website.displayPage != null) website.displayPage.ProcessEvent('Logged In');
                website.controlPanel.resetLoginRegisterWindow();
                website.forumController.loggedIn();
              }
              break;
            case MSG_LOGIN_FAILED:
              {
                website.controlPanel.processEvent('Login Failed');
              }
              break;
            case MSG_LOGGED_OUT:
              {
                website.database.processEvent('Logged Out');
                website.controlPanel.processEvent('Logged Out');
                website.forumController.loggedOut();
                //if (website.displayPage != null) website.displayPage.ProcessEvent('Logged Out');
              }
              break;
            case MSG_REGISTER_FAILED:
              {
                website.controlPanel.processEvent('Register Failed');
              }
              break;
          }
        }
        break;
      case Forum_Controller_Binary: {
        website.forumController.handleMessage(message);
      } break;
      default:
        { //Just push it to the active page
          //if (website.displayPage == null) return;
          //website.displayPage.onBinaryMessage(message, controller);
        }
    }
  }
}