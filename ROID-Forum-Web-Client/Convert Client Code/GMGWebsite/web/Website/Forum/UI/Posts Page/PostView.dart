library PostView;
import '../../ForumController.dart';
import 'dart:html';
//import '../../ForumMsgCreator.dart';
import '../../../Networking/NetworkingController.dart';
//import 'PostViewPiece.dart';
class PostView {
  ForumController forum;
  NetworkingController networkingController;

  //List<PostViewPiece> postPieces;

  DivElement postHolder;
  DivElement websiteNameDiv;
  DivElement forumNameDiv;
  SpanElement forumNameText;
  DivElement sectionNameDiv;
  SpanElement sectionNameText;
  DivElement descriptionDiv;
  DivElement pagesDiv;
  int sectionID;
  PostView(this.forum) {
    networkingController = forum.website.networkingController;
    //postPieces = new List();

    postHolder = createPostHolder();
    createWebsiteName();
    postHolder.append(websiteNameDiv);
    createForumName();
    postHolder.append(forumNameDiv);
    createSectionName();
    postHolder.append(sectionNameDiv);

    descriptionDiv = createSectionDescription();
    postHolder.append(descriptionDiv);

    createPagesDiv();
    postHolder.append(pagesDiv);
    postHolder.append(createPostLabel());
  }
  void clickedForumName() {
    forum.showForums();
  }
  void clickedSectionName() {
    forum.showSections(sectionID);
  }
  /*
  void requestPostList(int forumID, int _sectionID) {
    sectionID = _sectionID;
    //Ask server for list of posts
    networkingController.SendBinary(ForumMsgCreator.RequestPostList(forumID, sectionID), true);
  }*/
  /*
  void clearPostList() {
    for (PostViewPiece s in postPieces) {
      s.getDiv().remove();
    }
    postPieces.clear();
  }*/
  /*
  void processPostList(String forumName, String sectionName, String sectionDescription, List<Map> postList) {
    forumNameText.text = forumName;
    sectionNameText.text = sectionName;
    descriptionDiv.text = sectionDescription;
    for (Map f in postList) {
      int id = f['ID'];
      int sectionID = f['SectionID'];
      int forumID = f['ForumID'];
      String name = f['Name'];
      String creatorName = f['CreatorName'];
      int replies = f['Replies'];
      int views = f['Views'];
      String lastPostAuthor = f['LastPostAuthor'];
      DateTime lastPostDate = f['LastPostDate'];


      PostViewPiece piece = postPieces.firstWhere((PostViewPiece element) {return element.id == id;}, orElse: () => null);
      if (piece == null) {
        piece = new PostViewPiece(id, sectionID, forumID, name, creatorName, replies, views, lastPostAuthor, lastPostDate);
        postPieces.add(piece);
        postHolder.append(piece.getDiv());
      } else {
        piece.updateName(name);
        piece.updateLastPostAuthor(lastPostAuthor);
        piece.updateLastPostDate(lastPostDate);
        piece.updateRepliesCount(replies);
        piece.updateViewsCount(views);
        piece.updateStartedByName(creatorName);
      }
    }
  }*/
  DivElement getView() {
    return postHolder;
  }


