library AllSection;
import 'dart:html';
import '../GenericSection.dart';
import '../../Website.dart';
import 'ThreadController.dart';

class AllSection extends GenericSection {
  Website website;
  DivElement content;
  String name = 'All Section';
  String displayName = 'All';
  ThreadController threadController;
  AllSection(Website w) {
    website = w;
    makeMainContentComponent();
    threadController = new ThreadController(this);
    content.append(threadController.mainView);
  }
  void show() {
    Map m = new Map();
    m['Controller'] = "Server";
    m['Title'] = 'Viewing';
    m['Section'] = name;
    website.networkingController.Send(m);
  }
  void hide() {
    //Destroy all posts
    threadController.clearAllThreads();
  }
  void processEvent(String event) {
    switch (event) {
      case "Logged In":
        {
        }
        break;
      case "Logged Out":
        {
        }
        break;
    }
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
        }
        break;
      case "Thread Add":
        {
          threadController.addThread(message["Thread Map"]);
        }
        break;
      case "Thread Remove":
        {
          threadController.removeThread(message["ThreadID"], message["Section"]);
        }
        break;
      case "Thread Update":
        {
          threadController.updateThread(message["ThreadID"], message["Section"], message["Thread Title"]);
        }
        break;
      case "Thread Move To Top":
        {
          threadController.moveThreadToTop(message["ThreadID"], message["Section"]);
        }
        break;
      case "Comment Count Update":
        {
          threadController.updateCommentCount(message["ThreadID"], message["Section"], message["Comment Count"]);
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
