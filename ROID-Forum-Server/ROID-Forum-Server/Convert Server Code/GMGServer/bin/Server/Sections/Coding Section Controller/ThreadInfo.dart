library CodingThreadInfo;
import 'CommentInfo.dart';
import '../../Account Controller/AccountController.dart';

class ThreadInfo {
  int id;
  String title;
  String description;
  String owner;
  List<CommentInfo> comments;
  int commentIDs = 0;
  ThreadInfo.fromData(String _owner, int _id, String t, String d) {
    owner = _owner;
    id = _id;
    title = t;
    description = d;
    comments = new List();
  }
  ThreadInfo.fromMap(Map m) {
    owner = m["Owner"];
    id = m["ID"];
    title = m['Title'];
    description = m['Description'];
    List<Map> commentArray = m['Comments'];
    comments = new List();
    if (m['CommentIDs'] != null) {
      commentIDs = m['CommentIDs'];
    }
    //Repair comments when loading them just in case their IDs are wrong.
    for (int i = 0; i < commentArray.length; i++) {
      CommentInfo c = new CommentInfo.fromMap(commentArray[i]);
      if (getCommentForID(c.commentID) != null) {
        c.commentID = commentIDs++;
      }
      comments.add(c);
    }
  }
  Map toMap({AccountController addAvatars:null}) {
    Map m = new Map();
    m["Owner"] = owner;
    m["ID"] = id;
    m['Title'] = title;
    m['Description'] = description;
    if (addAvatars != null) {
      m['AvatarURL'] = addAvatars.getAvatarForAccount(owner);
    } else {
      m['CommentIDs'] = commentIDs;
    }
    List<Map> commentArray = new List();
    for (int i = 0; i < comments.length; i++) {
      commentArray.add(comments[i].toMap(addAvatars:addAvatars));
    }
    m['Comments'] = commentArray;
    return m;
  }
  CommentInfo getCommentForID(int commentID) {
    for (int i = 0; i < comments.length; i++) {
      if (comments[i].commentID == commentID) {
        return comments[i];
      }
    }
    return null;
  }
}