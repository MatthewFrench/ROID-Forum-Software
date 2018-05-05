library AllSectionController;
import '../../Server.dart';
import '../../User.dart';
import 'MessageSender.dart';
import 'ThreadController.dart';
import 'IOController.dart';

class AllSectionController {
  Server server;
  List<User> usersViewing = new List();
  String name = "All Section";
  MessageSender messageSender;
  ThreadController threadController;
  IOController ioController;
  int saveTimer = -1;

  AllSectionController(Server s) {
    server = s;
    messageSender = new MessageSender(this);
    threadController = new ThreadController(this);
    ioController = new IOController(this);
    ioController.loadAllData();
  }
  void addUser(User u) {
    usersViewing.add(u);
    messageSender.sendAllThreadsToUser(u);
  }
  void removeUser(User u) {
    usersViewing.remove(u);
  }
  void logic() {
    saveTimer += 1;
    if (saveTimer >= 60 * 60 * 2) { //Every 2 minutes
      saveTimer = -1;
      ioController.saveAllData();
    }
  }
  void onMessage(User p, Map message) {
    switch (message['Title']) {
    }
  }
}
