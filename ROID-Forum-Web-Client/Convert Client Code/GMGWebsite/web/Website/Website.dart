library Website;
import 'NetworkingController.dart';
import 'ControlPanel.dart';
import 'Database.dart';
import 'Chatbox.dart';
import 'MainTopBarSection.dart';
import 'Sections/AllSection/AllSection.dart';
import 'Sections/CodingSection/CodingSection.dart';
import 'Sections/GameSection/GameSection.dart';
import 'Sections/GraphicsSection/GraphicsSection.dart';
import 'Sections/OtherSection/OtherSection.dart';
import 'Sections/GenericSection.dart';
import 'dart:core';
import 'dart:async';
import 'dart:typed_data';
import 'dart:html';
import 'package:animation/animation.dart';
import 'Chatbox Games/GameController.dart';

class Website {
  NetworkingController networkingController;
  ControlPanel controlPanel;
  Timer logicTimer;
  Chatbox chatbox;
  MainSection mainSection;
  Database database;
  AllSection allSection;
  CodingSection codingSection;
  GameSection gameSection;
  GraphicsSection graphicsSection;
  OtherSection otherSection;
  List<GenericSection> sectionOrder;
  GenericSection showingSection = null;
  DivElement websiteDiv, behindWebsiteDiv, aheadWebsiteDiv;
  BodyElement body;
  DivElement reconnectingDiv;
  GameController gameController;

  Website() {
    websiteDiv = querySelector("#Website");
    behindWebsiteDiv = querySelector("#behindWebsite");
    aheadWebsiteDiv = querySelector("#aheadWebsite");
    websiteDiv.style
        ..width = "100%"
        ..height = "100%"
        ..overflow = "scroll";
    body = querySelector("body");
    logicTimer = new Timer.periodic(new Duration(milliseconds: 16), logic);
    setUp();
  }
  void setUp() {
    networkingController = new NetworkingController(this);
    database = new Database(this);
    controlPanel = new ControlPanel(this);
    //Put the chat on the right side
    chatbox = new Chatbox(this);
    //Lets make the visual stuff
    allSection = new AllSection(this);
    codingSection = new CodingSection(this);
    gameSection = new GameSection(this);
    graphicsSection = new GraphicsSection(this);
    otherSection = new OtherSection(this);
    sectionOrder = [allSection, codingSection, gameSection, graphicsSection, otherSection];
    //Now add the main parts of the site
    mainSection = new MainSection(this);
    gameController = new GameController(this);
    behindWebsiteDiv.append(gameController.getDiv());
  }
  void logic(Timer t) {
    mainSection.logic();
    codingSection.logic();
    gameController.logic();
  }
  void prepareReset() {
    behindWebsiteDiv.children.clear();
    websiteDiv.children.clear();
    aheadWebsiteDiv.children.clear();
    behindWebsiteDiv.hidden = true;
    websiteDiv.hidden = true;
    aheadWebsiteDiv.hidden = true;
    if (reconnectingDiv != null) {
      reconnectingDiv.remove();
      reconnectingDiv = null;
    }
    reconnectingDiv = new DivElement();
    reconnectingDiv.style
        ..width = "100%"
        ..height = "100%"
        ..textAlign = "Center"
        ..fontSize = "40px"
        ..background = "white";
    reconnectingDiv.text = "Unable to connect, switching to backup...";
    body.append(reconnectingDiv);
  }
  void reset() {
    setUp();
    //print("Trying to reconnect");
    //networkingController = new NetworkingController(this);
  }

  void connectedToServer() {
    behindWebsiteDiv.hidden = false;
    websiteDiv.hidden = false;
    aheadWebsiteDiv.hidden = false;
    if (reconnectingDiv != null) {
      reconnectingDiv.remove();
      reconnectingDiv = null;
    }
    //Try auto-login
    Storage localStorage = window.localStorage;
    if (localStorage['Name'] != null) {
      controlPanel.loginAlreadyEncrypted(localStorage['Name'], localStorage['Password']);
    }
    //Load the All page
    mainSection.sectionClick(0);
    gameController.connectedToServer();
  }

  void showView(int section) {
    if (showingSection != null) {
      showingSection.getDiv().style.opacity = "1.0";
      animate(showingSection.getDiv(), duration: 250, properties: {
        'opacity': 0.0
      }).onComplete.listen((_) {
        showingSection.getDiv().remove();
        showingSection.hide();
        showingSection = sectionOrder[section];
        showingSection.getDiv().style.opacity = "0.0";
        websiteDiv.append(showingSection.getDiv());
        showingSection.show();
        animate(showingSection.getDiv(), duration: 250, properties: {
          'opacity': 1.0
        });
      });
    } else {
      showingSection = sectionOrder[section];
      showingSection.getDiv().style.opacity = "0.0";
      websiteDiv.append(showingSection.getDiv());
      showingSection.show();
      animate(showingSection.getDiv(), duration: 250, properties: {
        'opacity': 1.0
      });
    }
  }

  void goTo(String section, int threadID) {
    switch (section) {
      case "Coding Section":
        {
          mainSection.sectionClick(sectionOrder.indexOf(codingSection));
          codingSection.showThread(threadID);
        }
        break;
      case "Game Section":
        {
          mainSection.sectionClick(sectionOrder.indexOf(gameSection));
          gameSection.showThread(threadID);
        }
        break;
      case "Graphics Section":
        {
          mainSection.sectionClick(sectionOrder.indexOf(graphicsSection));
          graphicsSection.showThread(threadID);
        }
        break;
      case "Other Section":
        {
          mainSection.sectionClick(sectionOrder.indexOf(otherSection));
          otherSection.showThread(threadID);
        }
        break;
    }
  }

  void Message(Map message) {
    if (message['Controller'] == 'Game') {
      gameController.onMessage(message);
    }
    if (message['Controller'] == 'Chat') {
      chatbox.onMessage(message);
    }
    if (message['Controller'] == 'Login') {
      switch (message['Title']) {
        case 'Logged In':
          {
            database.processEvent('Logged In', message);
            controlPanel.processEvent('Logged In');
            chatbox.processEvent('Logged In');
            for (GenericSection section in sectionOrder) section.processEvent('Logged In');
          }
          break;
        case 'Logged Out':
          {
            database.processEvent('Logged Out');
            controlPanel.processEvent('Logged Out');
            chatbox.processEvent('Logged Out');
            for (GenericSection section in sectionOrder) section.processEvent('Logged Out');
          }
          break;
        case 'Register Failed':
          {
            controlPanel.processEvent('Register Failed');
          }
          break;
        case 'Login Failed':
          {
            controlPanel.processEvent('Login Failed');
          } break;
        case 'Get Avatar': {
          controlPanel.preferencesAvatarInput.value = message['AvatarURL'];
        }break;
        default: {
          print("Got other message: ${message['Title']}");
        }
      }
    }
    if (message['Controller'] == codingSection.name) {
      codingSection.onMessage(message);
    }
    if (message['Controller'] == allSection.name) {
      allSection.onMessage(message);
    }
    if (message['Controller'] == otherSection.name) {
      otherSection.onMessage(message);
    }
    if (message['Controller'] == graphicsSection.name) {
      graphicsSection.onMessage(message);
    }
    if (message['Controller'] == gameSection.name) {
      gameSection.onMessage(message);
    }
  }

  void BinaryMessage(ByteData buffer) {
    gameController.onBinaryMessage(buffer);
  }
}
