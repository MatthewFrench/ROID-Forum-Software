library GraphicsThreadFullView;
import 'dart:html';
import '../ThreadInfo.dart';
import '../ThreadController.dart';
import '../CommentInfo.dart';
import 'CommentView.dart';
import 'ThreadDisplay.dart';

class FullView {
  ThreadInfo thread;
  ThreadController threadController;

  ThreadDisplay threadDisplay;
  DivElement main;
  TextAreaElement commentBox;
  ButtonElement commentButton;
  DivElement commentButtonAndBox;
  DivElement commentContainer;
  List<CommentView> commentViews;
  FullView(ThreadInfo t, ThreadController tc) {
    threadController = tc;
    thread = t;
    threadDisplay = new ThreadDisplay(this);

    makeMainComponent();
    makeComments();
    makeCommentBoxComponent();
    makeCommentButtonComponent();
    
    commentViews = new List();
  }
  void sendComment() {
    String text = commentBox.value;
    Map m = new Map();
    m['Controller'] = threadController.sectionController.name;
    m['Title'] = 'Add Comment';
    m['ID'] = thread.getID();
    m['Text'] = text;
    threadController.sectionController.website.networkingController.Send(m);
    commentBox.value = "";
  }
  DivElement getDiv() {
    return main;
  }
  void addComment(CommentInfo ci) {
    commentViews.add(ci.commentView);
    commentContainer.append(ci.commentView.getDiv());
    //ci.commentView.getDiv().style.opacity = "0.01";
    //animate(ci.commentView.getDiv(), duration: 250, properties: {'opacity': 1.0});
  }
  void removeComment(CommentInfo ci) {
      commentViews.remove(ci.commentView);
      ci.commentView.getDiv().remove();
    }
  //GUI Components
  void makeMainComponent() {
    main = new DivElement();
    main.append(threadDisplay.getDiv());
  }
  void makeComments() {
    commentContainer = new DivElement();
    commentContainer.style
        //..float = 'left'
        //..width = 'calc(100% - 100px)'
        //..marginLeft = '70px'
        //..marginTop = '10px'
        ..color = "white";
    main.append(commentContainer);
  }
  void makeCommentBoxComponent() {
    commentButtonAndBox = new DivElement();
    if (threadController.sectionController.website.database.loggedIn) {
      main.append(commentButtonAndBox);
    }
    
    commentBox = new TextAreaElement();
    commentBox.placeholder = "Comment...";
    commentBox.style
        ..float = 'left'
        ..width = 'calc(100% - 100px)'
        ..height = '200px'
        ..marginLeft = '70px'
        ..marginTop = '10px'
        ..resize = 'none';
    commentButtonAndBox.append(commentBox);
  }
  void makeCommentButtonComponent() {
    commentButton = new ButtonElement();
    commentButton.text = "Comment";
    commentButton.style
        ..float = "left"
        ..marginLeft = '70px'
        ..width = "100px";
    commentButtonAndBox.append(commentButton);
    commentButton.onClick.listen((_) {
      sendComment();
    });
  }
}