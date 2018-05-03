library Forum;

//    Forums
//        ID
//        Name
//        Description

class Forum {
  int id;
  String name, description;

  int sectionCount;
  int threadCount;
  int postCount;
  DateTime lastPostDate;
  String lastPostAuthor;
  String lastPostThreadName;

  Forum(Map m) {
    id = m['ID'];
    name = m['Name'];
    description = m['Description'];
  }
}
