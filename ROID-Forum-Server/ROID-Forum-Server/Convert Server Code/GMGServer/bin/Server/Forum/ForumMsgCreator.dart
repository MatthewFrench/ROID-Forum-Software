library ForumMsgCreator;
import '../Networking/MessageWriter.dart';
import '../Database/Forum.dart';
import '../Database/Section.dart';
import '../Database/Thread.dart';

const int Forum_Controller_Binary = 1;

//Messages to send to the client
const int MSG_PEOPLE_ONLINE = 0;
const int MSG_PEOPLE_REGISTERED = 1;
const int MSG_FORUM_LIST = 2;
const int MSG_SECTION_LIST = 3;
const int MSG_THREAD_LIST = 4;

class ForumMsgCreator {
  static MessageWriter ThreadList(String forumName, String sectionName, String sectionDescription, List<Thread> threads) {
    MessageWriter m = new MessageWriter();
    m.addUint8(Forum_Controller_Binary);
    m.addUint8(MSG_THREAD_LIST);
    m.addString(forumName);
    m.addString(sectionName);
    m.addString(sectionDescription);
    m.addUint32(threads.length);
    for (Thread thread in threads) {
      m.addUint32(thread.id);
      m.addUint32(thread.sectionID);
      m.addUint32(thread.forumID);
      m.addString(thread.name);
      m.addString(thread.creatorName);
      m.addUint32(thread.replies);
      m.addUint32(thread.views);
      m.addString(thread.lastPostAuthor);
      m.addString(thread.lastPostDate.toIso8601String());
    }
    return m;
  }
  static MessageWriter SectionList(String forumName, String forumDescription, List<Section> sections) {
    MessageWriter m = new MessageWriter();
    m.addUint8(Forum_Controller_Binary);
    m.addUint8(MSG_SECTION_LIST);
    m.addString(forumName);
    m.addString(forumDescription);
    m.addUint32(sections.length);
    for (Section section in sections) {
      m.addUint32(section.id);
      m.addUint32(section.forumID);
      m.addString(section.name);
      m.addString(section.description);
      m.addUint32(section.threadCount);
      m.addUint32(section.postCount);
      m.addString(section.lastPostThreadName);
      m.addString(section.lastPostAuthor);
      m.addString(section.lastPostDate.toIso8601String());
    }
    return m;
  }
  static MessageWriter ForumList(List<Forum> forums) {
    MessageWriter m = new MessageWriter();
    m.addUint8(Forum_Controller_Binary);
    m.addUint8(MSG_FORUM_LIST);
    m.addUint32(forums.length);
    for (Forum forum in forums) {
      m.addString(forum.name);
      m.addString(forum.description);
      m.addUint32(forum.id);
      m.addUint32(forum.sectionCount);
      m.addUint32(forum.threadCount);
      m.addUint32(forum.postCount);
      m.addString(forum.lastPostThreadName);
      m.addString(forum.lastPostAuthor);
      m.addString(forum.lastPostDate.toIso8601String());
    }
    return m;
  }
  static MessageWriter PeopleRegistered(int num) {
    MessageWriter m = new MessageWriter();
    m.addUint8(Forum_Controller_Binary);
    m.addUint8(MSG_PEOPLE_REGISTERED);
    m.addInt32(num);
    return m;
  }
  static MessageWriter PeopleOnline(int num) {
    MessageWriter m = new MessageWriter();
    m.addUint8(Forum_Controller_Binary);
    m.addUint8(MSG_PEOPLE_ONLINE);
    m.addInt32(num);
    return m;
  }
}