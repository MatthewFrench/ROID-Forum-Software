library Website;
import 'Networking/NetworkingController.dart';
import 'Control Panel/ControlPanel.dart';
import 'Database.dart';
import 'dart:core';
import 'dart:html';
import 'Gui/HeaderGui.dart';
import 'Gui/ContentView.dart';
import 'WebsiteMsgHandler.dart';
import 'Forum/ForumController.dart';

class Website {
  NetworkingController networkingController;
  ControlPanel controlPanel;
  Database database;
  DivElement websiteDiv, behindWebsiteDiv, aheadWebsiteDiv;
  BodyElement body;
  DivElement reconnectingDiv;
  HeaderGui headerGui;
  ContentView contentView;
  WebsiteMsgHandler websiteMsgHandler;
  ForumController forumController;

  Website() {
    websiteMsgHandler = new WebsiteMsgHandler(this);
    networkingController = new NetworkingController(this);

    websiteDiv = querySelector("#Website");
    behindWebsiteDiv = querySelector("#behindWebsite");
    aheadWebsiteDiv = querySelector("#aheadWebsite");
    behindWebsiteDiv.id = "behindWebsiteDiv";
    aheadWebsiteDiv.id = "aheadWebsiteDiv";
    body = querySelector("body");
    database = new Database(this);
    headerGui = new HeaderGui(this);
    contentView = new ContentView(this);
    headerGui.addHeader();
    controlPanel = new ControlPanel(this);
    forumController = new ForumController(this);
    websiteDiv.append(contentView.getDivElement());
  }
  void connectedToServer() {
    Storage localStorage = window.localStorage;
    if (localStorage['Username'] != null) {
      controlPanel.loginAlreadyEncrypted(localStorage['Username'], localStorage['Password']);
    }
    forumController.connected();
  }
}