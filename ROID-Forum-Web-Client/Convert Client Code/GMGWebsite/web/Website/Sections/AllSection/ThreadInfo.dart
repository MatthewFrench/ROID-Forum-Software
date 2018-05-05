library AllThreadInfo;
import 'ThreadViews/HeaderView.dart';
import 'ThreadController.dart';

class ThreadInfo {
  int _id;
  String _section;
  String _title;
  String _owner;
  String _avatarURL = "";
  int _commentCount;
  HeaderView headerView;
  ThreadController threadController;
  ThreadInfo(ThreadController tc) {
    threadController = tc;
    headerView = new HeaderView(this, tc);
    headerView.updateCommentCount();
  }
  void setID(int id) {
    _id = id;
  }
  int getID() {
    return _id;
  }
  void setSection(String section) {
      _section = section;
      headerView.updateSection();
    }
    String getSection() {
      return _section;
    }
  void setTitle(String title) {
    _title = title;
    headerView.updateTitle();
  }
  String getTitle() {
    return _title;
  }
  void setCommentCount(int commentCount) {
    _commentCount = commentCount;
    headerView.updateCommentCount();
  }
  int getCommentCount() {
    return _commentCount;
  }
  void setOwner(String owner) {
    _owner = owner;
    headerView.updateOwner();
  }
  String getOwner() {
    return _owner;
  }
  void setAvatarURL(String avatarURL) {
      _avatarURL = avatarURL;
      headerView.updateAvatarURL();
    }
    String getAvatarURL() {
      return _avatarURL;
    }
}
