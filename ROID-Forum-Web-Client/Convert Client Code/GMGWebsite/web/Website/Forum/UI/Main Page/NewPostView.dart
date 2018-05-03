library NewPostView;

import '../../ForumController.dart';
import 'dart:html';
import '../../ForumMsgCreator.dart';
import '../../../Networking/NetworkingController.dart';
import 'NewPostViewPiece.dart';
class NewPostView {
  ForumController forum;
  NetworkingController networkingController;
  List<NewPostViewPiece> newPostPieces;
  DivElement newPostHolder;
  NewPostView(this.forum) {
    networkingController = forum.website.networkingController;
    newPostPieces = new List();

    newPostHolder = createNewPostHolder();

    newPostHolder.append(createNewPostLabel());
  }
  void requestForumList() {
    //Ask server for list of forums
    networkingController.SendBinary(ForumMsgCreator.RequestForumList(), true);
  }
  void processForumList(List<Map> forumList) {
    for (Map f in forumList) {
      String name = f['Name'];
      String description = f['Description'];
      int id = f['ID'];
      print("id: ${id}");
      NewPostViewPiece piece = newPostPieces.firstWhere((NewPostViewPiece element) {return element.id == id;}, orElse: () => null);
      print("Piece: ${piece}");
      if (piece == null) {
        piece = new NewPostViewPiece(id, name, description, 0, 0, 0);
        newPostPieces.add(piece);
        newPostHolder.append(piece.getDiv());
      } else {
        piece.updateName(name);
        piece.updateDescription(description);
      }
    }
  }
  DivElement getView() {
    return newPostHolder;
  }
}

DivElement createNewPostHolder() {
  DivElement newPostHolder = new DivElement();
  newPostHolder.id = "newPostView";
    newPostHolder.className = "newPostHolder";
  return newPostHolder;
}

DivElement createNewPostLabel() {
  DivElement newPostLabel = new DivElement();
  newPostLabel.id = "newPostView";
  newPostLabel.className = "newPostLabel";
  LabelElement newPostNameLabel = new LabelElement();
  newPostNameLabel.text = "Latest Posts";
  newPostNameLabel.id = "newPostView";
  newPostNameLabel.className = "newPostNameLabel";
  newPostLabel.append(newPostNameLabel);
  LabelElement newPostLocationLabel = new LabelElement();
  newPostLocationLabel.text = "Location";
  newPostLocationLabel.id = "newPostView";
  newPostLocationLabel.className = "newPostLocationLabel";
  newPostLabel.append(newPostLocationLabel);
  LabelElement newPostAuthorDateLabel = new LabelElement();
  newPostAuthorDateLabel.text = "Author and Date";
  newPostAuthorDateLabel.id = "newPostView";
  newPostAuthorDateLabel.className = "newPostAuthorDateLabel";
  newPostLabel.append(newPostAuthorDateLabel);

  return newPostLabel;
}