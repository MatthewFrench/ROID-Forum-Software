library AccountIOController;
import 'AccountController.dart';
import 'Account.dart';
import 'dart:convert';
import 'dart:io';
import 'dart:async';

class AccountIOController {
  int saveTimer = -1;
  AccountController controller;
  AccountIOController(AccountController c) {
    controller = c;
  }
  void logic() {
    saveTimer += 1;
    if (saveTimer >= 60 * 60 * 2) { //Every 2 minutes
      saveTimer = -1;
      saveAllAccounts();
    }
  }
  void saveAllAccounts() {
    try {
      File file = new File('bin/Saved Data/accounts.json');
      file.exists().then((bool e) {
        if (e) {
          file.delete().then((var f) {
            var output = file.openWrite();
            List<Map> accountMaps = new List();
            for (int i = 0; i < controller.accounts.length; i++) {
              Account a = controller.accounts[i];
              accountMaps.add(a.toJSON());
            }
            output.write(JSON.encode(accountMaps));
            output.close();
          });
        } else {
          var output = file.openWrite();
          List<Map> accountMaps = new List();
          for (int i = 0; i < controller.accounts.length; i++) {
            Account a = controller.accounts[i];
            accountMaps.add(a.toJSON());
          }
          output.write(JSON.encode(accountMaps));
          output.close();
        }
      });
    } catch (exception, stackTrace) {
      print(exception);
      print(stackTrace);
    }
  }
  void loadAllAccounts() {
    try {
      File file = new File('bin/Saved Data/accounts.json');
      file.exists().then((bool e) {
        if (e) {
          Future<String> finishedReading = file.readAsString();
          finishedReading.then((text) {
            List<Map> accountMaps = JSON.decode(text);
            for (int i = 0; i < accountMaps.length; i++) {
              Map m = accountMaps[i];
              controller.accounts.add(new Account(m));
            }
            print("Loaded accounts");
          });
        }
      });
    } catch (exception, stackTrace) {
      print(exception);
      print(stackTrace);
    }
  }
}