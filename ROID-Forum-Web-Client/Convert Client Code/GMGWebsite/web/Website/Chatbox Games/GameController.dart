library GameController;
import '../Website.dart';
import 'dart:typed_data';
import 'BlockGame/BlockGameController.dart';
import 'AbstractGame.dart';
import 'dart:html';

class GameController {
  Website website;
  AbstractGame currentGame;
  DivElement gameDiv;
  GameController(Website w) {
    website = w;
    currentGame = null;
    gameDiv = new DivElement();
    gameDiv.style..width="100%"..height="100%"
        ..position = "absolute"..left = "0px"..top = "0px";
  }
  void connectedToServer() { //Switch to the tag game
    switchToGame(new BlockGameController(this));
  }
  void switchToGame(AbstractGame game) {
    Map m = new Map();
    m['Controller'] = 'Game';
    m['Game'] = 'Switch To Game';
    m['Title'] = game.getName();
    website.networkingController.Send(m);
    if (currentGame != null) {
      currentGame.hide();
      currentGame.getCanvas().remove();
      currentGame = null;
    }
    currentGame = game;
    currentGame.show();
    gameDiv.append(currentGame.getCanvas());
  }
  void logic() {
    if (currentGame != null) {
      currentGame.logic();
    }
  }
  void onMessage(Map message) {
    if (currentGame != null) {
      currentGame.onMessage(message);
    }
  }
  void onBinaryMessage(ByteData buffer) {
    if (currentGame != null) {
      currentGame.onBinaryMessage(buffer);
    }
  }
  DivElement getDiv() {
    return gameDiv;
  }
}
