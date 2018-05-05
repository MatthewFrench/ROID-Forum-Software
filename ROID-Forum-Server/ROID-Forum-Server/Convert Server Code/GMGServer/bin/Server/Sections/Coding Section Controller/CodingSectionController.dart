library CodingSectionController;
import '../../Server.dart';
import '../../User.dart';
import 'ThreadController.dart';
import 'IOController.dart';
import 'MessageSender.dart';

class CodingSectionController {
  Server server;
  MessageSender messageSender;
  ThreadController threadController;
  List<User> usersViewing = new List();
  String name = "Coding Section";
  IOController ioController;
  int saveTimer = -1;

  CodingSectionController(Server s) {
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
      case 'New Post':
        {
          String postTitle = message['Post Title'];
          String postDescription = message['Post Description'];
          threadController.addThread(p, postTitle, postDescription);
        }
        break;
      case 'Edit Post':
        {
          int threadID = message["Thread ID"];
          String title = message['Edit Title'];
          String description = message['Text'];
          threadController.editThread(p, threadID, title, description);
        }
        break;
      case 'Delete Post':
        {
          int threadID = message["Thread ID"];
          threadController.deleteThread(p, threadID);
        }
        break;
      case 'Add Comment':
        {
          int id = message["ID"];
          String text = message['Text'];
          threadController.addComment(p, id, text);
        }
        break;
      case 'Edit Comment':
        {
          int threadID = message["Thread ID"];
          int commentID = message["Comment ID"];
          String description = message['Text'];
          threadController.editComment(p, threadID, commentID, description);
        }
        break;
      case 'Delete Comment':
        {
          int threadID = message["Thread ID"];
          int commentID = message["Comment ID"];
          threadController.deleteComment(p, threadID, commentID);
        }
        break;
    }
  }
}
