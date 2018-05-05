library TagMessageSender;
import 'TagController.dart';
import 'TagUser.dart';
import 'dart:typed_data';

class MessageSender {
  TagController controller;
  MessageSender(TagController c) {
    controller = c;
  }
  void sendPositionUpdateToAll(TagUser tu) {
    /*
    Map m = new Map();
    m['Controller'] = "Game";
    m['Title'] = 'Position Update';
    m['ID'] = tu.id;
    m['X'] = tu.x;
    m['Y'] = tu.y;
    for (TagUser user in controller.usersViewing) {
      user.user.send(m);
    }*/
    ByteData buffer = new ByteData(21);
    buffer.setUint8(0, 1);
    buffer.setUint32(1, tu.id);
    buffer.setFloat64(5, tu.x);
    buffer.setFloat64(13, tu.y);
    for (TagUser user in controller.usersViewing) {
      user.user.sendBinary(buffer);
    }
  }
  void sendAllUsersTo(TagUser tu) {
    for (TagUser user in controller.usersViewing) {
      Map m = new Map();
      m['Controller'] = "Game";
      m['Title'] = 'Add User';
      m['ID'] = user.id;
      m['Name'] = user.name;
      m['X'] = user.x;
      m['Y'] = user.y;
      tu.user.sendMap(m);
    }
  }
  void sendAddUserToAll(TagUser tu) {
    Map m = new Map();
    m['Controller'] = "Game";
    m['Title'] = 'Add User';
    m['ID'] = tu.id;
    m['Name'] = tu.name;
    m['X'] = tu.x;
    m['Y'] = tu.y;
    for (TagUser user in controller.usersViewing) {
      user.user.sendMap(m);
    }
  }
  void sendRemoveUserToAll(TagUser tu) {
    Map m = new Map();
    m['Controller'] = "Game";
    m['Title'] = 'Remove User';
    m['ID'] = tu.id;
    for (TagUser user in controller.usersViewing) {
      user.user.sendMap(m);
    }
  }
  void sendNameChangeToAll(TagUser tu) {
    Map m = new Map();
    m['Controller'] = "Game";
    m['Title'] = 'Name Change';
    m['ID'] = tu.id;
    m['Name'] = tu.name;
    for (TagUser user in controller.usersViewing) {
      user.user.sendMap(m);
    }
  }
  void sendTaggedToAll(int tagged) {
    /*
    Map m = new Map();
    m['Controller'] = "Game";
    m['Title'] = 'Tagged';
    m['ID'] = tagged;
    for (TagUser user in controller.usersViewing) {
      user.user.send(m);
    }*/
    ByteData buffer = new ByteData(21);
    buffer.setUint8(0, 2);
    buffer.setUint32(1, tagged);
    for (TagUser user in controller.usersViewing) {
      user.user.sendBinary(buffer);
    }
  }
  void sendTaggedToUser(TagUser tu, int tagged) {
    Map m = new Map();
    m['Controller'] = "Game";
    m['Title'] = 'Tagged';
    m['ID'] = tagged;
    tu.user.sendMap(m);
  }
}
