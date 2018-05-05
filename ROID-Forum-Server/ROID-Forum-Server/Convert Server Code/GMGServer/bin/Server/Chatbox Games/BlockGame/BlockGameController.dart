library BlockGameController;
import '../../User.dart';
import '../GameController.dart';
import 'MessageSender.dart';
import 'dart:typed_data';
import 'BlockController.dart';
import 'EntityController.dart';
import 'Entities/Player.dart';
import 'BlockGameIOController.dart';

class BlockGameController {
  GameController controller;
  String name = 'BlockGame';
  List<User> usersViewing = new List();
  MessageSender messageSender;
  EntityController entityController;
  BlockController blockController;
  BlockGameIOController blockGameIOController;
  BlockGameController(GameController c) {
    controller = c;
    entityController = new EntityController(this);
    blockController = new BlockController(this);
    messageSender = new MessageSender(this);
    blockGameIOController = new BlockGameIOController(this);
    blockGameIOController.loadAllBlocks();
  }
  void logic() {
    entityController.movePlayers();
    blockGameIOController.logic();
  }
  void addUser(User u) {
    usersViewing.add(u);
    messageSender.sendAllBlocksTo(u);
    messageSender.sendAllPlayersTo(u);
    if (u.account != null) {
      accountLoggedIn(u);
    }
  }
  void removeUser(User u) {
    usersViewing.remove(u);
    if (u.account != null) {
      accountLoggedOut(u);
    }
  }
  void accountLoggedIn(User u) {
    entityController.addPlayer(u);
    print("User logged into block game");
  }
  void accountLoggedOut(User u) {
    entityController.removePlayer(u);
  }
  void onMessage(User u, Map message) {
    Player p = entityController.getPlayer(u);
    if (p == null) return;
    switch (message['Title']) {
      case 'Set Block':
        {
          print("Got color: ${message['Block Color']} for ${message['Block X']}, ${message['Block Y']}");
          int color = message['Block Color'];
          blockController.setBlockColor(message['Block X'], message['Block Y'], color);
        }
        break;
      case 'Erase Block':
        {
          blockController.removeBlock(message['Block ID']);
        }
        break;
    }
  }
  void onBinaryMessage(User u, Uint8List message) {
    Player p = entityController.getPlayer(u);
    if (p == null) return;
    int title = message[0];
    switch (title) {
      case 0:
        {
          p.leftPressed = true;
        }
        break;
      case 1:
        {
          p.rightPressed = true;
        }
        break;
      case 2:
        {
          p.upPressed = true;
        }
        break;
      case 3:
        {
          p.downPressed = true;
        }
        break;
      case 4:
        {
          p.leftPressed = false;
        }
        break;
      case 5:
        {
          p.rightPressed = false;
        }
        break;
      case 6:
        {
          p.downPressed = false;
        }
        break;
      case 7:
        {
          p.upPressed = false;
        }
        break;
    }
  }
}
