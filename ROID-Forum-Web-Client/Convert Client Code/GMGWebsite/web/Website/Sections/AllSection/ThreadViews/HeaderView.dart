library AllThreadHeaderView;
import 'dart:html';
import '../ThreadInfo.dart';
import '../ThreadController.dart';

class HeaderView {
  DivElement headerDiv;
  SpanElement threadHeaderTitle, threadHeaderOwner, threadHeaderSection, threadHeaderComment;
  ImageElement threadHeaderImage;
  ThreadInfo thread;
  ThreadController threadController;

  /**********Make GUI Components***********/
  HeaderView(ThreadInfo t, ThreadController tc) {
    thread = t;
    threadController = tc;
    makeThreadHeaderDivComponent();
    makeThreadHeaderTitleComponent();
    makeThreadHeaderOwnerComponent();
    makeThreadHeaderSectionComponent();
    makeThreadHeaderCommentComponent();
    makeThreadHeaderImageComponent();
  }
  DivElement getDiv() {
    return headerDiv;
  }
  void onClick() {
    threadController.threadClicked(thread);
  }
  void updateTitle() {
    threadHeaderTitle.text = thread.getTitle();
  }
  void updateOwner() {
    threadHeaderOwner.text = thread.getOwner();
  }
  void updateAvatarURL() {
    threadHeaderImage.src = thread.getAvatarURL();
  }
  void updateCommentCount() {
    threadHeaderComment.text = "Comments: ${thread.getCommentCount()}";
  }
  void updateSection() {
    threadHeaderSection.text = thread.getSection();
  }
  void makeThreadHeaderDivComponent() {
    headerDiv = new DivElement();
    headerDiv.style
        ..position = "absolute"
        ..width = "calc(100% - 20px)"
        ..top = "0px"
        ..left = "10px"
        ..height = "80px"
        ..borderTopStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#AAAAAA";
  }
  void makeThreadHeaderTitleComponent() {
    threadHeaderTitle = new SpanElement();
    threadHeaderTitle.style
        ..position = "absolute"
        ..fontSize = "30px"
        ..left = "70px"
        ..top = "10px"
        ..textDecoration = "underline"
        ..textOverflow = "ellipsis"
        ..maxWidth = "calc(100% - 70px)"
        ..height = "30px"
        ..cursor = "pointer"
        ..whiteSpace = "nowrap"
        ..overflow = "hidden"
        ..paddingTop = "10px";
    headerDiv.append(threadHeaderTitle);
    threadHeaderTitle.onClick.listen((var d) {
      onClick();
    });
  }
  void makeThreadHeaderOwnerComponent() {
    threadHeaderOwner = new SpanElement();
    threadHeaderOwner.style
        ..position = "absolute"
        ..fontSize = "15px"
        ..right = "10px"
        ..bottom = "10px"
        ..cursor = "pointer";
    headerDiv.append(threadHeaderOwner);
  }
  void makeThreadHeaderSectionComponent() {
      threadHeaderSection = new SpanElement();
      threadHeaderSection.style
          ..position = "absolute"
          ..width = "140px"
          ..fontSize = "15px"
          ..right = "calc(50% - 70px)"
          ..textAlign = "center"
          ..bottom = "10px"
          ..cursor = "pointer";
      headerDiv.append(threadHeaderSection);
    }
  void makeThreadHeaderCommentComponent() {
    threadHeaderComment = new SpanElement();
    threadHeaderComment.style
        ..position = "absolute"
        ..fontSize = "15px"
        ..left = "70px"
        ..bottom = "10px"
        ..cursor = "pointer";
    headerDiv.append(threadHeaderComment);
    threadHeaderComment.onClick.listen((var d) {
      onClick();
    });
  }
  void makeThreadHeaderImageComponent() {
    threadHeaderImage = new ImageElement();
    threadHeaderImage.style
        ..position = "absolute"
        ..left = "0px"
        ..top = "10px"
        ..height = "60px"
        ..width = "60px"
        ..backgroundColor = "rgb(200,200,200)"
        ..borderStyle = "solid"
        ..borderWidth = "1px";
    headerDiv.append(threadHeaderImage);
  }
}
