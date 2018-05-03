library Database;
import 'dart:html';
import 'Website.dart';

class Database {
  Website website;
  bool loggedIn = false;
  String name = "";
  String password = "";
  Database(Website w) {
    website = w;
  }
  void processEvent(String event, [Map message]) {
    switch (event) {
      case "Logged In":
        {
          String n = message['Username'];
          String p = message['Password'];

          //Save name and password in local storage
          Storage localStorage = window.localStorage;
          localStorage['Username'] = n;
          localStorage['Password'] = p;

          loggedIn = true;
          name = n;
          password = p;
        }
        break;
      case "Logged Out":
        {
          //Clear local storage
          Storage localStorage = window.localStorage;
          localStorage.remove('Username');
          localStorage.remove('Password');
          name = "";
          password = "";
          loggedIn = false;
        }
        break;
    }
  }
}
