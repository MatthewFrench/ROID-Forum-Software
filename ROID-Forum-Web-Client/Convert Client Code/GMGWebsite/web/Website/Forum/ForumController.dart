library ForumController;

import '../Website.dart';
import 'ForumMsgHandler.dart';
import '../Networking/MessageReader.dart';
import '../Gui/ContentView.dart';
import 'UI/Main Page/ForumView.dart';
import 'UI/Main Page/NewPostView.dart';
import 'UI/Threads Page/ThreadView.dart';
import 'UI/Sections Page/SectionView.dart';
import 'UI/Posts Page/PostView.dart';

class ForumController {
  Website website;
  ForumMsgHandler forumMsgHandler;
  ContentView view;
  ForumView forumView;
  SectionView sectionView;
  NewPostView newPostView;
  ThreadView threadView;
  PostView postView;
  ForumController(Website w) {
    website = w;
    forumMsgHandler = new ForumMsgHandler(this);
    forumView = new ForumView(this);
    sectionView = new SectionView(this);
    threadView = new ThreadView(this);
    postView = new PostView(this);
    newPostView = new NewPostView(this);
    view = website.contentView;
  }
  void connected() {
    //Show forum view cause just connected
    showForums();
  }
  void showForums() {
    view.clearView();
    forumView.requestForumList();
    view.appendView(forumView.getView());
    view.appendView(newPostView.getView());
  }
  void showSections(int forumID) {
    view.clearView();
    sectionView.clearSectionList();
    sectionView.requestSectionList(forumID);
    view.appendView(sectionView.getView());
  }
  void showThreadView(int forumID, int sectionID) {
    view.clearView();
    threadView.clearThreadList();
    threadView.requestThreadList(forumID, sectionID);
    view.appendView(threadView.getView());
  }
  void showPostView(int forumID, int sectionID, int threadID) {
    view.clearView();
    //postView.clearThreadList();
    //postView.requestPostList(forumID, sectionID, threadID);
    view.appendView(postView.getView());
  }
  void loggedIn() {}
  void loggedOut() {}
  void handleMessage(MessageReader message) {
    forumMsgHandler.onBinaryMessage(message);
  }
}

//The forum will display quite a lot of information
/*
On startup the forum will show the Forums with details on each. Post count, popular thread. Forum description.
Underneath the 3 sections it'll show the latest updated threads with information on who replied last,
a small line of text from the last post, the section name it belongs to. DateTime info. It'll show
about 30.
On clicking a Forum, it'll show sections. Each section will list the latest updated thread,
the number of threads, number of posts, section description. Could be a lot of sections.
In the future will need to make it have pages or something.
On clicking a section, it'll show Threads. Thread will have a name, latest post, time stamp, post count.
On clicking a thread it'll go to the first post in the thread, on clicking the latest
post button it'll show the latest post.
Gonna have to make a page system. Also posts and sections/forums/threads that get updated
that you are viewing will have to be tracked so they can be updated before your eyes.

 */
