library BlockGameController;
import '../AbstractGame.dart';
import 'dart:html';
import '../GameController.dart';
import 'dart:typed_data';
import 'MessageSender.dart';
import 'DrawController.dart';
import 'GuiController.dart';
import 'LogicController.dart';
import 'EntityController.dart';
import 'BlockController.dart';

class BlockGameController extends AbstractGame {
  String name = 'BlockGame';
  GameController controller;
  MessageSender messageSender;
  GuiController guiController;
  DrawController drawController;
  EntityController entityController;
  BlockController blockController;
  LogicController logicController;
  BlockGameController(GameController c) {
    controller = c;
    messageSender = new MessageSender(this);
    guiController = new GuiController(this);
    drawController = new DrawController(this);
    entityController = new EntityController(this);
    blockController = new BlockController(this);
    logicController = new LogicController(this);
    guiController.createGuiElements();
  }
  void logic() {
    logicController.logic();
  }
  void onMessage(Map m) {
    switch (m['Title']) {
      case 'Update Block Color':
        {
          int id = m['ID'];
          int color = m['Color'];
          blockController.updateBlockColor(id, color);
        }
        break;
      case 'Add Block':
        {
          int id = m['ID'];
          double x = m['X'];
          double y = m['Y'];
          int color = m['Color'];
          blockController.addBlock(id, x, y, color);
        }
        break;
      case 'Remove Block':
        {
          int id = m['ID'];
          blockController.removeBlock(id);
        }
        break;
      case 'Self ID':
        {
          int id = m['ID'];
          entityController.setMainPlayer(id);
          print("Got self ID: ${id}");
        }
        break;
      case 'Add Player':
        {
          int id = m['ID'];
          String name = m['Name'];
          double x = m['X'];
          double y = m['Y'];
          entityController.addPlayer(id, name, x, y);
          print("Got Add Player: ${id}");
        }
        break;
      case 'Remove Player':
        {
          int id = m['ID'];
          entityController.removePlayer(id);
        }
        break;
    }
  }
  void onBinaryMessage(ByteData buffer) {
    int binaryMsg = buffer.getUint8(0);
    switch (binaryMsg) {
      case 1:
        { // Position Update
          int id = buffer.getUint32(1);
          double x = buffer.getFloat64(5);
          double y = buffer.getFloat64(13);
          entityController.updatePlayer(id, x, y);
        }
        break;
    }
  }
  void show() {

  }
  void hide() {

  }
  String getName() {
    return name;
  }
  Element getCanvas() {
    return guiController.container;
  }
}