library GraphicsCommentView;
import '../ThreadInfo.dart';
import '../ThreadController.dart';
import '../CommentInfo.dart';
import 'dart:html';
import '../../../Tools/DescriptionParser.dart';

class CommentView {
  ThreadInfo thread;
  CommentInfo comment;
  ThreadController threadController;
  DivElement main, profileSection, descriptionSection, description;
  ImageElement image;
  TextAreaElement editDescription;
  ButtonElement editComment, saveEdit, deleteComment;
  SpanElement owner;
  CommentView(CommentInfo ci, ThreadInfo t, ThreadController tc) {
    threadController = tc;
    thread = t;
    comment = ci;
    makeMainComponent();
    makeImageComponent();
    makeOwnerComponent();
    makeDescriptionComponent();
    makeEditDescriptionComponent();
    makeEditCommentComponent();
    makeSaveEditComponent();
    makeDeleteCommentComponent();
  }
  void editButtonClick() {
    editComment.remove();
    descriptionSection.append(saveEdit);
    descriptionSection.append(deleteComment);
    descriptionSection.insertBefore(editDescription, description);
    description.remove();
    editDescription.value = comment.getComment();
  }
  void saveEditButtonClick() {
    descriptionSection.append(editComment);
    saveEdit.remove();
    deleteComment.remove();
    descriptionSection.insertBefore(description, editDescription);
    editDescription.remove();
    Map message = new Map();
    message['Controller'] = threadController.sectionController.name;
    message['Title'] = 'Edit Comment';
    message['Thread ID'] = thread.getID();
    message['Comment ID'] = comment.getCommentID();
    message['Text'] = editDescription.value;
    threadController.sectionController.website.networkingController.Send(message);
  }
  void deleteCommentButtonClick() {
    Map message = new Map();
    message['Controller'] = threadController.sectionController.name;
    message['Title'] = 'Delete Comment';
    message['Thread ID'] = thread.getID();
    message['Comment ID'] = comment.getCommentID();
    threadController.sectionController.website.networkingController.Send(message);
  }
  DivElement getDiv() {
    return main;
  }
  void updateOwner() {
    owner.text = comment.getPosterName();
    //If owner is self add edit button
    if (comment.getPosterName() == threadController.sectionController.website.database.name) {
      descriptionSection.append(editComment);
    }
  }
  void updateAvatarURL() {
      image.src = comment.getAvatarURL();
    }
  void updateDescription() {
    description.children.clear();
    description.append(DescriptionParser.parseDescription(comment.getComment()));
  }
  void makeMainComponent() {
    main = new DivElement();
    main.style..display = "inline-block"..marginLeft = "75px"..position="relative";
    main.style
        ..width = "calc(100% - 75px)"
        ..opacity = "1.0";

    profileSection = new DivElement();
    profileSection.style
        ..display = "inline-block"
        ..width = "60px"
        ..height = "80px"
        ..position = "absolute"
                ..top = "5px"
        ..textAlign = "center";
    main.append(profileSection);
    descriptionSection = new DivElement();
    descriptionSection.style
        ..display = "inline-block"
        ..width = "calc(100% - 95px)"
        ..marginLeft = "70px"
        ..marginTop = "5px"
        ..minHeight = "90px"
        ..textAlign = "left";
    main.append(descriptionSection);
    main.append(new HRElement());
  }
  void makeOwnerComponent() {
    owner = new SpanElement();
    owner.style
        ..fontSize = "15px"
        ..color = "black";
    profileSection.append(owner);
  }
  void makeImageComponent() {
    image = new ImageElement();
    image.style
        ..height = "60px"
        ..width = "60px"
        ..backgroundColor = "rgb(200,200,200)"
        ..borderStyle = "solid"
        ..borderWidth = "1px";
    profileSection.append(image);
  }
  void makeDescriptionComponent() {
    description = new DivElement();
    description.style
        ..width = "90%"
        ..wordWrap = "normal"
        ..color = "black";
    descriptionSection.append(description);
  }
  void makeEditDescriptionComponent() {
      editDescription = new TextAreaElement();
      editDescription.style
          ..width = "90%"
          ..wordWrap = "normal"
          ..color = "black";
    }
  void makeEditCommentComponent() {
    editComment = new ButtonElement();
    editComment.style.float = "right";
    editComment.text = "Edit";
    editComment.onClick.listen((_) {
      editButtonClick();
    });
  }
  void makeSaveEditComponent() {
    saveEdit = new ButtonElement();
    saveEdit.style.float = "right";
    saveEdit.text = "Save";
    saveEdit.onClick.listen((_) {
      saveEditButtonClick();
    });
  }
  void makeDeleteCommentComponent() {
    deleteComment = new ButtonElement();
    deleteComment.style.float = "left";
    deleteComment.text = "Delete";
    deleteComment.onClick.listen((_) {
      deleteCommentButtonClick();
    });
  }
}
