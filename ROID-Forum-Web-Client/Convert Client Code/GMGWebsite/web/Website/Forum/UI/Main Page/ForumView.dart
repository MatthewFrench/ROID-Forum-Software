library ForumView;
import '../../ForumController.dart';
import 'dart:html';
import '../../ForumMsgCreator.dart';
import '../../../Networking/NetworkingController.dart';
import 'ForumViewPiece.dart';
class ForumView {
  ForumController forum;
  NetworkingController networkingController;
  List<ForumViewPiece> forumPieces;
  DivElement forumHolder;
  DivElement websiteNameDiv;
  ForumView(this.forum) {
    networkingController = forum.website.networkingController;
    forumPieces = new List();

    forumHolder = createForumHolder();
    createWebsiteName();
    forumHolder.append(websiteNameDiv);
    forumHolder.append(createForumLabel());
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
      int sectionCount = f['SectionCount'];
      int threadCount = f['ThreadCount'];
      int postCount = f['PostCount'];
      String lastPostThreadName = f['LastPostThreadName'];
      String lastPostAuthor = f['LastPostAuthor'];
      DateTime lastPostDate = f['LastPostDate'];


      ForumViewPiece piece = forumPieces.firstWhere((ForumViewPiece element) {return element.id == id;}, orElse: () => null);
      if (piece == null) {
        piece = new ForumViewPiece(this, id, name, description, sectionCount, threadCount, postCount, lastPostThreadName, lastPostAuthor, lastPostDate);
        forumPieces.add(piece);
        forumHolder.append(piece.getDiv());
      } else {
        piece.updateDescription(description);
        piece.updateName(name);
        piece.updateSectionCount(sectionCount);
        piece.updateLastPostAuthor(lastPostAuthor);
        piece.updateLastPostDate(lastPostDate);
        piece.updateLastPostThreadName(lastPostThreadName);
        piece.updateThreadCount(threadCount);
        piece.updatePostCount(postCount);
      }
    }
  }
  DivElement getView() {
    return forumHolder;
  }

  void createWebsiteName() {
    websiteNameDiv = new DivElement();
    websiteNameDiv.id = "forumView";
    websiteNameDiv.className = "forumWebsiteNameDiv";
    ImageElement folderImage = new ImageElement(src: "Resources/GMGFolderIcon.png");
    folderImage.id = "forumView";
    folderImage.className = "folderImage";
    websiteNameDiv.append(folderImage);
    SpanElement nameText = new SpanElement();
    nameText.text = "Game Maker's Garage";
    websiteNameDiv.append(nameText);
  }
}

DivElement createForumHolder() {
  DivElement forumHolder = new DivElement();
  forumHolder.id = "forumView";
  forumHolder.className = "forumHolder";
  return forumHolder;
}

DivElement createForumLabel() {
  //Create green bar of labels
  //23 pixels in height is label
  DivElement forumLabel = new DivElement();
  forumLabel.id = "forumView";
  forumLabel.className = "forumLabel";
  LabelElement forumNameLabel = new LabelElement();
  forumNameLabel.id = "forumView";
  forumNameLabel.className = "forumNameLabel";
  forumNameLabel.text = "Forums";
  forumLabel.append(forumNameLabel);
  LabelElement forumSectionsLabel = new LabelElement();
  forumSectionsLabel.id = "forumView";
  forumSectionsLabel.className = "forumSectionsLabel";
  forumSectionsLabel.text = "Sections";
  forumLabel.append(forumSectionsLabel);
  LabelElement forumThreadsLabel = new LabelElement();
  forumThreadsLabel.id = "forumView";
  forumThreadsLabel.className = "forumThreadsLabel";
  forumThreadsLabel.text = "Threads";
  forumLabel.append(forumThreadsLabel);
  LabelElement forumPostsLabel = new LabelElement();
  forumPostsLabel.id = "forumView";
  forumPostsLabel.className = "forumPostsLabel";
  forumPostsLabel.text = "Posts";
  forumLabel.append(forumPostsLabel);
  LabelElement forumLastPostLabel = new LabelElement();
  forumLastPostLabel.id = "forumView";
  forumLastPostLabel.className = "forumLastPostLabel";
  forumLastPostLabel.text = "Last Post";
  forumLabel.append(forumLastPostLabel);

  return forumLabel;
}