library ForumMsgCreator;
import '../Networking/MessageWriter.dart';

const int User_Controller_Binary = 0,
    Forum_Controller_Binary = 1;

//Messages from the client
const int MSG_REQUEST_FORUM_LIST = 0;
const int MSG_REQUEST_SECTION_LIST = 1;
const int MSG_REQUEST_THREAD_LIST = 2;

class ForumMsgCreator {
  static MessageWriter RequestForumList() {
    MessageWriter m = new MessageWriter();
    m.addUint8(Forum_Controller_Binary);
    m.addUint8(MSG_REQUEST_FORUM_LIST);
    return m;
  }
  static MessageWriter RequestSectionList(int forumID) {
    MessageWriter m = new MessageWriter();
    m.addUint8(Forum_Controller_Binary);
    m.addUint8(MSG_REQUEST_SECTION_LIST);
    m.addUint32(forumID);
    return m;
  }
  static MessageWriter RequestThreadList(int forumID, int sectionID) {
    MessageWriter m = new MessageWriter();
    m.addUint8(Forum_Controller_Binary);
    m.addUint8(MSG_REQUEST_THREAD_LIST);
    m.addUint32(forumID);
    m.addUint32(sectionID);
    return m;
  }
}