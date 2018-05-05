library GameController;
import 'Tag/TagController.dart';
import 'BlockGame/BlockGameController.dart';
import '../Server.dart';
import 'dart:typed_data';
import '../User.dart';

class GameController {
  Server server;
  TagController tagController;
  BlockGameController blockController;
  GameController(Server s) {
    server = s;
    tagController = new TagController(this);
    blockController = new BlockGameController(this);
  }
  void logic() {
    tagController.logic();
    blockController.logic();
  }
  void onMessage(User p, Map message) {
    switch (message['Game']) {
      case 'Switch To Game':
        {
          disengageFromGame(p);
          engageToGame(p, message['Title']);
        }
        break;
      case 'Data':
        {
          if (p.inGame == tagController.name) {
            tagController.onMessage(p, message);
          }
          if (p.inGame == blockController.name) {
            blockController.onMessage(p, message);
          }
        }
        break;
    }
  }
  void onBinaryMessage(User p, Uint8List message) {
    if (p.inGame == tagController.name) {
      tagController.onBinaryMessage(p, message);
    }
    if (p.inGame == blockController.name) {
      blockController.onBinaryMessage(p, message);
    }
  }
  void disengageFromGame(User u) {
    if (u.inGame == tagController.name) {
      tagController.removeUser(u);
    }
    if (u.inGame == blockController.name) {
      blockController.removeUser(u);
    }
    u.inGame = "";
  }
  void engageToGame(User u, String game) {
    print("Trying to engage to game: ${game}");
    if (game == tagController.name) {
      tagController.addUser(u);
      u.inGame = game;
    }
    if (game == blockController.name) {
      print("Connected to ${blockController.name}");
      blockController.addUser(u);
      u.inGame = game;
    }
  }

  void accountLoggedIn(User u) {
    print("Account logged In but not in game yet");
    if (u.inGame == tagController.name) {
      tagController.accountLoggedIn(u);
    }
    if (u.inGame == blockController.name) {
      blockController.accountLoggedIn(u);
    }
  }
  void accountLoggedOut(User u) {
    if (u.inGame == tagController.name) {
      tagController.accountLoggedOut(u);
    }
    if (u.inGame == blockController.name) {
      blockController.accountLoggedOut(u);
    }
  }
}