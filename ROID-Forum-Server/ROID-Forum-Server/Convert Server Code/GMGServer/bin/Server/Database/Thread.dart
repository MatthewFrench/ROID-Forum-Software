library Thread;

//    Threads
//        ID
//        SectionID
//        ForumID
//        CreatorID
//        Name
//        CreatedDate

class Thread {
  int id, sectionID, forumID, creatorID;
  String name;
  DateTime createdDate;

  int replies;
  int views;
  String creatorName;
  DateTime lastPostDate;
  String lastPostAuthor;

  Thread(Map m) {
    id = m['ID'];
    sectionID = m['SectionID'];
    forumID = m['ForumID'];
    creatorID = m['CreatorID'];
    name = m['Name'];
    createdDate = m['CreatedDate'];
    views = m['Views'];
  }
}
