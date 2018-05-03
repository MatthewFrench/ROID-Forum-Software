library UserMsgHandler;
import '../User/User.dart';
import '../Networking/MessageReader.dart';
import 'ForumController.dart';
import 'ForumMsgCreator.dart';
import '../Database/Forum.dart';
import '../Database/DatabaseController.dart';
import '../Database/Post.dart';
import '../Database/Section.dart';
import '../Database/Thread.dart';
//import 'dart:io';

//Messages from the client
const int MSG_REQUEST_FORUM_LIST = 0;
const int MSG_REQUEST_SECTION_LIST = 1;
const int MSG_REQUEST_THREAD_LIST = 2;
const int MSG_REQUEST_POST_LIST = 3;

class ForumMsgHandler {
  ForumController forum;

  ForumMsgHandler(this.forum) {
  }
  void onBinaryMessage(User u, MessageReader message) {
    DatabaseController database = forum.server.databaseController;
    int binaryMsg = message.getUint8();
    switch (binaryMsg) {
      case MSG_REQUEST_POST_LIST: {
        int forumID = message.getUint32();
        int sectionID = message.getUint32();
        int threadID = message.getUint32();
        forum.server.databaseController.getPosts(forumID, sectionID, threadID).then((List<Post> posts) async {
          String forumName = await database.getForumName(forumID);
          String sectionName = await database.getSectionName(forumID, sectionID);
          //String threadName = await database.getThreadName(forumID, sectionID, threadID);
          for (Post p in posts) {
            /*
            Need
            Author Name
            Author Avatar
            Author Avatar Quote
            Author Number of Posts
            Author Post Quote
             */
          }
          //u.sendBinary(ForumMsgCreator.PostList(forumName, sectionName, threadName, posts), true);
        });
      } break;
      case MSG_REQUEST_THREAD_LIST: {
        int forumID = message.getUint32();
        int sectionID = message.getUint32();
        forum.server.databaseController.getThreads(forumID, sectionID).then((List<Thread> threads) async {
          String forumName = await database.getForumName(forumID);
          String sectionName = await database.getSectionName(forumID, sectionID);
          String sectionDescription = await database.getSectionDescription(forumID, sectionID);
          for (Thread t in threads) {
            //t.views = await database.getViewCountInThread(t.forumID, t.sectionID, t.id);
            t.replies = await database.getReplyCountInThread(t.forumID, t.sectionID, t.id);
            t.creatorName = await database.getDiplayNameForAccount(t.creatorID);

              Post post = await database.getLastPostForThread(t.forumID, t.sectionID, t.id);
              if (post != null) {
                t.lastPostDate = post.editedDate;
                t.lastPostAuthor =
                await database.getDiplayNameForAccount(post.creatorID);
                //t.lastPostThreadName =
                //await database.getThreadName(post.threadID);
              } else {
                t.lastPostDate = new DateTime.now();
                t.lastPostAuthor = "No Author";
                //t.lastPostThreadName = "No Thread Name";
              }

          }
          //Send forum list
          u.sendBinary(ForumMsgCreator.ThreadList(forumName, sectionName, sectionDescription, threads) ,true);

        });
      } break;
      case MSG_REQUEST_SECTION_LIST:
        {
          int forumID = message.getUint32();

          forum.server.databaseController.getSections(forumID).then((List<Section> sections) async {
            String forumName = await database.getForumName(forumID);
            String forumDescription = await database.getForumDescription(forumID);
            for (Section s in sections) {
              s.threadCount = await database.getThreadCountInSection(s.forumID, s.id);
              s.postCount = await database.getPostCountInSection(s.forumID, s.id);
              Post post = await database.getLastPostForSection(s.forumID, s.id);
              if (post != null) {
                s.lastPostDate = post.editedDate;
                s.lastPostAuthor =
                await database.getDiplayNameForAccount(post.creatorID);
                s.lastPostThreadName =
                await database.getThreadName(post.threadID);
              } else {
                s.lastPostDate = new DateTime.now();
                s.lastPostAuthor = "No Author";
                s.lastPostThreadName = "No Thread Name";
              }
            }
            //Send forum list
            u.sendBinary(ForumMsgCreator.SectionList(forumName, forumDescription, sections) ,true);
          });
        } break;
      case MSG_REQUEST_FORUM_LIST:
        {
          //Get list of forums from MongoDB
          forum.server.databaseController.getForums().then((List<Forum> forums) async{

            //Need section count, thread count, post count
            //Last post date, author and thread it's in
            for (Forum f in forums) {
              f.sectionCount = await database.getSectionCountInForum(f.id);
              f.threadCount = await database.getThreadCountInForum(f.id);
              f.postCount = await database.getPostCountInForum(f.id);
              Post post = await database.getLastPostForForum(f.id);
              if (post != null) {
                f.lastPostDate = post.editedDate;
                f.lastPostAuthor =
                await database.getDiplayNameForAccount(post.creatorID);
                f.lastPostThreadName =
                await database.getThreadName(post.threadID);
              } else {
                f.lastPostDate = new DateTime.now();
                f.lastPostAuthor = "No Author";
                f.lastPostThreadName = "No Thread Name";
              }
            }

            //Send forum list
            u.sendBinary(ForumMsgCreator.ForumList(forums) ,true);
          });
        }
        break;
    }
  }
}