library ThreadView;
import '../../ForumController.dart';
import 'dart:html';
import '../../ForumMsgCreator.dart';
import '../../../Networking/NetworkingController.dart';
import 'ThreadViewPiece.dart';
class ThreadView {
  ForumController forum;
  NetworkingController networkingController;
  List<ThreadViewPiece> threadPieces;
  DivElement threadHolder;
  DivElement websiteNameDiv;
  DivElement forumNameDiv;
  SpanElement forumNameText;
  DivElement sectionNameDiv;
  SpanElement sectionNameText;
  DivElement descriptionDiv;
  DivElement pagesDiv;
  int sectionID;
  ThreadView(this.forum) {
    networkingController = forum.website.networkingController;
    threadPieces = new List();

    threadHolder = createThreadHolder();
    createWebsiteName();
    threadHolder.append(websiteNameDiv);
    createForumName();
    threadHolder.append(forumNameDiv);
    createSectionName();
    threadHolder.append(sectionNameDiv);

    descriptionDiv = createSectionDescription();
    threadHolder.append(descriptionDiv);

    createPagesDiv();
    threadHolder.append(pagesDiv);
    threadHolder.append(createThreadLabel());
  }
  void clickedForumName() {
    forum.showForums();
  }
  void clickedSectionName() {
    forum.showSections(sectionID);
  }
  void requestThreadList(int forumID, int _sectionID) {
    sectionID = _sectionID;
    //Ask server for list of threads
    networkingController.SendBinary(ForumMsgCreator.RequestThreadList(forumID, sectionID), true);
  }
  void clearThreadList() {
    for (ThreadViewPiece s in threadPieces) {
      s.getDiv().remove();
    }
    threadPieces.clear();
  }
  void processThreadList(String forumName, String sectionName, String sectionDescription, List<Map> threadList) {
    forumNameText.text = forumName;
    sectionNameText.text = sectionName;
    descriptionDiv.text = sectionDescription;
    for (Map f in threadList) {
      int id = f['ID'];
      int sectionID = f['SectionID'];
      int forumID = f['ForumID'];
      String name = f['Name'];
      String creatorName = f['CreatorName'];
      int replies = f['Replies'];
      int views = f['Views'];
      String lastPostAuthor = f['LastPostAuthor'];
      DateTime lastPostDate = f['LastPostDate'];


      ThreadViewPiece piece = threadPieces.firstWhere((ThreadViewPiece element) {return element.id == id;}, orElse: () => null);
      if (piece == null) {
        piece = new ThreadViewPiece(this, id, sectionID, forumID, name, creatorName, replies, views, lastPostAuthor, lastPostDate);
        threadPieces.add(piece);
        threadHolder.append(piece.getDiv());
      } else {
        piece.updateName(name);
        piece.updateLastPostAuthor(lastPostAuthor);
        piece.updateLastPostDate(lastPostDate);
        piece.updateRepliesCount(replies);
        piece.updateViewsCount(views);
        piece.updateStartedByName(creatorName);
      }
    }

  }
  DivElement getView() {
    return threadHolder;
  }


