library CodingThreadDisplay;
import 'dart:html';
import '../../../Tools/DescriptionParser.dart';
import 'FullView.dart';

class ThreadDisplay {
  DivElement main;
  DivElement mainView, profileSection, titleSection, descriptionSection;
  SpanElement title, owner;
  ImageElement image;
  DivElement description;
  //Edit stuff
  DivElement editView;
  TextInputElement editTitle;
  TextAreaElement editDescription;
  ButtonElement editButton;
  ButtonElement deleteButton;
  ButtonElement switchToEditViewButton;
  FullView controller;
  bool onMainView = true;
  ThreadDisplay(FullView c) {
    controller = c;
    makeMain();
    makeMainView();
    makeTitleComponent();
    makeImageComponent();
    makeDescriptionComponent();
    makeOwnerComponent();
    makeSwitchToEditViewButton();
    //Make the edit stuff
    makeEditView();
    makeEditTitleComponent();
    makeEditDescriptionComponent();
    makeEditButtonComponent();
    makeDeleteButtonComponent();
    main.append(mainView);
  }
  void updateTitle() {
    title.text = controller.thread.getTitle();
    editTitle.value = controller.thread.getTitle();
  }
  void updateOwner() {
    owner.text = controller.thread.getOwner();
    //If owner is self add edit button
    if (controller.thread.getOwner() == controller.threadController.sectionController.website.database.name) {
      descriptionSection.append(switchToEditViewButton);
    }
  }
  void updateAvatarURL() {
    image.src = controller.thread.getAvatarURL();
    }
  void updateDescription() {
    description.children.clear();
    description.append(DescriptionParser.parseDescription(controller.thread.getDescription()));
    editDescription.text = controller.thread.getDescription();
  }
  DivElement getDiv() {
    return main;
  }
  void saveEdit() {
    //Send new post to server
    Map message = new Map();
    message['Controller'] = controller.threadController.sectionController.name;
    message['Title'] = 'Edit Post';
    message['Thread ID'] = controller.thread.getID();
    message['Edit Title'] = editTitle.value;
    message['Text'] = editDescription.value;
    controller.threadController.sectionController.website.networkingController.Send(message);
    switchView();
  }
  void deleteThread() {
    //Send new post to server
    Map message = new Map();
    message['Controller'] = controller.threadController.sectionController.name;
    message['Title'] = 'Delete Post';
    message['Thread ID'] = controller.thread.getID();
    controller.threadController.sectionController.website.networkingController.Send(message);
    controller.threadController.restoreToDefaultState();
  }
  void switchView() {
    onMainView = !onMainView;
    if (onMainView) {
      switchToMainView();
    } else {
      switchToEditView();
    }
  }
  void switchToMainView() {
    editView.remove();
    main.append(mainView);
  }
  void switchToEditView() {
    mainView.remove();
    main.append(editView);
  }
  //Make GUI Components
  void makeMain() {
    main = new DivElement();
  }
  void makeMainView() {
    mainView = new DivElement();
    mainView.style..display = "inline-block"..marginLeft = "75px";
    profileSection = new DivElement();
    profileSection.style
        ..display = "inline-block"
        ..width = "60px"
        ..position = "absolute"
        ..top = "5px"
        ..textAlign = "center"..color = "white";
    mainView.append(profileSection);
    titleSection = new DivElement();
    titleSection.style
        ..display = "inline-block"
        ..width = "calc(100% - 80px)"
        ..minHeight = "80px"
        ..marginLeft = "70px"
        ..textAlign = "left"..color = "white";
    mainView.append(titleSection);
    descriptionSection = new DivElement();
    descriptionSection.style
            ..display = "inline-block"
            ..width = "calc(100% - 90px)"
            ..marginLeft = "70px"
            ..textAlign = "left"..color = "white";
        mainView.append(descriptionSection);

        mainView.append(new HRElement());
  }
  void makeTitleComponent() {
    title = new SpanElement();
    title.style
        ..fontSize = "50px"
        ..textDecoration = "underline"
        ..paddingTop = "10px"
        ..color = "white";
    titleSection.append(title);
  }
  void makeOwnerComponent() {
    owner = new SpanElement();
    owner.style
        ..fontSize = "15px"
        ..color = "white";
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
        ..wordWrap = "normal"
        //..whiteSpace = "pre"
        ..width = "90%"
        ..color = "white";
    descriptionSection.append(description);
  }
  void makeSwitchToEditViewButton() {
    switchToEditViewButton = new ButtonElement();
    switchToEditViewButton.style
        //..padding = "10px"
        ..color = "black"
        ..float = "right";
    //..width = "100px";
    switchToEditViewButton.text = "Edit";
    switchToEditViewButton.onClick.listen((_) {
      switchView();
    });
  }
  //Edit stuff
  void makeEditView() {
    editView = new DivElement();
    editView.style
        ..margin = "30px"
        ..marginLeft = "70px";
  }
  void makeEditTitleComponent() {
    editTitle = new TextInputElement();
    editTitle.style
        //..fontSize = "20px"
        ..padding = "10px"
        ..color = "black"
        ..width = "calc(100% - 20px)";
    editTitle.placeholder = "Title here";
    editView.append(editTitle);
  }
  void makeEditDescriptionComponent() {
    editDescription = new TextAreaElement();
    editDescription.style
        //..fontSize = "20px"
        ..color = "black"
        ..width = "100%"
        ..height = "100px";
    editDescription.placeholder = "Description here";
    editView.append(editDescription);
  }
  void makeEditButtonComponent() {
    editButton = new ButtonElement();
    editButton.style
        //..fontSize = "20px"
        //..padding = "10px"
        ..color = "black"
        ..float = "right";
    editButton.text = "Save Edit";
    editView.append(editButton);
    editButton.onClick.listen((_) {
      saveEdit();
    });
  }
  void makeDeleteButtonComponent() {
    deleteButton = new ButtonElement();
    deleteButton.style
        //..fontSize = "20px"
        //..padding = "10px"
        ..color = "black"
        ..float = "left";
    deleteButton.text = "Delete Thread";
    editView.append(deleteButton);
    deleteButton.onClick.listen((_) {
      deleteThread();
    });
  }
}