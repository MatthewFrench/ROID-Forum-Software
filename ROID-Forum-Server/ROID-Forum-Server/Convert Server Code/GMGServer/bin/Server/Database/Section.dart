library Section;

//    Sections
//        ID
//        ForumID
//        CreatorID
//        Name
//        Description
//        CreatedDate

class Section {
  int id, forumID, creatorID;
  String name, description;
  DateTime createdDate;

  int threadCount;
  int postCount;
  DateTime lastPostDate;
  String lastPostAuthor;
  String lastPostThreadName;
  String forumName;

  Section(Map m) {
    //    Sections
    //        ID
    //        ForumID
    //        CreatorID
    //        Name
    //        Description
    //        CreatedDate
    id = m['ID'];
    forumID = m['ForumID'];
    creatorID = m['CreatorID'];
    name = m['Name'];
    description = m['Description'];
    createdDate = m['CreatedDate'];
  }
}
