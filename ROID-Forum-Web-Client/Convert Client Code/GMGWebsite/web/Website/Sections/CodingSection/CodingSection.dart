library CodingSection;
import 'dart:html';
import '../GenericSection.dart';
import '../../Website.dart';
import 'NewPostWindow.dart';
import 'ThreadController.dart';
import 'FancyBackground.dart';

class CodingSection extends GenericSection {
  Website website;
  DivElement content;
  String name = 'Coding Section';
  String displayName = 'Coding';
  NewPostWindow newPostWindow = null;
  ThreadController threadController;
  int showThreadWhenLoaded = -1;
  FancyBackground background;
  CodingSection(Website w) {
    website = w;
    makeMainContentComponent();
    newPostWindow = new NewPostWindow(this);
    threadController = new ThreadController(this);
    content.append(threadController.mainView);
    background = new FancyBackground(this);
  }
  void show() {
    Map m = new Map();
    m['Controller'] = "Server";
    m['Title'] = 'Viewing';
    m['Section'] = name;
    website.networkingController.Send(m);

    threadController.restoreToDefaultState();

    background.show();

    website.chatbox.darkTheme();
    website.mainSection.darkTheme();
    website.controlPanel.darkTheme();
  }
  void hide() {
    //Destroy all posts
    threadController.clearAllThreads();

    background.hide();

    website.chatbox.lightTheme();
    website.mainSection.lightTheme();
    website.controlPanel.lightTheme();
  }
  void logic() {
    background.logic();
  }
  void showThread(int threadID) {
    showThreadWhenLoaded = threadID;
  }
  void processEvent(String event) {
    switch (event) {
      case "Logged In":
        {
          threadController.loggedInEvent();
        }
        break;
      case "Logged Out":
        {
          threadController.loggedOutEvent();
        }
        break;
    }
  }
  void newPostButtonClicked() {
    newPostWindow.show();
  }
  void onMessage(Map message) {
    switch (message['Title']) {
      case "All Threads":
        {
          threadController.clearAllThreads();
          List threads = message['Threads'];
          for (int i = 0; i < threads.length; i++) {
            threadController.addThread(threads[i]);
          }
          if (showThreadWhenLoaded != -1) {
            threadController.showThread(showThreadWhenLoaded);
            //print("Told to show thread ${showThreadWhenLoaded}");
            showThreadWhenLoaded = -1;
          }
        }
        break;
      case "Thread Add":
        {
          threadController.addThread(message["Thread Map"]);
        }
        break;
      case "Thread Remove":
        {
          threadController.removeThread(message["ThreadID"]);
        }
        break;
      case "Thread Update":
        {
          threadController.updateThread(message["ThreadID"], message["Thread Title"], message["Thread Description"]);
        }
        break;
      case "Thread Move To Top":
        {
          threadController.moveThreadToTop(message["ThreadID"]);
        }
        break;
      case "Comment Add":
        {
          threadController.addComment(message["Comment"]);
        }
        break;
      case "Comment Delete":
        {
          threadController.deleteComment(message["ThreadID"], message["CommentID"]);
        }
        break;
      case "Comment Update":
        {
          threadController.updateComment(message["ThreadID"], message["CommentID"], message["Comment"]);
        }
        break;
    }
  }
  DivElement getDiv() {
    return content;
  }
  String getName() {
    return name;
  }
  String getDisplayName() {
      return displayName;
    }
  /************* Create the GUI Components ***********/
  void makeMainContentComponent() {
    content = new DivElement();
    content.style
        ..position = "absolute"
        ..left = "0px"
        ..top = "90px"
        ..width = "calc(100% - 300px)"
        ..paddingTop = "10px";
  }
}
