library AllThreadInfo;

class ThreadInfo {
  int threadID;
  String section;
  String title;
  int commentCount = 0;
  String owner;
  ThreadInfo.fromData(int _threadID, String _section, String _title, int _commentCount, String _owner) {
    threadID = _threadID;
    section = _section;
    title = _title;
    commentCount = _commentCount;
    owner = _owner;
  }
  ThreadInfo.fromMap(Map m) {
    threadID = m["ThreadID"];
    section = m["Section"];
    title = m["Title"];
    commentCount = m["Comment Count"];
    owner = m["Owner"];
  }
  Map toMap() {
    Map m = new Map();
    m["ThreadID"] = threadID;
    m["Section"] = section;
    m["Title"] = title;
    m["Comment Count"] = commentCount;
    m["Owner"] = owner;
    return m;
  }
}