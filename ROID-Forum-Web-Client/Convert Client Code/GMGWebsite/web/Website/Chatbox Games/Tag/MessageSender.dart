library TagMessageSender;
import 'TagController.dart';
import 'dart:typed_data';

class MessageSender {
  TagController controller;
  MessageSender(TagController c) {
    controller = c;
  }
  void sendLeftArrowDown() {
    /*
    Map m = new Map();
    m['Controller'] = "Game";
    m['Game'] = "Data";
    m['Title'] = 'Key Down';
    m['Data'] = 'Left';
    controller.controller.website.networkingController.Send(m);
    */
    ByteData data = new ByteData(1);
    data.setUint8(0, 0);
    controller.controller.website.networkingController.SendBinary(data.buffer);
  }
  void sendRightArrowDown() {
    /*
    Map m = new Map();
    m['Controller'] = "Game";
    m['Game'] = "Data";
    m['Title'] = 'Key Down';
    m['Data'] = 'Right';
    controller.controller.website.networkingController.Send(m);
    */
    ByteData data = new ByteData(1);
    data.setUint8(0, 1);
    controller.controller.website.networkingController.SendBinary(data.buffer);
  }
  void sendUpArrowDown() {
    /*
    Map m = new Map();
    m['Controller'] = "Game";
    m['Game'] = "Data";
    m['Title'] = 'Key Down';
    m['Data'] = 'Up';
    controller.controller.website.networkingController.Send(m);
    * */
    ByteData data = new ByteData(1);
    data.setUint8(0, 2);
    controller.controller.website.networkingController.SendBinary(data.buffer);
  }
  void sendDownArrowDown() {
    /*
    Map m = new Map();
    m['Controller'] = "Game";
    m['Game'] = "Data";
    m['Title'] = 'Key Down';
    m['Data'] = 'Down';
    controller.controller.website.networkingController.Send(m);
    */
    ByteData data = new ByteData(1);
    data.setUint8(0, 3);
    controller.controller.website.networkingController.SendBinary(data.buffer);
  }
  void sendLeftArrowUp() {
    /*
    Map m = new Map();
    m['Controller'] = "Game";
    m['Game'] = "Data";
    m['Title'] = 'Key Up';
    m['Data'] = 'Left';
    controller.controller.website.networkingController.Send(m);
    */
    ByteData data = new ByteData(1);
    data.setUint8(0, 4);
    controller.controller.website.networkingController.SendBinary(data.buffer);
  }
  void sendRightArrowUp() {
    /*
    Map m = new Map();
    m['Controller'] = "Game";
    m['Game'] = "Data";
    m['Title'] = 'Key Up';
    m['Data'] = 'Right';
    controller.controller.website.networkingController.Send(m);
    */
    ByteData data = new ByteData(1);
    data.setUint8(0, 5);
    controller.controller.website.networkingController.SendBinary(data.buffer);
  }
  void sendDownArrowUp() {
    /*
    Map m = new Map();
    m['Controller'] = "Game";
    m['Game'] = "Data";
    m['Title'] = 'Key Up';
    m['Data'] = 'Down';
    controller.controller.website.networkingController.Send(m);
    */
    ByteData data = new ByteData(1);
    data.setUint8(0, 6);
    controller.controller.website.networkingController.SendBinary(data.buffer);
  }
  void sendUpArrowUp() {
    /*
    Map m = new Map();
    m['Controller'] = "Game";
    m['Game'] = "Data";
    m['Title'] = 'Key Up';
    m['Data'] = 'Up';
    controller.controller.website.networkingController.Send(m);
    */
    ByteData data = new ByteData(1);
    data.setUint8(0, 7);
    controller.controller.website.networkingController.SendBinary(data.buffer);
  }
}
