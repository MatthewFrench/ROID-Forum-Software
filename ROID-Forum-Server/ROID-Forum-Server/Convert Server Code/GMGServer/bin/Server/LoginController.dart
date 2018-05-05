library LoginController;
import 'Server.dart';
import 'User.dart';
import 'dart:convert';

class LoginController {
  Server server;
  LoginController(Server s) {
    server = s;
  }
  void logic() {
  }
  void onMessage(User u, Map message) {
    if (message['Title'] == 'Set Avatar' && u.account != null) {
      u.account.avatarURL = message['AvatarURL'];
    }
    if (message['Title'] == 'Get Avatar' && u.account != null) {
      Map m = new Map();
              m['Controller'] = 'Login';
              m['Title'] = 'Get Avatar';
              m['AvatarURL'] =  u.account.avatarURL ;
              String s = JSON.encode(m);
              u.sendString(s);
        }
    if (message['Title'] == 'Login' && u.account == null) {
      if (server.accountController.accountExists(message['Name'], message['Password'])) {
        u.account = server.accountController.getAccount(message['Name'], message['Password']);
        //Send login notification
        Map m = new Map();
        m['Controller'] = 'Login';
        m['Title'] = 'Logged In';
        m['Name'] = u.account.name;
        m['Password'] = u.account.password;
        String s = JSON.encode(m);
        u.sendString(s);

        server.accountLoggedIn(u);
      } else {
        //Send login failure
        Map m = new Map();
        m['Controller'] = 'Login';
        m['Title'] = 'Login Failed';
        String s = JSON.encode(m);
        u.sendString(s);
      }
    }
    if (message['Title'] == 'Logout' && u.account != null) {
      //Perhaps an account method can be called for saving or other logic
      u.account = null;
      //Send logout notification
      Map m = new Map();
      m['Controller'] = 'Login';
      m['Title'] = 'Logged Out';
      String s = JSON.encode(m);
      u.sendString(s);
      server.accountLoggedOut(u);
    }
    if (message['Title'] == 'Register' && u.account == null) {
      if (server.accountController.accountNameExists(message['Name'])) {
        //Send register failure
        Map m = new Map();
        m['Controller'] = 'Login';
        m['Title'] = 'Register Failed';
        String s = JSON.encode(m);
        u.sendString(s);
      } else {
        u.account = server.accountController.createAccount(message['Name'], message['Password'], message['Email']);
        //Send login notification
        Map m = new Map();
        m['Controller'] = 'Login';
        m['Title'] = 'Logged In';
        m['Name'] = u.account.name;
        m['Password'] = u.account.password;
        String s = JSON.encode(m);
        u.sendString(s);
        server.accountLoggedIn(u);
      }
    }
  }
}
