library Server;
import 'Account Controller/AccountController.dart';
import 'LoginController.dart';
import 'Chat Box/ChatController.dart';
import 'NetworkingController.dart';
import 'Sections/Coding Section Controller/CodingSectionController.dart';
import 'Sections/All Section Controller/AllSectionController.dart';
import 'Sections/Game Section Controller/GameSectionController.dart';
import 'Sections/Graphics Section Controller/GraphicsSectionController.dart';
import 'Sections/Other Section Controller/OtherSectionController.dart';
import 'User.dart';
import 'dart:core';
import 'dart:async';
import 'dart:typed_data';
import 'dart:convert';
import 'Chatbox Games/GameController.dart';

class Server {
  AccountController accountController;
  LoginController loginController;
  ChatController chatController;
  NetworkingController networkingController;
  Timer logicTimer;
  GameController gameController;
  CodingSectionController codingSection;
  AllSectionController allSection;
  GameSectionController gameSection;
  GraphicsSectionController graphicsSection;
  OtherSectionController otherSection;
  Server() {
    accountController = new AccountController(this);
    loginController = new LoginController(this);
    chatController = new ChatController(this);
    networkingController = new NetworkingController(this);
    logicTimer = new Timer.periodic(new Duration(milliseconds: 8), logic); //120 fps logic timer
    codingSection = new CodingSectionController(this);
    allSection = new AllSectionController(this);
    gameSection = new GameSectionController(this);
    graphicsSection = new GraphicsSectionController(this);
    otherSection = new OtherSectionController(this);
    gameController = new GameController(this);
  }
  void logic(Timer t) {
    gameController.logic();

    chatController.logic();
    accountController.logic();

    allSection.logic();
    codingSection.logic();
    gameSection.logic();
    graphicsSection.logic();
    otherSection.logic();
  }
  void accountLoggedIn(User u) {
    //print("Account logged in ${u.account.name}");
    chatController.sendListUpdateToAll();
    gameController.accountLoggedIn(u);
  }
  void accountLoggedOut(User u) {
    //print("Account logged out ${u.account.name}");
    chatController.sendListUpdateToAll();
    gameController.accountLoggedOut(u);
  }
  void onOpen(User u) {
    chatController.addUser(u);
  }
  void onMessage(User u, Object message) {
    if (message is String) {
      Map m = JSON.decode(message);
      if (m['Controller'] == 'Chat') {
        chatController.onMessage(u, m);
      }
      if (m['Controller'] == 'Login') {
        loginController.onMessage(u, m);
      }
      if (m['Controller'] == 'Game') {
        gameController.onMessage(u, m);
      }
      if (m['Controller'] == codingSection.name) codingSection.onMessage(u, m);
      if (m['Controller'] == allSection.name) allSection.onMessage(u, m);
      if (m['Controller'] == graphicsSection.name) graphicsSection.onMessage(u, m);
      if (m['Controller'] == gameSection.name) gameSection.onMessage(u, m);
      if (m['Controller'] == otherSection.name) otherSection.onMessage(u, m);
      if (m['Controller'] == 'Server' && m['Title'] == 'Viewing') {
        disengageFromSection(u.viewingSection, u);
        engageToSection(m['Section'], u);
      }
    } else if (message is Uint8List) {
      gameController.onBinaryMessage(u, message);
    }
  }
  void onClose(User u) {
    disengageFromSection(u.viewingSection, u);
    chatController.removeUser(u);
    gameController.disengageFromGame(u);
  }
  void engageToSection(String section, User u) {
    u.viewingSection = section;
    switch (section) {
      case "Coding Section":
        {
          codingSection.addUser(u);
        }
        break;
      case "All Section":
        {
          allSection.addUser(u);
        }
        break;
      case "Other Section":
        {
          otherSection.addUser(u);
        }
        break;
      case "Graphics Section":
        {
          graphicsSection.addUser(u);
        }
        break;
      case "Game Section":
        {
          gameSection.addUser(u);
        }
        break;
    }
  }
  void disengageFromSection(String section, User u) {
    switch (section) {
      case "All Section":
        {
          allSection.removeUser(u);
        }
        break;
      case "Coding Section":
        {
          codingSection.removeUser(u);
        }
        break;
      case "Game Section":
        {
          gameSection.removeUser(u);
        }
        break;
      case "Graphics Section":
        {
          graphicsSection.removeUser(u);
        }
        break;
      case "Other Section":
        {
          otherSection.removeUser(u);
        }
        break;
    }
    u.viewingSection = "";
  }
}
