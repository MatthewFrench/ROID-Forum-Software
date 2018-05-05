library GraphicsThreadInfo;
import 'CommentInfo.dart';
import 'ThreadViews/FullView.dart';
import 'ThreadViews/HeaderView.dart';
import 'ThreadController.dart';

class ThreadInfo {
  int _id;
  String _title;
  String _description;
  String _owner;
  String _avatarURL = "";
  List<CommentInfo> _comments;
  HeaderView headerView;
  FullView fullView;
  ThreadController threadController;
  ThreadInfo(ThreadController tc) {
    threadController = tc;
    _comments = new List();
    headerView = new HeaderView(this, tc);
    fullView = new FullView(this, tc);
    headerView.updateCommentCount();
  }
  void setID(int id) {
    _id = id;
  }
  int getID() {
    return _id;
  }
  void setTitle(String title) {
    _title = title;
    headerView.updateTitle();
    fullView.threadDisplay.updateTitle();
  }
  String getTitle() {
    return _title;
  }
  void setDescription(String description) {
    _description = description;
    fullView.threadDisplay.updateDescription();
  }
  String getDescription() {
    return _description;
  }
  void setOwner(String owner) {
    _owner = owner;
    headerView.updateOwner();
    fullView.threadDisplay.updateOwner();
  }
  String getOwner() {
    return _owner;
  }

  void setAvatarURL(String avatarURL) {
      _avatarURL = avatarURL;
      headerView.updateAvatarURL();
      fullView.threadDisplay.updateAvatarURL();
    }
    String getAvatarURL() {
      return _avatarURL;
    }
  int getCommentCount() {
    return _comments.length;
  }
  void addComment(CommentInfo comment) {
    _comments.add(comment);
    fullView.addComment(comment);
    headerView.updateCommentCount();
  }
  CommentInfo getComment(int commentID) {
    for (int i = 0; i < _comments.length; i++) {
      CommentInfo ci = _comments[i];
      if (ci.getCommentID() == commentID) {
        return ci;
      }
    }

    return null;
  }
  CommentInfo removeComment(int commentID) {
      CommentInfo c = getComment(commentID);
      if (c != null) {
        _comments.remove(c);
        fullView.removeComment(c);
        headerView.updateCommentCount();
      }
      return c;
    }
}
