library UserMsgCreator;
import '../Networking/MessageWriter.dart';

const int User_Controller_Binary = 0;

//Messages to send to the client
const int MSG_PING = 0;
const int MSG_AVATAR = 1;
const int MSG_LOGGED_IN = 2;
const int MSG_LOGIN_FAILED = 3;
const int MSG_LOGGED_OUT = 4;
const int MSG_REGISTER_FAILED = 5;
const int MSG_PEOPLE_ONLINE = 6;
const int MSG_PEOPLE_REGISTERED = 7;

class UserMsgCreator {
  static MessageWriter PeopleRegistered(int num) {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_PEOPLE_REGISTERED);
    m.addInt32(num);
    return m;
  }
  static MessageWriter PeopleOnline(int num) {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_PEOPLE_ONLINE);
    m.addInt32(num);
    return m;
  }
  static MessageWriter Ping() {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_PING);
    return m;
  }
  static MessageWriter Avatar(String avatarURL) {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_AVATAR);
    m.addString(avatarURL);
    return m;
  }
  static MessageWriter RegisterFailed() {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_REGISTER_FAILED);
    return m;
  }
  static MessageWriter LoggedIn(String name, String password) {
    //Send both for the client to save for auto-login next session
    //Password is encrypted
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_LOGGED_IN);
    m.addString(name);
    m.addString(password);
    return m;
  }
  static MessageWriter LoggedOut() {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_LOGGED_OUT);
    return m;
  }
  static MessageWriter LoginFailed() {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_LOGIN_FAILED);
    return m;
  }
}