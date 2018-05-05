library BlockMessageSender;
import 'BlockGameController.dart';
import '../../User.dart';
import 'Entities/Player.dart';
import 'dart:typed_data';
import 'Blocks/Block.dart';

class MessageSender {
  BlockGameController controller;
  MessageSender(BlockGameController c) {
    controller = c;
  }
  void sendPositionUpdateToAll(Player p) {
    ByteData buffer = new ByteData(21);
    buffer.setUint8(0, 1);
    buffer.setUint32(1, p.id);
    buffer.setFloat64(5, p.x);
    buffer.setFloat64(13, p.y);
    for (User user in controller.usersViewing) {
      user.sendBinary(buffer);
    }
  }
  void sendUpdateBlockColorToAll(Block b) {
    Map m = new Map();
    m['Controller'] = "Game";
    m['Title'] = 'Update Block Color';
    m['ID'] = b.id;
    m['Color'] = b.color;
    for (User user in controller.usersViewing) {
      user.sendMap(m);
    }
  }
  void sendAddBlockToAll(Block b) {
    Map m = new Map();
    m['Controller'] = "Game";
    m['Title'] = 'Add Block';
    m['ID'] = b.id;
    m['X'] = b.x;
    m['Y'] = b.y;
    m['Color'] = b.color;
    for (User user in controller.usersViewing) {
      user.sendMap(m);
    }
  }
  void sendRemoveBlockToAll(int id) {
    Map m = new Map();
    m['Controller'] = "Game";
    m['Title'] = 'Remove Block';
    m['ID'] = id;
    for (User user in controller.usersViewing) {
      user.sendMap(m);
    }
  }
  void sendAllBlocksTo(User u) {
    for (int i = 0; i < controller.blockController.blocks.length; i++) {
      Block b = controller.blockController.blocks[i];
      Map m = new Map();
      m['Controller'] = "Game";
      m['Title'] = 'Add Block';
      m['ID'] = b.id;
      m['X'] = b.x;
      m['Y'] = b.y;
      m['Color'] = b.color;
      u.sendMap(m);
    }
  }
  void sendSelfID(User u, int id) {
    Map m = new Map();
    m['Controller'] = "Game";
    m['Title'] = 'Self ID';
    m['ID'] = id;
    u.sendMap(m);
  }
  void sendAllPlayersTo(User u) {
    for (Player p in controller.entityController.players) {
      Map m = new Map();
      m['Controller'] = "Game";
      m['Title'] = 'Add Player';
      m['ID'] = p.id;
      m['Name'] = p.name;
      m['X'] = p.x;
      m['Y'] = p.y;
      u.sendMap(m);
    }
  }
  void sendAddPlayerToAll(Player p) {
    Map m = new Map();
    m['Controller'] = "Game";
    m['Title'] = 'Add Player';
    m['ID'] = p.id;
    m['Name'] = p.name;
    m['X'] = p.x;
    m['Y'] = p.y;
    for (User user in controller.usersViewing) {
      user.sendMap(m);
    }
  }
  void sendRemovePlayerToAll(int id) {
    Map m = new Map();
    m['Controller'] = "Game";
    m['Title'] = 'Remove Player';
    m['ID'] = id;
    for (User user in controller.usersViewing) {
      user.sendMap(m);
    }
  }
}