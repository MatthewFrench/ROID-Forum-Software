library GameCommentInfo;
import '../../Account Controller/AccountController.dart';

class CommentInfo {
  int threadID;
  int commentID;
  String comment;
  String owner;
  CommentInfo.fromData(int _threadID, int _commentID, String _comment, String _owner) {
    threadID = _threadID;
    commentID = _commentID;
    comment = _comment;
    owner = _owner;
  }
  CommentInfo.fromMap(Map m) {
    threadID = m["ThreadID"];
    commentID = m["CommentID"];
    comment = m['Comment'];
    owner = m['Owner'];
  }
  Map toMap({AccountController addAvatars: null}) {
      Map m = new Map();
      m["ThreadID"] = threadID;
      m["CommentID"] = commentID;
      m['Comment'] = comment;
      m['Owner'] = owner;
      if (addAvatars != null) {
        m['AvatarURL'] = addAvatars.getAvatarForAccount(owner);
      }
      return m;
    }
}