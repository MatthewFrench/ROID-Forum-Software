library AllThreadController.dart;
import 'ThreadInfo.dart';
import 'AllSection.dart';
import 'dart:html';
import 'package:animation/animation.dart';

class ThreadController {
  List<ThreadInfo> threads;
  AllSection sectionController;
  DivElement mainView;
  DivElement headerView;
  int viewingPosition = 0;
  ThreadController(AllSection c) {
    sectionController = c;
    threads = new List();
    mainView = new DivElement();
    headerView = new DivElement();
    mainView.append(headerView);
    SpanElement s = new SpanElement();
        s.text = "The GMG Front Page!";
        s.style
        ..position = "absolute"
        ..width = "100%"
        ..textAlign = "center"
        ..top = "14px"
        ..fontSize = "25px";
        headerView.append(s);
  }
  void threadClicked(ThreadInfo thread) {
    sectionController.website.goTo(thread.getSection(), thread.getID());
  }
  void updateThreadPositions() {
    for (int i = 0; i < threads.length; i++) {
      ThreadInfo t = threads[i];
      //t.headerView.getDiv().style.top = "${i*85+60}px";
      animate(t.headerView.getDiv(), duration: 250, properties: {'top': "${i*85+60}px"});
    }
  }
  ThreadInfo getThread(int threadID, String section) {
    for (ThreadInfo t in threads) {
      if (t.getID() == threadID && t.getSection() == section) return t;
    }
    return null;
  }
  //Actions
  void clearAllThreads() {
    for (int i = 0; i < threads.length; i++) {
      ThreadInfo t = threads[i];
      t.headerView.getDiv().remove();
    }
    threads.clear();
  }
  void addThread(Map threadMap) {
    int id = threadMap["ThreadID"];
    String section = threadMap["Section"];
    if (getThread(id, section) == null) {
    ThreadInfo thread = new ThreadInfo(this);
    thread.setID(id);
    threads.add(thread);

    thread.setCommentCount(threadMap["Comment Count"]);
    thread.setSection(threadMap["Section"]);
    thread.setOwner(threadMap["Owner"]);
    thread.setTitle(threadMap["Title"]);
    if (threadMap["AvatarURL"] != null) {
    thread.setAvatarURL(threadMap["AvatarURL"]);
    }

    thread.headerView.getDiv().style.opacity = "0.0";
    thread.headerView.getDiv().style.top = "${(threads.length-1)*85+60}px";
    headerView.append(thread.headerView.getDiv());
    animate(thread.headerView.getDiv(), duration: 250, properties: {'opacity': 1.0});
    
    updateThreadPositions();
    }
  }
  void removeThread(int threadID, String section) {
    ThreadInfo thread = getThread(threadID, section);
    if (thread != null) {
      threads.remove(thread);
      thread.headerView.getDiv().style.opacity = "1.0";
      animate(thread.headerView.getDiv(), duration: 250, properties: {'opacity': 0.0}).onComplete.listen((_){
        thread.headerView.getDiv().remove();
      });
      updateThreadPositions();
    }
  }
  void updateThread(int threadID, String section, String title) {
    ThreadInfo thread = getThread(threadID, section);
    if (thread != null) {
      thread.setTitle(title);
    }
  }
  void moveThreadToTop(int threadID, String section) {
    ThreadInfo thread = getThread(threadID, section);
    if (thread != null) {
      threads.remove(thread);
      threads.insert(0, thread);
      updateThreadPositions();
    }
  }
  void updateCommentCount(int threadID, String section, int commentCount) {
    ThreadInfo thread = getThread(threadID, section);
    if (thread != null) {
      thread.setCommentCount(commentCount);
    }
  }
}
