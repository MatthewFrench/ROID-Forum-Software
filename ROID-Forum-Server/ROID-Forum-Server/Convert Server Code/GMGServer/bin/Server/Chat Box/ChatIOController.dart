library ChatIOController;
import 'ChatController.dart';
import 'dart:io';
import 'dart:async';
import 'dart:convert';

class ChatIOController {
  ChatController controller;
  int saveTimer = -1;
  ChatIOController(ChatController c) {
    controller = c;
  }
  void logic() {
    saveTimer += 1;
    if (saveTimer >= 60 * 60 * 2) { //Every 2 minutes
      saveTimer = -1;
      saveAllChats();
    }
  }
  void saveAllChats() {
    try {
      File file = new File('bin/Saved Data/chat.json');
      file.exists().then((bool e) {
        if (e) {
          file.delete().then((var f) {
            var output = file.openWrite();
            output.write(JSON.encode(controller.chats));
            output.close();
          });
        } else {
          var output = file.openWrite();
          output.write(JSON.encode(controller.chats));
          output.close();
        }
      });
    } catch (exception, stackTrace) {
      print(exception);
      print(stackTrace);
    }
  }
  void loadAllChats() {
    try {
      File file = new File('bin/Saved Data/chat.json');
      file.exists().then((bool e) {
        if (e) {
          Future<String> finishedReading = file.readAsString();
          finishedReading.then((text) {
            controller.chats = JSON.decode(text);
            print("Loaded chatbox chats");
          });
        }
      });
    } catch (exception, stackTrace) {
      print(exception);
      print(stackTrace);
    }
  }
}
