library GameThreadController;
import 'ThreadInfo.dart';
import 'GameSectionController.dart';
import '../../User.dart';
import 'CommentInfo.dart';

class ThreadController {
  List<ThreadInfo> threads;
  GameSectionController controller;
  int threadIDs = 0;
    ThreadController(GameSectionController c) {
      controller = c;
      threads = new List();
    }
    ThreadInfo getThreadForID(int threadID) {
      for (int i = 0; i < threads.length; i++) {
        if (threads[i].id == threadID) {
          return threads[i];
        }
      }
      return null;
    }
    //Thread actions
    void addThread(User p, String title, String description) {
      ThreadInfo t = new ThreadInfo.fromData(p.account.name, threadIDs++, title, description);
      threads.add(t);
      controller.messageSender.sendAddThreadToAll(t);

      //Send a message to the All Controller
      controller.server.allSection.threadController.addThread(controller.name, t.id, t.title, t.comments.length, t.owner);

      moveThreadToTop(t.id);
    }
    void deleteThread(User p,int threadID) {
      ThreadInfo t = getThreadForID(threadID);
      if (t != null) {
        if (t.owner == p.account.name) {
          threads.remove(t);
          controller.messageSender.sendRemoveThreadToAll(t);
          //Send a message to the All Controller
          controller.server.allSection.threadController.deleteThread(controller.name, t.id);
        }
      }
    }
    void editThread(User p,int id, String title, String description) {
      ThreadInfo t = getThreadForID(id);
      if (t != null) {
            if (t.owner == p.account.name) {
      t.title = title;
      t.description = description;
      controller.messageSender.sendUpdateThreadToAll(t);
      //Send a message to the All Controller
      controller.server.allSection.threadController.editThread(controller.name, t.id, t.title);
            }
      }
    }
    void moveThreadToTop(int id) {
      ThreadInfo t = getThreadForID(id);
      if (t != null) {
        threads.remove(t);
        threads.insert(0, t);
        controller.messageSender.sendMoveThreadToTopToAll(t);
        //Send a message to the All Controller
        controller.server.allSection.threadController.moveThreadToTop(controller.name, t.id);
      }
    }
    void addComment(User u, int threadID, String text) {
      ThreadInfo t = getThreadForID(threadID);
      if (t != null) {
        CommentInfo c = new CommentInfo.fromData(t.id, t.commentIDs++, text, u.account.name);
        t.comments.add(c);
        controller.messageSender.sendAddCommentToAll(c);

        //Send a message to the All Controller
        controller.server.allSection.threadController.updateCommentCount(controller.name, t.id, t.comments.length);

        moveThreadToTop(threadID);
      }
    }
    void deleteComment(User p,int threadID, int commentID) {
      ThreadInfo t = getThreadForID(threadID);
      if (t != null) {
        CommentInfo c = t.getCommentForID(commentID);
        if (c != null) {
          if (p.account.name == c.owner) {
          t.comments.remove(c);
          controller.messageSender.sendDeleteCommentToAll(c);

          //Send a message to the All Controller
          controller.server.allSection.threadController.updateCommentCount(controller.name, t.id, t.comments.length);
          }
        }
      }
    }
    void editComment(User p,int threadID, int commentID, String description) {
      ThreadInfo t = getThreadForID(threadID);
      if (t != null) {
        CommentInfo c = t.getCommentForID(commentID);
        if (c != null) {
          if (p.account.name == c.owner) {
            c.comment = description;
            controller.messageSender.sendUpdateCommentToAll(c);
          }
        }
      }
    }
  }