  void createWebsiteName() {
    websiteNameDiv = new DivElement();
    websiteNameDiv.id = "postView";
    websiteNameDiv.className = "websiteNameDiv";
    websiteNameDiv.onClick.listen((_){
      clickedForumName();
    });
    ImageElement folderImage = new ImageElement(
        src: "Resources/GMGFolderIcon.png");
    websiteNameDiv.append(folderImage);
    folderImage.id = "postView";
    folderImage.className = "folderImage";
    SpanElement nameText = new SpanElement();
    nameText.text = "Game Maker's Garage";
    websiteNameDiv.append(nameText);
  }

/*
  void createForumName() {
    forumNameDiv = new DivElement();
    forumNameDiv.style
      ..paddingLeft = "3px"
      ..width = "calc(100% - 3px)"
      ..color = "rgb(22, 20, 114)"
      ..fontSize = "20px"
      ..display = "inline-block"
      ..textOverflow = "ellipsis"
      ..overflow = "hidden"
      ..whiteSpace = "nowrap"
      ..fontWeight = "bold"
      ..cursor = "pointer";
    forumNameDiv.onClick.listen((_){
      clickedForumName();
    });
  }
  */
  void createForumName() {
    forumNameDiv = new DivElement();
    forumNameDiv.id = "postView";
    forumNameDiv.className = "forumNameDiv";

    forumNameDiv.onClick.listen((_){
      clickedSectionName();
    });
    ImageElement subFolderImage = new ImageElement(
        src: "Resources/GMGSubFolderIcon.png");
    forumNameDiv.append(subFolderImage);

    subFolderImage.id = "postView";
    subFolderImage.className = "subFolderImage";
    ImageElement folderImage = new ImageElement(
        src: "Resources/GMGFolderIcon.png");
    forumNameDiv.append(folderImage);
    folderImage.id = "postView";
    folderImage.className = "folderImage2";
    forumNameText = new SpanElement();
    forumNameDiv.append(forumNameText);
  }
  void createSectionName() {
    sectionNameDiv = new DivElement();
    sectionNameDiv.id = "postView";
    sectionNameDiv.className = "sectionNameDiv";
    ImageElement subFolderImage = new ImageElement(
        src: "Resources/GMGSubFolderIcon.png");
    sectionNameDiv.append(subFolderImage);
    subFolderImage.id = "postView";
    subFolderImage.className = "subFolderImage2";
    ImageElement folderImage = new ImageElement(
        src: "Resources/GMGFolderIcon.png");
    sectionNameDiv.append(folderImage);
    folderImage.id = "postView";
    folderImage.className = "folderImage2";
    sectionNameText = new SpanElement();
    sectionNameDiv.append(sectionNameText);
  }
  void createPagesDiv() {
    pagesDiv = new DivElement();
    pagesDiv.id = "postView";
    pagesDiv.className = "pagesDiv";

    SpanElement pagesSpan = new SpanElement();
    pagesSpan.text = "Pages: ";
    pagesSpan.id = "postView";
    pagesSpan.className = "pagesSpan";

    //Add page numbers
    //Add Start New Topic with image icon

    pagesDiv.append(pagesSpan);

    SpanElement pageNumbers = new SpanElement();
    pageNumbers.text = "1";
    pageNumbers.id = "postView";
    pageNumbers.className = "pagesSpan";
    pagesDiv.append(pageNumbers);

    SpanElement newSectionSpan = new SpanElement();
    newSectionSpan.id = "postView";
    newSectionSpan.className = "newSectionSpan";
    newSectionSpan.text = "Start new topic";
    pagesDiv.append(newSectionSpan);

    ImageElement newSectionIcon = new ImageElement(src:"Resources/GMGNewPostIcon.png");

    newSectionIcon.id = "postView";
    newSectionIcon.className = "newSectionIcon";
    pagesDiv.append(newSectionIcon);
  }
}

DivElement createSectionDescription() {
  DivElement descriptionDiv = new DivElement();
  descriptionDiv.id = "postView";
  descriptionDiv.className = "descriptionDiv";
  return descriptionDiv;
}
DivElement createPostHolder() {
  DivElement postHolder = new DivElement();
  postHolder.id = "postView";
  postHolder.className = "postHolder";
  return postHolder;
}
DivElement createPostLabel() {
  //Create green bar of labels
  DivElement postLabel = new DivElement();
  postLabel.id = "postView";
  postLabel.className = "postLabel";

  LabelElement flagLabel = new LabelElement();
  flagLabel.id = "postView";
  flagLabel.className = "flagLabel";
  postLabel.append(flagLabel);

  LabelElement postNameLabel = new LabelElement();
  postNameLabel.text = "Subject";
  postNameLabel.id = "postView";
  postNameLabel.className = "postNameLabel";
  postLabel.append(postNameLabel);
  LabelElement startedByLabel = new LabelElement();
  startedByLabel.text = "Started by";

  startedByLabel.id = "postView";
  startedByLabel.className = "startedByLabel";
  postLabel.append(startedByLabel);
  LabelElement repliesLabel = new LabelElement();
  repliesLabel.text = "Replies";
  repliesLabel.id = "postView";
  repliesLabel.className = "repliesLabel";
  postLabel.append(repliesLabel);

  LabelElement viewsLabel = new LabelElement();
  viewsLabel.text = "Views";

  viewsLabel.id = "postView";
  viewsLabel.className = "viewsLabel";
  postLabel.append(viewsLabel);

  LabelElement postLastPostLabel = new LabelElement();
  postLastPostLabel.text = "Last post";
  postLastPostLabel.id = "postView";
  postLastPostLabel.className = "postLastPostLabel";
  postLabel.append(postLastPostLabel);

  return postLabel;
}