library GraphicsCommentInfo;
import 'ThreadViews/CommentView.dart';
import 'ThreadController.dart';
import 'ThreadInfo.dart';

class CommentInfo {
  int _threadID;
  int _commentID;
  String _comment;
  String _owner;
  String _avatarURL = "";
  CommentView commentView;
  ThreadInfo thread;
  ThreadController threadController;
  CommentInfo(ThreadInfo t, ThreadController tc) {
    thread = t;
    threadController = tc;
    commentView = new CommentView(this, thread, threadController);
  }
  void setThreadID(int id) {
    _threadID = id;
  }int getThreadID() {return _threadID;}
  void setCommentID(int id) {
    _commentID = id;
  }int getCommentID() {return _commentID;}
  void setComment(String comment) {
    _comment = comment; commentView.updateDescription();
  }String getComment() {return _comment;}
  void setOwner(String owner) {
    _owner = owner; commentView.updateOwner();
  }String getPosterName() {return _owner;}
  void setAvatarURL(String avatarURL) {
        _avatarURL = avatarURL;
        commentView.updateAvatarURL();
      }
      String getAvatarURL() {
        return _avatarURL;
      }
}