  void createWebsiteName() {
    websiteNameDiv = new DivElement();
    websiteNameDiv.id = "threadView";
    websiteNameDiv.className = "websiteNameDiv";
    websiteNameDiv.onClick.listen((_){
      clickedForumName();
    });
    ImageElement folderImage = new ImageElement(
        src: "Resources/GMGFolderIcon.png");
    websiteNameDiv.append(folderImage);
    folderImage.id = "threadView";
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
    forumNameDiv.id = "threadView";
    forumNameDiv.className = "forumNameDiv";

    forumNameDiv.onClick.listen((_){
      clickedSectionName();
    });
    ImageElement subFolderImage = new ImageElement(
        src: "Resources/GMGSubFolderIcon.png");
    forumNameDiv.append(subFolderImage);

    subFolderImage.id = "threadView";
    subFolderImage.className = "subFolderImage";
    ImageElement folderImage = new ImageElement(
        src: "Resources/GMGFolderIcon.png");
    forumNameDiv.append(folderImage);
    folderImage.id = "threadView";
    folderImage.className = "folderImage2";
    forumNameText = new SpanElement();
    forumNameDiv.append(forumNameText);
  }
  void createSectionName() {
    sectionNameDiv = new DivElement();
    sectionNameDiv.id = "threadView";
    sectionNameDiv.className = "sectionNameDiv";
    ImageElement subFolderImage = new ImageElement(
        src: "Resources/GMGSubFolderIcon.png");
    sectionNameDiv.append(subFolderImage);
    subFolderImage.id = "threadView";
    subFolderImage.className = "subFolderImage2";
    ImageElement folderImage = new ImageElement(
        src: "Resources/GMGFolderIcon.png");
    sectionNameDiv.append(folderImage);
    folderImage.id = "threadView";
    folderImage.className = "folderImage2";
    sectionNameText = new SpanElement();
    sectionNameDiv.append(sectionNameText);
  }
  void createPagesDiv() {
    pagesDiv = new DivElement();
    pagesDiv.id = "threadView";
    pagesDiv.className = "pagesDiv";

    SpanElement pagesSpan = new SpanElement();
    pagesSpan.text = "Pages: ";
    pagesSpan.id = "threadView";
    pagesSpan.className = "pagesSpan";

    //Add page numbers
    //Add Start New Topic with image icon

    pagesDiv.append(pagesSpan);

    SpanElement pageNumbers = new SpanElement();
    pageNumbers.text = "1";
    pageNumbers.id = "threadView";
    pageNumbers.className = "pagesSpan";
    pagesDiv.append(pageNumbers);

    SpanElement newSectionSpan = new SpanElement();
    newSectionSpan.id = "threadView";
    newSectionSpan.className = "newSectionSpan";
    newSectionSpan.text = "Start new topic";
    pagesDiv.append(newSectionSpan);

    ImageElement newSectionIcon = new ImageElement(src:"Resources/GMGNewThreadIcon.png");

    newSectionIcon.id = "threadView";
    newSectionIcon.className = "newSectionIcon";
    pagesDiv.append(newSectionIcon);
  }
}

DivElement createSectionDescription() {
  DivElement descriptionDiv = new DivElement();
  descriptionDiv.id = "threadView";
  descriptionDiv.className = "descriptionDiv";
  return descriptionDiv;
}
DivElement createThreadHolder() {
  DivElement threadHolder = new DivElement();
  threadHolder.id = "threadView";
  threadHolder.className = "threadHolder";
  return threadHolder;
}
DivElement createThreadLabel() {
  //Create green bar of labels
  DivElement threadLabel = new DivElement();
  threadLabel.id = "threadView";
  threadLabel.className = "threadLabel";

  LabelElement flagLabel = new LabelElement();
  flagLabel.id = "threadView";
  flagLabel.className = "flagLabel";
  threadLabel.append(flagLabel);

  LabelElement threadNameLabel = new LabelElement();
  threadNameLabel.text = "Subject";
  threadNameLabel.id = "threadView";
  threadNameLabel.className = "threadNameLabel";
  //..minWidth = "439px";
  threadLabel.append(threadNameLabel);
  LabelElement startedByLabel = new LabelElement();
  startedByLabel.text = "Started by";

  startedByLabel.id = "threadView";
  startedByLabel.className = "startedByLabel";
  threadLabel.append(startedByLabel);
  LabelElement repliesLabel = new LabelElement();
  repliesLabel.text = "Replies";
  repliesLabel.id = "threadView";
  repliesLabel.className = "repliesLabel";
  threadLabel.append(repliesLabel);

  LabelElement viewsLabel = new LabelElement();
  viewsLabel.text = "Views";

  viewsLabel.id = "threadView";
  viewsLabel.className = "viewsLabel";
  threadLabel.append(viewsLabel);

  LabelElement threadLastPostLabel = new LabelElement();
  threadLastPostLabel.text = "Last post";
  threadLastPostLabel.id = "threadView";
  threadLastPostLabel.className = "threadLastPostLabel";
  threadLabel.append(threadLastPostLabel);

  return threadLabel;
}