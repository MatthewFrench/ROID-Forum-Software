library AllThreadController;
import 'ThreadInfo.dart';
import 'AllSectionController.dart';

class ThreadController {
  List<ThreadInfo> threads;
  AllSectionController controller;
  int threadIDs = 0;
  ThreadController(AllSectionController c) {
    controller = c;
    threads = new List();
  }
  ThreadInfo getThread(int threadID, String section) {
    for (int i = 0; i < threads.length; i++) {
      if (threads[i].threadID == threadID && threads[i].section == section) {
        return threads[i];
      }
    }
    return null;
  }
  //Thread actions
  void addThread(String section, int threadID, String title, int commentCount, String owner) {
    ThreadInfo t = new ThreadInfo.fromData(threadID, section, title, commentCount, owner);
    threads.add(t);
    controller.messageSender.sendAddThreadToAll(t);
  }
  void deleteThread(String section, int threadID) {
    ThreadInfo t = getThread(threadID, section);
    if (t != null) {
      threads.remove(t);
      controller.messageSender.sendRemoveThreadToAll(t);
    }
  }
  void editThread(String section, int threadID, String title) {
    ThreadInfo t = getThread(threadID, section);
    t.title = title;
    controller.messageSender.sendUpdateThreadToAll(t);
  }
  void moveThreadToTop(String section, int threadID) {
    ThreadInfo t = getThread(threadID, section);
    if (t != null) {
      threads.remove(t);
      threads.insert(0, t);
      controller.messageSender.sendMoveThreadToTopToAll(t);
    }
  }
  void updateCommentCount(String section, int threadID, int commentCount) {
      ThreadInfo t = getThread(threadID, section);
      if (t != null) {
        t.commentCount = commentCount;
        controller.messageSender.sendUpdateCommentCountToAll(t);
      }
    }
}
