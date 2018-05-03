library Post;

//    Posts
//        ID
//        ThreadID
//        SectionID
//        ForumID
//        CreatorID
//        Content
//        CreatedDate
//        EditedDate

class Post {
  int id, threadID, sectionID, forumID, creatorID;
  String content;
  DateTime createdDate, editedDate;

  Post(Map m) {
    id = m['ID'];
    threadID = m['ThreadID'];
    sectionID = m['SectionID'];
    forumID = m['ForumID'];
    creatorID = m['CreatorID'];
    content = m['Content'];
    createdDate = m['CreatedDate'];
    editedDate = m['EditedDate'];
    if (editedDate == null) {
      editedDate = createdDate;
    }
  }
}
