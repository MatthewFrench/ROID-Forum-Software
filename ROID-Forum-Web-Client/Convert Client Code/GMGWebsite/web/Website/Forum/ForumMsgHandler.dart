library ForumMsgHandler;
import 'ForumController.dart';
import '../Networking/MessageReader.dart';

const int User_Controller_Binary = 0;
const int Forum_Controller_Binary = 1;

//Messages from server
const int MSG_PEOPLE_ONLINE = 0;
const int MSG_PEOPLE_REGISTERED = 1;
const int MSG_FORUM_LIST = 2;
const int MSG_SECTION_LIST = 3;
const int MSG_THREAD_LIST = 4;

class ForumMsgHandler {
  ForumController forum;

  ForumMsgHandler(this.forum) {
  }

  void onBinaryMessage(MessageReader message) {
    int binaryMsg = message.getUint8();
    switch (binaryMsg) {
      case MSG_THREAD_LIST: {
        String forumName = message.getString();
        String sectionName = message.getString();
        String sectionDescription = message.getString();
        int numOfThreads = message.getUint32();
        List<Map> threadList = new List<Map>();
        for (var i = 0; i < numOfThreads; i++) {
          Map thread = new Map();

          thread['ID'] = message.getUint32();
          thread['SectionID'] = message.getUint32();
          thread['ForumID'] = message.getUint32();
          thread['Name'] = message.getString();
          thread['CreatorName'] = message.getString();
          thread['Replies'] = message.getUint32();
          thread['Views'] = message.getUint32();
          thread['LastPostAuthor'] = message.getString();
          thread['LastPostDate'] = DateTime.parse(message.getString());
          threadList.add(thread);
        }
        forum.threadView.processThreadList(forumName, sectionName, sectionDescription, threadList);
      } break;
      case MSG_SECTION_LIST: {
        //Process section list
        String forumName = message.getString();
        String forumDescription = message.getString();
        int numOfSections = message.getUint32();
        List<Map> sectionList = new List<Map>();
        for (var i = 0; i < numOfSections; i++) {
          Map section = new Map();
          section['ID'] = message.getUint32();
          section['ForumID'] = message.getUint32();
          section['Name'] = message.getString();
          section['Description'] = message.getString();
          section['ThreadCount'] = message.getUint32();
          section['PostCount'] = message.getUint32();
          section['LastPostThreadName'] = message.getString();
          section['LastPostAuthor'] = message.getString();
          section['LastPostDate'] = DateTime.parse(message.getString());
          sectionList.add(section);
        }
        forum.sectionView.processSectionList(forumName, forumDescription, sectionList);
      } break;
      case MSG_FORUM_LIST: {
        //Process forum list
        int numOfForums = message.getUint32();
        List<Map> forumList = new List<Map>();
        for (var i = 0; i < numOfForums; i++) {
          Map forum = new Map();
          forum['Name'] = message.getString();
          forum['Description'] = message.getString();
          forum['ID'] = message.getUint32();
          forum['SectionCount'] = message.getUint32();
          forum['ThreadCount'] = message.getUint32();
          forum['PostCount'] = message.getUint32();
          forum['LastPostThreadName'] = message.getString();
          forum['LastPostAuthor'] = message.getString();
          forum['LastPostDate'] = DateTime.parse(message.getString());
          forumList.add(forum);
        }
        forum.forumView.processForumList(forumList);
      } break;
      case MSG_PEOPLE_ONLINE:
        {
          int num = message.getInt32();
          forum.website.headerGui.updatePeopleOnline(num);
        }
        break;
      case MSG_PEOPLE_REGISTERED:
        {
          int num = message.getInt32();
          forum.website.headerGui.updateRegisteredPeople(num);
        }
        break;
    }
  }
}