library AllMessageSender;
import 'AllSectionController.dart';
import '../../User.dart';
import 'ThreadInfo.dart';

class MessageSender {
  AllSectionController controller;
  MessageSender(AllSectionController c) {
    controller = c;
  }
  void sendAllThreadsToUser(User u) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'All Threads';
    List threadArray = new List();
    for (int i = 0; i < controller.threadController.threads.length; i++) {
      ThreadInfo t = controller.threadController.threads[i];
      Map thread = t.toMap();
      thread['AvatarURL'] = controller.server.accountController.getAvatarForAccount(t.owner); 
      threadArray.add(thread);
    }
    m['Threads'] = threadArray;
    u.sendMap(m);
  }
  void sendAddThreadToAll(ThreadInfo t) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'Thread Add';
    Map thread = t.toMap();
    thread['AvatarURL'] = controller.server.accountController.getAvatarForAccount(t.owner); 
    
    m['Thread Map'] = thread;
    for (User u in controller.usersViewing) {
      u.sendMap(m);
    }
  }
  void sendRemoveThreadToAll(ThreadInfo t) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'Thread Remove';
    m['ThreadID'] = t.threadID;
    m['Section'] = t.section;
    for (User u in controller.usersViewing) {
      u.sendMap(m);
    }
  }
  void sendUpdateThreadToAll(ThreadInfo t) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'Thread Update';
    m['ThreadID'] = t.threadID;
    m['Section'] = t.section;
    m['Thread Title'] = t.title;
    for (User u in controller.usersViewing) {
      u.sendMap(m);
    }
  }
  void sendMoveThreadToTopToAll(ThreadInfo t) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'Thread Move To Top';
    m['ThreadID'] = t.threadID;
    m['Section'] = t.section;
    for (User u in controller.usersViewing) {
      u.sendMap(m);
    }
  }
  void sendUpdateCommentCountToAll(ThreadInfo t) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'Comment Count Update';
    m['ThreadID'] = t.threadID;
    m['Section'] = t.section;
    m['Comment Count'] = t.commentCount;
    for (User u in controller.usersViewing) {
      u.sendMap(m);
    }
  }
}
