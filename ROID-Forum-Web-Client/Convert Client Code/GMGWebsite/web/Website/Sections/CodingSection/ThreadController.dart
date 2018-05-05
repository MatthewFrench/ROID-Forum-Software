library CodingThreadController.dart;
import 'ThreadInfo.dart';
import 'CodingSection.dart';
import 'dart:html';
import 'CommentInfo.dart';
import 'package:animation/animation.dart';

class ThreadController {
  List<ThreadInfo> threads;
  CodingSection sectionController;
  DivElement mainView;
  DivElement headerView;
  DivElement fullView;
  ButtonElement newPostButton, backButton;
  int viewingPosition = 0;
  ThreadInfo viewingThread = null;
  ThreadController(CodingSection c) {
    sectionController = c;
    threads = new List();
    mainView = new DivElement();
    headerView = new DivElement();
    SpanElement s = new SpanElement();
    s.text = "The Programmer's Corner";
    s.style
    ..position = "absolute"
    ..width = "100%"
    ..textAlign = "center"
    ..top = "14px"
    ..fontSize = "25px"
    ..color = "white";
    headerView.append(s);
    fullView = new DivElement();
    makeNewPostButtonComponent();
    makeBackButtonComponent();
    mainView.append(headerView);
  }
  void loggedInEvent() {
    headerView.append(newPostButton);
    for (int i = 0; i < threads.length; i++) {
      ThreadInfo t = threads[i];
      t.fullView.main.append(t.fullView.commentButtonAndBox);
    }
  }
  void loggedOutEvent() {
    newPostButton.remove();
    for (int i = 0; i < threads.length; i++) {
      ThreadInfo t = threads[i];
      t.fullView.commentButtonAndBox.remove();
    }
  }
  void restoreToDefaultState() {
    fullView.remove();
    fullView.children.clear();
    mainView.append(headerView);
    backButton.remove();
    viewingThread = null;
  }
  void threadClicked(ThreadInfo thread) {
    headerView.remove();
    mainView.append(fullView);
    fullView.append(thread.fullView.getDiv());
    mainView.append(backButton);
    viewingThread = thread;
  }
  void backButtonClicked() {
    restoreToDefaultState();
  }
  void updateThreadPositions() {
    for (int i = 0; i < threads.length; i++) {
      ThreadInfo t = threads[i];
      //t.headerView.getDiv().style.top = "${i*85+60}px";
      animate(t.headerView.getDiv(), duration: 250, properties: {
        'top': "${i*85+60}px"
      });
    }
  }
  ThreadInfo getThread(int id) {
    for (ThreadInfo t in threads) {
      if (t.getID() == id) return t;
    }
    return null;
  }
  void showThread(int threadID) {
    ThreadInfo thread = getThread(threadID);
    if (thread != null) {
      threadClicked(thread);
    }
  }
  //Actions
  void clearAllThreads() {
    for (int i = 0; i < threads.length; i++) {
      ThreadInfo t = threads[i];
      t.headerView.getDiv().remove();
      t.fullView.getDiv().remove();
    }
    threads.clear();
  }
  void addThread(Map threadMap) {
    int id = threadMap["ID"];
    if (getThread(id) == null) {
      ThreadInfo thread = new ThreadInfo(this);
      thread.setID(id);
      threads.add(thread);

      thread.setOwner(threadMap["Owner"]);
      if (threadMap["AvatarURL"] != null) {
        thread.setAvatarURL(threadMap["AvatarURL"]);
      }
      thread.setTitle(threadMap["Title"]);
      thread.setDescription(threadMap["Description"]);

      List<Map> commentArray = threadMap["Comments"];
      for (int i = 0; i < commentArray.length; i++) {
        Map c = commentArray[i];
        CommentInfo cf = new CommentInfo(thread, this);
        thread.addComment(cf);
        print("Loaded comment with threadID: ${c['ThreadID']} and commentID: ${c['CommentID']}");
        cf.setThreadID(c['ThreadID']);
        cf.setCommentID(c['CommentID']);
        cf.setComment(c['Comment']);
        cf.setOwner(c['Owner']);
        if (c["AvatarURL"] != null) {
        cf.setAvatarURL(c["AvatarURL"]);
        }
      }
      thread.headerView.getDiv().style.opacity = "0.0";
      thread.headerView.getDiv().style.top = "${(threads.length-1)*85+60}px";
      headerView.append(thread.headerView.getDiv());
      animate(thread.headerView.getDiv(), duration: 250, properties: {
        'opacity': 1.0
      });

      updateThreadPositions();
    }
  }
  void removeThread(int threadID) {
    ThreadInfo thread = getThread(threadID);
    if (thread != null) {
      threads.remove(thread);
      thread.headerView.getDiv().style.opacity = "1.0";
      animate(thread.headerView.getDiv(), duration: 250, properties: {
        'opacity': 0.0
      }).onComplete.listen((_) {
        thread.headerView.getDiv().remove();
      });
      thread.fullView.getDiv().style.opacity = "1.0";
      animate(thread.fullView.getDiv(), duration: 250, properties: {
        'opacity': 0.0
      }).onComplete.listen((_) {
        thread.fullView.getDiv().remove();
      });
      updateThreadPositions();
      if (thread == viewingThread) {
        restoreToDefaultState();
      }
    }
  }
  void updateThread(int threadID, String title, String description) {
    ThreadInfo thread = getThread(threadID);
    if (thread != null) {
      thread.setTitle(title);
      thread.setDescription(description);
    }
  }
  void moveThreadToTop(int threadID) {
    ThreadInfo thread = getThread(threadID);
    if (thread != null) {
      threads.remove(thread);
      threads.insert(0, thread);
      updateThreadPositions();
    }
  }
  void addComment(Map commentMap) {
    int threadID = commentMap["ThreadID"];
    ThreadInfo thread = getThread(threadID);
    if (thread != null) {
      CommentInfo cf = new CommentInfo(thread, this);
      thread.addComment(cf);
      cf.setThreadID(commentMap['ThreadID']);
      cf.setCommentID(commentMap['CommentID']);
      cf.setComment(commentMap['Comment']);
      cf.setOwner(commentMap['Owner']);
      cf.setAvatarURL(commentMap["AvatarURL"]);
    }
  }
  void deleteComment(int threadID, int commentID) {
    ThreadInfo thread = getThread(threadID);
    if (thread != null) {
      thread.removeComment(commentID);
    }
  }
  void updateComment(int threadID, int commentID, String comment) {
    ThreadInfo thread = getThread(threadID);
    if (thread != null) {
      CommentInfo ci = thread.getComment(commentID);
      ci.setComment(comment);
    }
  }
  //GUI
  void makeBackButtonComponent() {
    //Add the new post button to it
    backButton = new ButtonElement();
    backButton.style
        ..width = "60px"
        ..height = "30px"
        ..backgroundColor = "white"
        ..borderRadius = "4px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..fontSize = "15px"
        ..top = "5px"
        ..position = "absolute"
        ..outline = "none"
        ..left = "5px";
    backButton.dataset['active'] = "background: #BBBBBB;";
    backButton.text = 'Back';
    backButton.onClick.listen((var d) {
      backButtonClicked();
    });
  }
  void makeNewPostButtonComponent() {
    //Add the new post button to it
    newPostButton = new ButtonElement();
    newPostButton.style
        ..width = "140px"
        ..height = "40px"
        ..backgroundColor = "white"
        ..borderRadius = "4px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..fontSize = "15px"
        ..top = "5px"
        ..position = "absolute"
        ..outline = "none"
        ..right = "5px";
    newPostButton.dataset['active'] = "background: #BBBBBB;";
    newPostButton.text = 'Create New Post';
    newPostButton.onClick.listen((var d) {
      sectionController.newPostButtonClicked();
    });
  }
}
