library SectionView;
import '../../ForumController.dart';
import 'dart:html';
import '../../ForumMsgCreator.dart';
import '../../../Networking/NetworkingController.dart';
import 'SectionViewPiece.dart';
class SectionView {
  ForumController forum;
  NetworkingController networkingController;
  List<SectionViewPiece> sectionPieces;
  DivElement sectionHolder;
  DivElement websiteNameDiv;
  DivElement forumNameDiv;
  SpanElement forumNameText;
  DivElement descriptionDiv;
  DivElement pagesDiv;
  SectionView(this.forum) {
    networkingController = forum.website.networkingController;
    sectionPieces = new List();

    sectionHolder = createSectionHolder();
    createWebsiteName();
    sectionHolder.append(websiteNameDiv);
    createForumName();
    sectionHolder.append(forumNameDiv);
    descriptionDiv = createForumDescription();
    sectionHolder.append(descriptionDiv);
    createPagesDiv();
    sectionHolder.append(pagesDiv);
    sectionHolder.append(createSectionLabel());
  }
  void clickedForumName() {
    forum.showForums();
  }
  void requestSectionList(int forumID) {
    //Ask server for list of sections
    networkingController.SendBinary(ForumMsgCreator.RequestSectionList(forumID), true);
  }
  void clearSectionList() {
    for (SectionViewPiece s in sectionPieces) {
      s.getDiv().remove();
    }
    sectionPieces.clear();
  }
  void processSectionList(String forumName, String forumDescription, List<Map> sectionList) {
    forumNameText.text = forumName;
    descriptionDiv.text = forumDescription;
    for (Map f in sectionList) {
      int id = f['ID'];
      int forumID = f['ForumID'];
      String name = f['Name'];
      String description = f['Description'];
      int threadCount = f['ThreadCount'];
      int postCount = f['PostCount'];
      String lastPostThreadName = f['LastPostThreadName'];
      String lastPostAuthor = f['LastPostAuthor'];
      DateTime lastPostDate = f['LastPostDate'];


      SectionViewPiece piece = sectionPieces.firstWhere((SectionViewPiece element) {return element.id == id;}, orElse: () => null);
      if (piece == null) {
        piece = new SectionViewPiece(this, id, forumID, name, description, threadCount, postCount, lastPostThreadName, lastPostAuthor, lastPostDate);
        sectionPieces.add(piece);
        sectionHolder.append(piece.getDiv());
      } else {
        piece.updateDescription(description);
        piece.updateName(name);
        piece.updateLastPostAuthor(lastPostAuthor);
        piece.updateLastPostDate(lastPostDate);
        piece.updateLastPostThreadName(lastPostThreadName);
        piece.updateThreadCount(threadCount);
        piece.updatePostCount(postCount);
      }
    }
  }
  DivElement getView() {
    return sectionHolder;
  }

  void createWebsiteName() {
    websiteNameDiv = new DivElement();
    websiteNameDiv.id = "sectionView";
    websiteNameDiv.className = "websiteNameDiv";
    websiteNameDiv.onClick.listen((_){
      clickedForumName();
    });
    ImageElement folderImage = new ImageElement(
        src: "Resources/GMGFolderIcon.png");
    folderImage.id = "sectionView";
    folderImage.className = "folderImage";
    websiteNameDiv.append(folderImage);
    SpanElement nameText = new SpanElement();
    nameText.text = "Game Maker's Garage";
    websiteNameDiv.append(nameText);
  }

  void createForumName() {
    forumNameDiv = new DivElement();
    forumNameDiv.id = "sectionView";
    forumNameDiv.className = "forumNameDiv";
    ImageElement subFolderImage = new ImageElement(
        src: "Resources/GMGSubFolderIcon.png");
    forumNameDiv.append(subFolderImage);
    subFolderImage.id = "sectionView";
    subFolderImage.className = "subFolderImage";
    ImageElement folderImage = new ImageElement(
        src: "Resources/GMGFolderIcon.png");
    forumNameDiv.append(folderImage);
    folderImage.id = "sectionView";
    folderImage.className = "folderImage";
    forumNameText = new SpanElement();
    forumNameDiv.append(forumNameText);
    //..marginTop = "50px"
    //..borderTopStyle="solid"
    //..borderLeftStyle="solid"
    //..borderRightStyle="solid"
    //..borderColor="black"
    //..borderWidth="1px"
    //..width="calc(100% - 50px)"
    //..marginLeft="25px"
    //..marginRight="25px";
  }

  void createPagesDiv() {
    pagesDiv = new DivElement();
    pagesDiv.id = "sectionView";
    pagesDiv.className = "pagesDiv";

    SpanElement pagesSpan = new SpanElement();
    pagesSpan.text = "Pages: ";
    pagesSpan.id = "sectionView";
    pagesSpan.className = "pagesSpan";

    //Add page numbers
    //Add Start New Topic with image icon

    pagesDiv.append(pagesSpan);

    SpanElement pageNumbers = new SpanElement();
    pageNumbers.text = "1";
    pageNumbers.id = "sectionView";
    pageNumbers.className = "pageNumbers";
    pagesDiv.append(pageNumbers);

    SpanElement newSectionSpan = new SpanElement();
    newSectionSpan.id = "sectionView";
    newSectionSpan.className = "newSectionSpan";
    newSectionSpan.text = "Start new section";
    pagesDiv.append(newSectionSpan);

    ImageElement newSectionIcon = new ImageElement(src:"Resources/GMGNewThreadIcon.png");

    newSectionIcon.id = "sectionView";
    newSectionIcon.className = "newSectionIcon";
    pagesDiv.append(newSectionIcon);
  }
}

DivElement createSectionHolder() {
  DivElement sectionHolder = new DivElement();
  sectionHolder.style
    ..marginTop = "50px"
    ..borderColor="black"
    ..borderWidth="1px"
    ..width="calc(100% - 50px)"
    ..marginLeft="25px"
    ..marginRight="25px";
  return sectionHolder;
}
DivElement createForumDescription() {
  DivElement descriptionDiv = new DivElement();
  descriptionDiv.id = "sectionView";
  descriptionDiv.className = "descriptionDiv";
  return descriptionDiv;
}
DivElement createSectionLabel() {
  //Create green bar of labels
  //23 pixels in height is label
  DivElement sectionLabel = new DivElement();
  sectionLabel.id = "sectionView";
  sectionLabel.className = "sectionLabel";
  LabelElement sectionNameLabel = new LabelElement();
  sectionNameLabel.text = "Sections";
  sectionNameLabel.id = "sectionView";
  sectionNameLabel.className = "sectionNameLabel";
  sectionLabel.append(sectionNameLabel);
  LabelElement sectionThreadsLabel = new LabelElement();
  sectionThreadsLabel.text = "Threads";
  sectionThreadsLabel.id = "sectionView";
  sectionThreadsLabel.className = "sectionThreadsLabel";
  sectionLabel.append(sectionThreadsLabel);
  LabelElement sectionPostsLabel = new LabelElement();
  sectionPostsLabel.text = "Posts";
  sectionPostsLabel.id = "sectionView";
  sectionPostsLabel.className = "sectionPostsLabel";
  sectionLabel.append(sectionPostsLabel);
  LabelElement sectionLastPostLabel = new LabelElement();
  sectionLastPostLabel.text = "Last Post";
  sectionLastPostLabel.id = "sectionView";
  sectionLastPostLabel.className = "sectionLastPostLabel";
  sectionLabel.append(sectionLastPostLabel);

  return sectionLabel;
}