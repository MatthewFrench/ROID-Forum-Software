library OtherMessageSender;
import 'OtherSectionController.dart';
import '../../User.dart';
import 'ThreadInfo.dart';
import 'CommentInfo.dart';

class MessageSender {
  OtherSectionController controller;
  MessageSender(OtherSectionController c) {
    controller = c;
  }
  void sendAllThreadsToUser(User u) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'All Threads';
    List threadArray = new List();
    for (int i = 0; i < controller.threadController.threads.length; i++) {
      ThreadInfo t = controller.threadController.threads[i];
      Map thread = t.toMap(addAvatars:controller.server.accountController);
      threadArray.add(thread);
    }
    m['Threads'] = threadArray;
    u.sendMap(m);
  }
  void sendAddThreadToAll(ThreadInfo t) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'Thread Add';
    Map thread = t.toMap(addAvatars:controller.server.accountController);
    m['Thread Map'] = thread;
    for (User u in controller.usersViewing) {
      u.sendMap(m);
    }
  }
  void sendRemoveThreadToAll(ThreadInfo t) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'Thread Remove';
    m['ThreadID'] = t.id;
    for (User u in controller.usersViewing) {
      u.sendMap(m);
    }
  }
  void sendUpdateThreadToAll(ThreadInfo t) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'Thread Update';
    m['ThreadID'] = t.id;
    m['Thread Title'] = t.title;
    m['Thread Description'] = t.description;
    for (User u in controller.usersViewing) {
      u.sendMap(m);
    }
  }
  void sendMoveThreadToTopToAll(ThreadInfo t) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'Thread Move To Top';
    m['ThreadID'] = t.id;
    for (User u in controller.usersViewing) {
      u.sendMap(m);
    }
  }
  void sendAddCommentToAll(CommentInfo c) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'Comment Add';
    Map commentMap = c.toMap(addAvatars:controller.server.accountController);
        m['Comment'] = commentMap;
    for (User u in controller.usersViewing) {
      u.sendMap(m);
    }
  }
  void sendDeleteCommentToAll(CommentInfo c) {
    Map m = new Map();
    m['Controller'] = controller.name;
    m['Title'] = 'Comment Delete';
    m['ThreadID'] = c.threadID;
    m['CommentID'] = c.commentID;
    for (User u in controller.usersViewing) {
      u.sendMap(m);
    }
  }
  void sendUpdateCommentToAll(CommentInfo c) {
      Map m = new Map();
      m['Controller'] = controller.name;
      m['Title'] = 'Comment Update';
      m['ThreadID'] = c.threadID;
      m['CommentID'] = c.commentID;
      m['Comment'] = c.comment;
      for (User u in controller.usersViewing) {
        u.sendMap(m);
      }
    }
}