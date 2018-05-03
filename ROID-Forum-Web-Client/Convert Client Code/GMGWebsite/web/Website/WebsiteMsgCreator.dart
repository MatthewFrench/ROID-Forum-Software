library WebsiteMsgCreator;
import 'Networking/MessageWriter.dart';

const int User_Controller_Binary = 0,
    //Chat_Controller_Binary = 1, //I'll make the chat more of a global thing so it can be used in any page
    //Block_Game_Controller_Binary = 2,
    Forum_Controller_Binary = 3;

//Messages from the client
const int MSG_PING = 0;
const int MSG_SET_AVATAR = 1;
const int MSG_GET_AVATAR = 2;
const int MSG_LOGIN = 3;
const int MSG_LOGOUT = 4;
const int MSG_REGISTER = 5;
const int MSG_PING_FINAL = 6;

class WebsiteMsgCreator {
  static MessageWriter Ping() {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_PING);
    return m;
  }
  static MessageWriter PingFinal() {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_PING_FINAL);
    return m;
  }
  static MessageWriter SetAvatar(String avatarURL) {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_SET_AVATAR);
    m.addString(avatarURL);
    return m;
  }
  static MessageWriter GetAvatar() {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_GET_AVATAR);
    return m;
  }
  static MessageWriter Register(String name, String password, String email, String displayName, String avatarURL) {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_REGISTER);
    m.addString(name);
    m.addString(password);
    m.addString(email);
    m.addString(displayName);
    m.addString(avatarURL);
    return m;
  }
  static MessageWriter LogIn(String name, String password) {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_LOGIN);
    m.addString(name);
    m.addString(password);
    return m;
  }
  static MessageWriter LogOut() {
    MessageWriter m = new MessageWriter();
    m.addUint8(User_Controller_Binary);
    m.addUint8(MSG_LOGOUT);
    return m;
  }
}