library AccountController;
import '../Server.dart';
import 'Account.dart';
import 'AccountIOController.dart';

class AccountController {
  List<Account> accounts = new List();
  Server server;
  AccountIOController accountIOController;
  AccountController(Server s) {
    server = s;
    accountIOController = new AccountIOController(this);
    accountIOController.loadAllAccounts();
  }
  void logic() {
      accountIOController.logic();
  }
  String getAvatarForAccount(String name) {
      for (int i = 0; i < accounts.length; i++) {
        Account a = accounts[i];
        if (a.name == name) {
          return a.avatarURL;
        }
      }
      return "";
    }
  bool accountExists(String name, String password) {
    for (int i = 0; i < accounts.length; i++) {
      Account a = accounts[i];
      if (a.name == name && a.password == password) {
        return true;
      }
    }
    return false;
  }
  bool accountNameExists(String name) {
      for (int i = 0; i < accounts.length; i++) {
        Account a = accounts[i];
        if (a.name == name) {
          return true;
        }
      }
      return false;
    }
  Account getAccount(String name, String password) {
    for (int i = 0; i < accounts.length; i++) {
      Account a = accounts[i];
      if (a.name == name && password == password) {
        return a;
      }
    }
    return null;
  }
  Account createAccount(String name, String password, String email) {
    Map m = new Map();
    m['Name'] = name;
    m['Password'] = password;
    m['Email'] = email;
    print("Created account: ${name}, ${password}, ${email}");
    Account a = new Account(m);
    accounts.add(a);
    return a;
  }
}