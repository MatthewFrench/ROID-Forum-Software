library OtherNewPostWindow;
import 'dart:html';
import 'OtherSection.dart';

class NewPostWindow {
  OtherSection sectionController;
  DivElement backgroundBlockDiv;
  DivElement mainWindowDiv;
  InputElement titleInput;
  TextAreaElement mainText;
  NewPostWindow(OtherSection cs) {
    sectionController = cs;

    makeWindowComponent();
    makeTitleComponent();
    makeTitleInputComponent();
    makeDescriptionComponent();
    makePostButtonComponent();
    makeBackgroundBlockComponent();
  }
  void show() {
    sectionController.website.aheadWebsiteDiv.append(backgroundBlockDiv);
    sectionController.website.aheadWebsiteDiv.append(mainWindowDiv);
  }
  void hide() {
    backgroundBlockDiv.remove();
    mainWindowDiv.remove();
  }
  void reset() {
    titleInput.value = '';
    mainText.value = '';
  }
  void postButtonClick() {
    //Send new post to server
    Map message = new Map();
    message['Controller'] = sectionController.name;
    message['Title'] = 'New Post';
    message['Post Title'] = titleInput.value;
    message['Post Description'] = mainText.value;
    sectionController.website.networkingController.Send(message);
    //Hide the window
    hide();
    //Reset window
    reset();
  }
  /*****GUI COMPONENT CREATION********/
  void makeWindowComponent() {
    mainWindowDiv = new DivElement();
    mainWindowDiv.style
        ..position = "absolute"
        ..width = "400px"
        ..height = "300px"
        ..borderRadius = "4px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..backgroundColor = "white"
        ..top = "calc(50% - 150px)"
        ..left = "calc(50% - 200px)";
  }
  void makeBackgroundBlockComponent() {
    backgroundBlockDiv = new DivElement();
    backgroundBlockDiv.style
        ..position = "absolute"
        ..width = "100%"
        ..height = "100%"
        ..backgroundColor = "rgba(0, 0, 0, 0.5)"
        ..top = "0px"
        ..left = "0px";
    backgroundBlockDiv.onClick.listen((var d) {
      hide();
    });
  }
  void makeTitleComponent() {
    DivElement title = new DivElement();
    title.text = "Create New Post";
    title.style
        ..position = 'absolute'
        ..top = '10px'
        ..left = '0px'
        ..width = '100%'
        ..textAlign = 'center'
        ..fontSize = '25px';
    mainWindowDiv.append(title);
  }
  void makeTitleInputComponent() {
    titleInput = new InputElement();
    titleInput.placeholder = "Post Title";
    titleInput.style
        ..position = 'absolute'
        ..top = '40px'
        ..left = '10px'
        ..width = 'calc(100% - 20px)';
    mainWindowDiv.append(titleInput);
  }
  void makeDescriptionComponent() {
    mainText = new TextAreaElement();
    mainText.placeholder = "Write description here";
    mainText.style
    ..position = 'absolute'
    ..top = '65px'
    ..left = '10px'
    ..width = 'calc(100% - 20px)'
    ..height = 'calc(100% - 125px)'
    ..resize = 'none';
    mainWindowDiv.append(mainText);
  }
  void makePostButtonComponent() {
    ButtonElement postButton = new ButtonElement();
    postButton.text = "Post";
    postButton.style
        ..width = "140px"
        ..height = "40px"
        ..backgroundColor = "white"
        ..borderRadius = "4px"
        ..borderStyle = "solid"
        ..borderWidth = "1px"
        ..borderColor = "#DDDDDD"
        ..fontSize = "15px"
        ..bottom = "5px"
        ..position = "absolute"
        ..outline = "none"
        ..right = "5px";
    postButton.dataset['active'] = "background: #BBBBBB;";
    postButton.onClick.listen((var d) {
      postButtonClick();
    });
    mainWindowDiv.append(postButton);
  }
}
