library TagController;
import '../AbstractGame.dart';
import 'dart:html';
import '../GameController.dart';
import 'dart:typed_data';
import 'TagUser.dart';
import 'MessageSender.dart';
import 'dart:math';

class TagController extends AbstractGame {
  CanvasElement gameCanvas;
  String name = 'Tag';
  GameController controller;
  int width, height;
  List<TagUser> users = new List();
  int taggedUser = -1;
  MessageSender messageSender;
  bool leftArrow = false,
      rightArrow = false,
      upArrow = false,
      downArrow = false;
  TagController(GameController c) {
    controller = c;
    messageSender = new MessageSender(this);
    gameCanvas = new CanvasElement();
    gameCanvas.style
        ..position = "absolute"
        ..left = "0px"
        ..top = "0px";
    window.onKeyDown.listen((KeyboardEvent ke) {
      if ((ke.keyCode == 37 || ke.keyCode == 65) && leftArrow != true) {
        leftArrow = true;
        messageSender.sendLeftArrowDown();
      }
      if ((ke.keyCode == 38 || ke.keyCode == 87) && upArrow != true) {
        upArrow = true;
        messageSender.sendUpArrowDown();
      }
      if ((ke.keyCode == 39 || ke.keyCode == 68) && rightArrow != true) {
        rightArrow = true;
        messageSender.sendRightArrowDown();
      }
      if ((ke.keyCode == 40 || ke.keyCode == 83) && downArrow != true) {
        downArrow = true;
        messageSender.sendDownArrowDown();
      }
      //ke.preventDefault();
    });
    window.onKeyUp.listen((KeyboardEvent ke) {
      if ((ke.keyCode == 37 || ke.keyCode == 65) && leftArrow != false) {
        leftArrow = false;
        messageSender.sendLeftArrowUp();
      }
      if ((ke.keyCode == 38 || ke.keyCode == 87) && upArrow != false) {
        upArrow = false;
        messageSender.sendUpArrowUp();
      }
      if ((ke.keyCode == 39 || ke.keyCode == 68) && rightArrow != false) {
        rightArrow = false;
        messageSender.sendRightArrowUp();
      }
      if ((ke.keyCode == 40 || ke.keyCode == 83) && downArrow != false) {
        downArrow = false;
        messageSender.sendDownArrowUp();
      }
      //ke.preventDefault();
    });
  }
  void logic() {
    draw();
  }
  void draw() {
    CanvasRenderingContext2D ctx = gameCanvas.context2D;
    if (width != window.innerWidth || height != window.innerHeight) {
      width = window.innerWidth;
      height = window.innerHeight;
      gameCanvas.width = window.innerWidth;
      gameCanvas.height = window.innerHeight;
    }
    ctx.clearRect(0, 0, width, height);
    int xOffset = width ~/ 2;
    int yOffset = height ~/ 2;
    ctx.fillStyle = "Black";
    ctx.lineWidth = 2;
    ctx.font = "15px Arial";
    ctx.textAlign = 'center';
    for (TagUser u in users) {
      if (u.id == taggedUser) {
        ctx.strokeStyle = "Red";
      } else {
        ctx.strokeStyle = "Blue";
      }
      ctx.beginPath();
      ctx.arc(u.x + xOffset, u.y + yOffset, 10, 0, 2 * PI, false);
      ctx.stroke();
      ctx.strokeStyle = 'white';
      ctx.strokeText(u.name, u.x + xOffset, u.y + yOffset - 15);
      ctx.fillText(u.name, u.x + xOffset, u.y + yOffset - 15);
    }
  }
  void onMessage(Map m) {
    switch (m['Title']) {
      case 'Add User':
        {
          int id = m['ID'];
          String name = m['Name'];
          double x = m['X'];
          double y = m['Y'];
          users.add(new TagUser(id, name, x, y));
        }
        break;
      case 'Position Update':
        {
          int id = m['ID'];
          double x = m['X'];
          double y = m['Y'];
          TagUser tu = getUserForID(id);
          tu.x = x;
          tu.y = y;
        }
        break;
      case 'Remove User':
        {
          int id = m['ID'];
          TagUser tu = getUserForID(id);
          users.remove(tu);
        }
        break;
      case 'Name Change':
        {
          int id = m['ID'];
          String name = m['Name'];
          TagUser tu = getUserForID(id);
          tu.name = name;
        }
        break;
      case 'Tagged':
        {
          int id = m['ID'];
          taggedUser = id;
        }
        break;
    }
  }
  TagUser getUserForID(int id) {
    for (TagUser u in users) {
      if (u.id == id) {
        return u;
      }
    }
    return null;
  }
  void onBinaryMessage(ByteData buffer) {
    int binaryMsg = buffer.getUint8(0);
    switch (binaryMsg) {
      case 1:
        { // Position Update
          int id = buffer.getUint32(1);
          double x = buffer.getFloat64(5);
          double y = buffer.getFloat64(13);
          TagUser tu = getUserForID(id);
          tu.x = x;
          tu.y = y;
        }
        break;
      case 2:
        { // Tagged Update
          int id = buffer.getUint32(1);
          taggedUser = id;
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
  CanvasElement getCanvas() {
    return gameCanvas;
  }
}
